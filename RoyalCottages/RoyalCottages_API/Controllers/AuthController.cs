using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoyalCottage_DTO;
using RoyalCottages_API.Data;
using RoyalCottages_API.Services;

namespace RoyalCottages_API.Controllers
{
    // Controller for handling authentication-related endpoints
    [Route("api/auth")] // Base route for the controller
    [ApiController]
    public class AuthController : ControllerBase
    {
        // Inject the AuthService into the controller
        private readonly IAuthService _authService;

        // Constructor for Database - dependency injection
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/auth/register
        [HttpPost("register")] // Route for the registration endpoint
        // ProducesResponseType - To specify the response type and status code
        [ProducesResponseType(typeof(ApiResponse<UserDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<UserDTO>>> Register([FromBody] RegistrationRequestDTO registrationRequestDTO)
        {
            // Wrap the registration logic in a try-catch block to handle exceptions
            try
            {
                // Validate the registration request
                if (registrationRequestDTO == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Invalid registration request- data is required"));
                }

                // Check if the email already exists in the database
                if (await _authService.EmailExistsAsync(registrationRequestDTO.Email))
                {
                    return Conflict(ApiResponse<object>.Conflict($"User with email {registrationRequestDTO.Email} already exists"));
                }

                // Register the user and get the user object
                var user = await _authService.RegisterAsync(registrationRequestDTO);

                // Check if the user object is null, indicating that registration failed
                if (user == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("User registration failed"));
                }

                // Authenticate the user and get the token
                var response = ApiResponse<UserDTO>.Ok(user, "User registered successfully");

                // Return the response with the CreatedAtAction method, which returns a
                // 201 Created status code and the location of the newly created resource
                return CreatedAtAction(nameof(Register), response);
            }
            catch (Exception ex)
            {
                // Return a 500 status code and an error message
                var errorResponse = ApiResponse<object>.Error(500, "An error occurred during registration", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }

        // POST: api/auth/login
        [HttpPost("login")] // Route for the login endpoint
        // ProducesResponseType - To specify the response type and status code
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<LoginResponseDTO>>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<LoginResponseDTO>>> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            // Wrap the login logic in a try-catch block to handle exceptions
            try
            {
                // Validate the login request
                if (loginRequestDTO == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Invalid login request - data is required"));
                }

                // Register the user and get the user object
                var loginResponse = await _authService.LoginAsync(loginRequestDTO);

                // Check if the user object is null, indicating that registration failed
                if (loginResponse == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("User login failed"));
                }

                // Authenticate the user and get the token
                var response = ApiResponse<LoginResponseDTO>.Ok(loginResponse, "User logged in successfully");

                // Return the response with the CreatedAtAction method, which returns a
                // 201 Created status code and the location of the newly created resource
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Return a 500 status code and an error message
                var errorResponse = ApiResponse<object>.Error(500, "An error occurred during login process", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }
    }
}
