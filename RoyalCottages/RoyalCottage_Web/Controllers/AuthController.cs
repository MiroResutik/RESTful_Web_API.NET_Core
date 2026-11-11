using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RoyalCottage_DTO;
using RoyalCottage_Web.Models;
using RoyalCottage_Web.Services.IServices;
using System.Diagnostics;

namespace RoyalCottage_Web.Controllers
{
    public class AuthController : Controller
    {
        // Inject Dependencies 
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        // Constructor to initialize dependencies
        public AuthController(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequestDTO)
        {
            try
            {
                var response = await _authService.LoginAsync<ApiResponse<LoginResponseDTO>>(loginRequestDTO);

                // Check if the response is successful and contains data
                if (response != null && response.Success && response.Data != null)
                {
                    LoginResponseDTO model= response.Data; 
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the API call and log the error message
                TempData["error"] = $"An error occurred: {ex.Message}";
            }
            
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegistrationRequestDTO
            {
                Email = string.Empty,
                Name = string.Empty,
                Password = string.Empty,
                Role = "Customer",
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            try
            {
                ApiResponse<UserDTO> response = await _authService.RegisterAsync<ApiResponse<UserDTO>>(registrationRequestDTO);

                // Check if the response is successful and contains data
                if (response != null && response.Success && response.Data != null)
                {
                    UserDTO model = response.Data;
                    TempData["success"] = "Registration successful! Please login with your credentials";
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["error"] = response?.Message??  "Registration failed. Please try again";
                    return View(registrationRequestDTO);

                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the API call and log the error message
                TempData["error"] = $"An error occurred: {ex.Message}";
            }

            return View(registrationRequestDTO);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            return View();
        }

    }
}
