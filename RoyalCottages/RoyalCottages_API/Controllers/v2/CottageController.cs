using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoyalCottage_DTO;
using RoyalCottages_API.Data;
using RoyalCottages_API.Models;
using System.Collections;

namespace RoyalCottages_API.Controllers.v2
{
    // Route attribute to specify the base route for the controller
    [Route("api/v{version:apiVersion}/cottage")]
    [ApiVersion("2.0")]
    [ApiController]
    //[Authorize] // Authorize attribute to restrict access to authenticated users
    //[AllowAnonymous] // Allow anyone to access - only for development phase
    public class CottageController : ControllerBase
    {
        // Access the Database - access entity framework core
        private readonly ApplicationDbContext _db;

        // Access the Mapper - access AutoMapper
        private readonly IMapper _mapper;

        // Constructor for Database - dependency injection
        public CottageController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        // Endpoint to get all Cottages
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CottageDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<CottageDTO>>>> GetCottages([FromQuery] string? filterBy,
            [FromQuery] string? filterQuery) // Accept filterBy and filterQuery as query parameters for filtering the cottages
        {
            // Get all cottages from the database using linq query operation
            var cottagesQuery = _db.Cottage.AsQueryable();

            // Apply filtering based on the filterBy and filterQuery parameters if they are provided
            if (!string.IsNullOrEmpty(filterQuery) && (!string.IsNullOrEmpty(filterBy)))
            {
                // Apply filtering based on the filterBy parameter
                switch (filterBy.ToLower())
                {
                    case "id":
                        if (int.TryParse(filterQuery, out int id))
                        {
                            cottagesQuery = cottagesQuery.Where(u => u.Id == id);
                        }
                        break;
                    case "name":
                        cottagesQuery = cottagesQuery.Where(u => u.Name.ToLower().Contains(filterQuery.ToLower()));
                        break;
                    case "details":
                        cottagesQuery = cottagesQuery.Where(u => u.Details.ToLower().Contains(filterQuery.ToLower()));
                        break;
                    case "rate":
                        if (double.TryParse(filterQuery, out double rate))
                        {
                            cottagesQuery = cottagesQuery.Where(u => u.Rate == rate);
                        }
                        break;
                    case "occupancy":
                        if (int.TryParse(filterQuery, out int occupancy))
                        {
                            cottagesQuery = cottagesQuery.Where(u => u.Occupancy == occupancy);
                        }
                        break;
                    case "minrate":
                        if (double.TryParse(filterQuery, out double minRate))
                        {
                            cottagesQuery = cottagesQuery.Where(u => u.Rate >= minRate);
                        }
                        break;
                    case "maxrate":
                        if (double.TryParse(filterQuery, out double maxRate))
                        {
                            cottagesQuery = cottagesQuery.Where(u => u.Rate <= maxRate);
                        }
                        break;
                    case "minoccupancy":
                        if (int.TryParse(filterQuery, out int minOccupancy))
                        {
                            cottagesQuery = cottagesQuery.Where(u => u.Occupancy >= minOccupancy);
                        }
                        break;
                    case "maxoccupancy":
                        if (int.TryParse(filterQuery, out int maxOccupancy))
                        {
                            cottagesQuery = cottagesQuery.Where(u => u.Occupancy <= maxOccupancy);
                        }
                        break;
                    case "minsqmeters":
                        if (int.TryParse(filterQuery, out int minSqMeters))
                        {
                            cottagesQuery = cottagesQuery.Where(u => u.SqMeters >= minSqMeters);
                        }
                        break;
                    case "maxsqmeters":
                        if (int.TryParse(filterQuery, out int maxSqMeters))
                        {
                            cottagesQuery = cottagesQuery.Where(u => u.SqMeters <= maxSqMeters);
                        }
                        break;
                }

            }
            // Get the filtered cottages from the database and convert them to a list
            var cottages = await cottagesQuery.ToListAsync();

            // Map the cottages to CottageDTO objects and return them in an ApiResponse object with a 200 status code and a success message
            var dtoResponseCottage = _mapper.Map<List<CottageDTO>>(cottages);

            // Return the cottages with a 200 status code and a success message
            var response = ApiResponse<IEnumerable<CottageDTO>>.Ok(dtoResponseCottage, "Cottages retrieved successfully");
            return Ok(response);
        }

        // Endpoint to get  Cottage by Id
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<CottageDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CottageDTO>>> GetCottageById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return NotFound(ApiResponse<object>.NotFound("Cottage ID must be greater than 0"));

                }

                // Get cottage by ID using linq query operation
                var cottage = await _db.Cottage.FirstOrDefaultAsync(x => x.Id == id);

