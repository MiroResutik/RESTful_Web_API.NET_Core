using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoyalCottage_DTO;
using RoyalCottages_API.Data;
using RoyalCottages_API.Models;

namespace RoyalCottages_API.Controllers
{
    [Route("api/cottage-amenities")]
    [ApiController]
    [AllowAnonymous] // Allow anyone to access - only for development phase
    //[Authorize(Roles = "Customer, Admin")] // Authorize attribute to restrict access to users with the "Customer" or "Admin" roles

    public class CottageAmenitiesController : ControllerBase
    {
        // Access the Database - access entity framework core
        private readonly ApplicationDbContext _db;

        private readonly IMapper _mapper;

        // Constructor for Database - dependency injection
        public CottageAmenitiesController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        // [Authorize(Roles ="Admin")] // Overwrite at action level - Authorize attribute to restrict access to users with the "Admin" role 
        // ProducesResponseType - To specify the response type and status code
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CottageAmenitiesDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<CottageAmenitiesDTO>>>> GetCottageAmenities()
        {
            var cottageAmenities = await _db.CottageAmenities.ToListAsync();
            var dtoResponseCottageAmenities = _mapper.Map<List<CottageAmenitiesDTO>>(cottageAmenities);
            var response = ApiResponse<IEnumerable<CottageAmenitiesDTO>>.Ok(dtoResponseCottageAmenities, "Cottage amenities retrieved successfully");
            return Ok(response);
        }

        // Endpoint to get  Cottage Amenity by Id
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<CottageAmenitiesDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CottageAmenitiesDTO>>> GetCottageAmenitiesById(int id)
        {
            try
            {
                // ID Validation
                if (id <= 0)
                {
                    return NotFound(ApiResponse<object>.NotFound("Cottage amenity ID must be greater than 0"));
                }

                // Get cottage amenity by ID using linq query operation
                var cottageAmenity = await _db.CottageAmenities.FirstOrDefaultAsync(x => x.Id == id);

                // Check if cottage amenity is populated
                if (cottageAmenity == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"Cottage amenity with ID {id} was not found"));
                }

                // Return the cottage amenity with a 200 status code and a success message
                return Ok(ApiResponse<CottageAmenitiesDTO>.Ok(_mapper.Map<CottageAmenitiesDTO>(cottageAmenity), "Records retrieved successfully"));
            }
            catch (Exception ex)
            {
                // Return a 500 status code and an error message
                var errorResponse = ApiResponse<object>.Error(500, "An error occurred while retrieving cottage amenity with ID", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }

        // Endpoint to create Cottage
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CottageAmenitiesDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<CottageAmenitiesDTO>>> CreateCottageAmenity(CottageAmenitiesCreateDTO cottageAmenitiesDTO)
        {
            try
            {
                // ID Validation
                if (cottageAmenitiesDTO == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Cottage amenity data is required"));
                }

                // Check if the CottageId exists in the Cottage table
                var cottageExists = await _db.CottageAmenities.FirstOrDefaultAsync(v => v.Id == cottageAmenitiesDTO.CottageId);

                // Check if the cottage exists, if not return a 409 Conflict response with an error message
                if (cottageExists == null)
                {
                    return Conflict(ApiResponse<object>.Conflict($"A Cottage with the ID '{cottageAmenitiesDTO.CottageId}' does not exists"));
                }

                // Convert to cottage amenity object for _db
                CottageAmenities cottageAmenities = _mapper.Map<CottageAmenities>(cottageAmenitiesDTO);

                cottageAmenities.CreatedDate= DateTime.Now;

                // Create Cottage and send it and save it in DB
                await _db.CottageAmenities.AddAsync(cottageAmenities);
                await _db.SaveChangesAsync();

                // Return the created cottage with a 201 status code and a success message
                var response = ApiResponse<CottageAmenitiesDTO>.CreatedAt(_mapper.Map<CottageAmenitiesDTO>(cottageAmenities), "Cottage amenity created successfully");

                // Return the created cottage with a 201 status code and the location of the new resource
                return CreatedAtAction(nameof(CreateCottageAmenity), new { id = cottageAmenities.Id }, response);

            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(500, "An error occurred while creating cottage amenity", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }

        // Endpoint to update Cottage Amenity - ActionResult - To return a response to the client
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<CottageAmenitiesDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CottageAmenitiesDTO>>> UpdateCottageAmenity(int id, CottageAmenitiesUpdateDTO cottageAmenitiesDTO)
        {
            try
            {
                // ID Validation
                if (cottageAmenitiesDTO == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Cottage Amenity data is required"));
                }

                // Check if the ID in the URL matches the ID in the request body
                if (id != cottageAmenitiesDTO.Id)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Cottage Amenity ID in URL does not match Cottage Amenity ID in request body"));
                }
                // Get the existing cottage from the database
                var existingCottageAmenities = await _db.CottageAmenities.FirstOrDefaultAsync(v => v.Id == id);

                // Check if the existing cottage is null
                if (existingCottageAmenities == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"Cottage Amenity with ID {id} was not found"));
                }

                // Check if the CottageId exists in the Cottage table
                var cottageExists = await _db.Cottage.FirstOrDefaultAsync(v => v.Id == cottageAmenitiesDTO.CottageId);

                // Check if the cottage exists, if not return a 409 Conflict response with an error message
                if (cottageExists == null)
                {
                    return Conflict(ApiResponse<object>.Conflict($"A Cottage with the ID '{cottageAmenitiesDTO.CottageId}' does not exists"));
                }

                // Map the updated properties from the DTO to the existing cottage entity
                _mapper.Map(cottageAmenitiesDTO, existingCottageAmenities);

                // Update the UpdatedDate property to the current date and time
                existingCottageAmenities.UpdatedDate = DateTime.Now;

                // Save the changes to the database
                await _db.SaveChangesAsync();

                // Return the updated cottage with a 200 status code and a success message
                var response = ApiResponse<CottageAmenitiesDTO>.Ok(_mapper.Map<CottageAmenitiesDTO>(existingCottageAmenities), "Cottage Amenity updated successfully");

                // Return the updated cottage with a 200 status code
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(500, "An error occurred while updating cottage amenity", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }

        // Endpoint to update Cottage Amenity
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteCottageAmenity(int id)
        {
            try
            {
                // ID Validation
                var existingCottageAmenity = await _db.CottageAmenities.FirstOrDefaultAsync(v => v.Id == id);

                // Check if the existing cottage is null
                if (existingCottageAmenity == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"Cottage amenity with ID {id} was not found"));
                }

                // Remove the existing cottage from the database
                _db.CottageAmenities.Remove(existingCottageAmenity);

                await _db.SaveChangesAsync();

                // Return a 200 status code and a success message
                var response = ApiResponse<object>.NoContent("Cottage amenity deleted successfully");
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(500, "An error occurred while deleting the cottage amenity", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }

    }
}
