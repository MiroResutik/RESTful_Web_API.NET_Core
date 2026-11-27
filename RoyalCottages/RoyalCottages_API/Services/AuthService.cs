using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RoyalCottage_DTO;
using RoyalCottages_API.Data;
using RoyalCottages_API.Models;
using RoyalCottages_API.Services.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RoyalCottages_API.Services
{
    // Service for handling authentication-related operations
    public class AuthService : IAuthService
    {
        // Inject the database context into the service
        private readonly ApplicationDbContext _db;

        // Inject the UserManager and RoleManager for managing users and roles
        private readonly UserManager<ApplicationUser> _userManager;

        // Inject the RoleManager for managing roles
        private readonly RoleManager<IdentityRole> _roleManager;

        // Inject the configuration into the service
        private readonly IConfiguration _configuration;
        // Inject the AutoMapper into the service
        private readonly IMapper _mapper;

        private readonly ITokenService _tokenService;

        // Constructor for dependency injection
        public AuthService(
            ApplicationDbContext db, 
            IConfiguration configuration, 
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ITokenService tokenService)
        {
            _db = db;
            _configuration = configuration;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
        }

        // Endpoint to check if an email already exists
        public async Task<bool> EmailExistsAsync(string email)
        {
            // Check if any user in the database has the specified email (case-insensitive)
            return await _db.ApplicationUsers.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        // Endpoint for user login
        public async Task<TokenDTO?> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            // Validate the login request
            try
            {
                // Find the user in the database by email (case-insensitive)
                var user = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.Email.ToLower() == loginRequestDTO.Email.ToLower());

                // If the user is not found, return null
                if (user == null)
                {
                    return null; // user not found
                }

                // Check if the provided password is valid for the found user
                bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

                // If the password is invalid, return null
                if (!isValid)
                {
                    return null; // invalid password
                }

                // Generate a JWT token for the authenticated user
                var token = await _tokenService.GenerateJwtTokenAsync(user);

                // Retrieve the roles of the user
                var roles = await _userManager.GetRolesAsync(user);

                // Create a TokenDTO to return the generated token
                TokenDTO tokenDTO =  new TokenDTO
                {
                    //// Map the ApplicationUser to UserDTO and include the generated token
                    //UserDTO = _mapper.Map<UserDTO>(user),
                    AccessToken = token
                };

                //// Set the user's role in the response, defaulting to "Customer" if no roles are found
                //loginResponseDTO.UserDTO.Role = roles.FirstOrDefault()??"Customer";

                return tokenDTO;
            }
            catch (Exception ex)
            {
                // Log the exception and throw a new exception with a user-friendly message
                throw new InvalidOperationException($"Error occurred during user login: {ex.Message}");
            }
        }

        // Endpoint for user registration
        public async Task<UserDTO?> RegisterAsync(RegistrationRequestDTO registrationRequestDTO)
        {
            try
            {
                // Check if the email already exists in the database
                if (await EmailExistsAsync(registrationRequestDTO.Email))
                {
                    throw new InvalidOperationException($"User with email {registrationRequestDTO.Email} already exists");
                }
                // Create a new ApplicationUser object with the provided registration details
                ApplicationUser user = new()
                {
                    Email = registrationRequestDTO.Email,
                    Name = registrationRequestDTO.Name,
                    UserName = registrationRequestDTO.Email,
                    NormalizedEmail = registrationRequestDTO.Email.ToUpper(),
                    EmailConfirmed = true
                    
                };

                // Create the user in the database using UserManager
                var result = await _userManager.CreateAsync(user,registrationRequestDTO.Password);

                // Check if the user creation was successful
                if (!result.Succeeded)
                {
                    // If user creation failed, throw an exception with the error messages
                    var errors = string.Join(",", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"User registration failed: {errors}");

                }

                // Determine the role for the user, defaulting to "Customer" if not provided
                var role = string.IsNullOrEmpty(registrationRequestDTO.Role) ? "Customer" : registrationRequestDTO.Role;

                // Check if the role exists, and create it if it doesn't
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }

                // Assign the user to the specified role
                await _userManager.AddToRoleAsync(user, role);


                // Map the ApplicationUser to UserDTO and set the role
                var userDTO = _mapper.Map<UserDTO>(user);
                userDTO.Role = role;

                return userDTO;
            }
            catch (Exception ex)
            {
                // Log the exception and throw a new exception with a user-friendly message
                throw new InvalidOperationException($"Error occurred while registering user: {ex.Message}");
            }
        }

    }
}
