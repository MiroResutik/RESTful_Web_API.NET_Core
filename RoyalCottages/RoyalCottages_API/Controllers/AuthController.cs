using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoyalCottages_API.Data;
using RoyalCottages_API.Models.DTO;
using RoyalCottages_API.Services;

namespace RoyalCottages_API.Controllers
{
    // Controller for handling authentication-related endpoints
    [Route("api/auth")]
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
        [HttpPost]
        // ProducesResponseType - To specify the response type and status code
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CottageDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<UserDTO>>> Register([FromBody] RegistrationRequestDTO registrationRequestDTO)
        {
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
    }
}
