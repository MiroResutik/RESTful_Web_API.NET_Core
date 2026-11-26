using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RoyalCottage_DTO;
using RoyalCottage_Web.Models;
using RoyalCottage_Web.Services.IServices;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RoyalCottage_Web.Controllers
{
    public class AuthController : Controller
    {
        // Inject Dependencies 
        private readonly IAuthService _authService;

        // Inject IMapper for mapping between DTOs and models
        private readonly IMapper _mapper;

        // Inject ITokenProvider for handling JWT tokens
        private readonly ITokenProvider _tokenProvider;

        // Constructor to initialize dependencies
        public AuthController(IAuthService authService, IMapper mapper, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _mapper = mapper;
            _tokenProvider = tokenProvider;
        }

        // Display the login page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Handle the login form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequestDTO)
        {
            try
            {
                // Call the LoginAsync method of the IAuthService to authenticate the user
                var response = await _authService.LoginAsync<ApiResponse<TokenDTO>>(loginRequestDTO);

                // Check if the response is successful and contains data
                if (response != null && response.Success && response.Data != null)
                {

                    // Create a ClaimsPrincipal from the JWT token
                    var principal = _tokenProvider.CreatePrincipalFromJwtToken(response.Data.AccessToken);

                    // Check if the ClaimsPrincipal is not null
                    if (principal != null) 
                    {
                        // Sign in the user and set the authentication cookie
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                        // Store the JWT token in the session for future use
                        _tokenProvider.SetToken(response.Data.AccessToken);

                        // Redirect the user to the home page after successful login
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["error"] = "Invalid token received. Please try again.";
                        return View(loginRequestDTO);
                    }
                }
                else
                {
                    TempData["error"] = response.Message;
                    
                }
                return View(loginRequestDTO);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the API call and log the error message
                TempData["error"] = $"An error occurred: {ex.Message}";
            }
            
            return View();
        }

        // Display the registration page
        [HttpGet]
        public IActionResult Register()
        {
            // Return a new instance of RegistrationRequestDTO to the view for user registration
            return View(new RegistrationRequestDTO
            {
                Email = string.Empty,
                Name = string.Empty,
                Password = string.Empty,
                Role = "Customer",
            });
        }

        // Handle the registration form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            try
            {
                // Call the RegisterAsync method of the IAuthService to register a new user
                ApiResponse<UserDTO> response = await _authService.RegisterAsync<ApiResponse<UserDTO>>(registrationRequestDTO);

                // Check if the response is successful and contains data
                if (response != null && response.Success && response.Data != null)
                {
                    // Retrieve the registered user data from the response
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

        // Display the access denied page
        public IActionResult AccessDenied()
        {
            return View();
        }

        // Handle user logout
        public async Task<IActionResult> Logout()
        {
            // Sign out the user and remove the authentication cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Clear the JWT token from the session
            _tokenProvider.ClearToken(); 

            // Redirect the user to the home page after logout
            return RedirectToAction("Index", "Home");
        }

    }
}
