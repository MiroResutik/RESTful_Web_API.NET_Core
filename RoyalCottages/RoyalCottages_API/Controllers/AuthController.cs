using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoyalCottages_API.Data;
using RoyalCottages_API.Models.DTO;

namespace RoyalCottages_API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // Constructor for Database - dependency injection
        public AuthController()
        {

        }

        // POST: api/auth/register
        [HttpGet]
        // ProducesResponseType - To specify the response type and status code
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CottageDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<UserDTO>>> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            // Authenticate the user and get the token

            var response = ApiResponse<UserDTO>.Ok(null, "User registered successfully");
            return Ok(response);
        }

    }
}