                // Check if cottage is populated
                if (cottage == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"Cottage with ID {id} was not found!"));
                    //return NotFound($"Cottage with ID {id} was not found!");
                }

                // Return the cottage with a 200 status code and a success message
                return Ok(ApiResponse<CottageDTO>.Ok(_mapper.Map<CottageDTO>(cottage), "Records retrieved successfully"));

            }
            catch (Exception ex)
            {
                // Return a 500 status code and an error message
                var errorResponse = ApiResponse<object>.Error(500, "An error occurred while retrieving cottage with ID", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }

        // Endpoint to create Cottage
        [HttpPost]
        //[Authorize(Roles = "Admin")] // Authorize attribute to restrict access to users with the "Customer" or "Admin" roles
        [ProducesResponseType(typeof(ApiResponse<CottageDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<CottageDTO>>> CreateCottage(CottageCreateDTO cottageDTO)
        {
            try
            {
                if (cottageDTO == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Cottage data is required"));

                }
                // Check for duplicate cottage name (case-insensitive) excluding the current cottage being updated
                var duplicateCottage = await _db.Cottage.FirstOrDefaultAsync(v => v.Name.ToLower() == cottageDTO.Name.ToLower());

                // Check if a duplicate cottage exists
                if (duplicateCottage != null)
                {
                    return Conflict(ApiResponse<object>.Conflict($"A Cottage with the name '{cottageDTO.Name}' already exists!!!"));
                }

                // Convert to cottage object for _db
                Cottage cottage = _mapper.Map<Cottage>(cottageDTO);

                // Create Cottage and send it and save it in DB
                await _db.Cottage.AddAsync(cottage);
                await _db.SaveChangesAsync();

                // Return the created cottage with a 201 status code and a success message
                var response = ApiResponse<CottageDTO>.CreatedAt(_mapper.Map<CottageDTO>(cottage), "Cottage created successfully");

                // Return the created cottage with a 201 status code and the location of the new resource
                return CreatedAtAction(nameof(GetCottageById), new { id = cottage.Id }, response);

            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(500, "An error occurred while creating cottage", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }

        // Endpoint to update Cottage - ActionResult - To return a response to the client
        [HttpPut("{id:int}")]
        //[Authorize(Roles = "Admin")] // Authorize attribute to restrict access to users with the "Customer" or "Admin" roles
        [ProducesResponseType(typeof(ApiResponse<CottageDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CottageDTO>>> UpdateCottage(int id, CottageUpdateDTO cottageDTO)
        {
            try
            {
                if (cottageDTO == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Cottage data is required"));

                }
                // Check if the ID in the URL matches the ID in the request body
                if (id != cottageDTO.Id)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Cottage ID in URL does not match Cottage ID in request body!!!"));

                }
                // Get the existing cottage from the database
                var existingCottage = await _db.Cottage.FirstOrDefaultAsync(v => v.Id == id);

                // Check if the existing cottage is null
                if (existingCottage == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"Cottage with ID {id} was not found!"));
                }

                // Check for duplicate cottage name (case-insensitive) excluding the current cottage being updated
                var duplicateCottage = await _db.Cottage.FirstOrDefaultAsync(v => v.Name.ToLower() == cottageDTO.Name.ToLower() && v.Id != id);

                // Check if a duplicate cottage exists
                if (duplicateCottage != null)
                {
                    return Conflict(ApiResponse<object>.Conflict($"A Cottage with the name '{cottageDTO.Name}' already exists!!!"));

                }

                // Map the updated properties from the DTO to the existing cottage entity
                _mapper.Map(cottageDTO, existingCottage);

                // Update the UpdatedDate property to the current date and time
                existingCottage.UpdatedDate = DateTime.UtcNow;

                // Save the changes to the database
                await _db.SaveChangesAsync();

                // Return the updated cottage with a 200 status code and a success message
                var response = ApiResponse<CottageDTO>.Ok(_mapper.Map<CottageDTO>(cottageDTO), "Cottage updated successfully");

                // Return the updated cottage with a 200 status code
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(500, "An error occurred while updating cottage", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }

        // Endpoint to update Cottage 
        [HttpDelete("{id:int}")]
        //[Authorize(Roles = "Admin")] // Authorize attribute to restrict access to users with the"Admin" roles
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteCottage(int id)
        {
            try
            {
                var existingCottage = await _db.Cottage.FirstOrDefaultAsync(v => v.Id == id);

                // Check if the existing cottage is null
                if (existingCottage == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"Cottage with ID {id} was not found!"));
                }

                // Remove the existing cottage from the database
                _db.Cottage.Remove(existingCottage);

                await _db.SaveChangesAsync();

                // Return a 200 status code and a success message
                var response = ApiResponse<object>.NoContent("Cottage deleted successfully");
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(500, "An error occurred while deleting the cottage", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }

    }
}
