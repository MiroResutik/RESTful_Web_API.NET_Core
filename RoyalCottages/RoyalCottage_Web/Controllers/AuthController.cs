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
        private readonly IMapper _mapper;

        // Constructor to initialize dependencies
        public AuthController(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
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
                var response = await _authService.LoginAsync<ApiResponse<LoginResponseDTO>>(loginRequestDTO);

                // Check if the response is successful and contains data
                if (response != null && response.Success && response.Data != null)
                {
                    LoginResponseDTO model= response.Data;

                    // Create a JwtSecurityTokenHandler to read the JWT token from the response
                    var handler = new JwtSecurityTokenHandler();
                    var jwt = handler.ReadJwtToken(model.Token);

                    // Create a ClaimsIdentity and add claims for the user's email and role
                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type == "email").Value));
                    identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));

                    // Create a ClaimsPrincipal with the identity and sign in the user using cookie authentication
                    var principal = new ClaimsPrincipal(identity);

                    // Sign in the user and set the authentication cookie
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    // Redirect the user to the home page after successful login
                    return RedirectToAction("Index", "Home");
                }
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
            // Sign out the user and clear the authentication cookie
            await HttpContext.SignOutAsync();

            // Redirect the user to the home page after logout
            return RedirectToAction("Index", "Home");
        }

    }
}
