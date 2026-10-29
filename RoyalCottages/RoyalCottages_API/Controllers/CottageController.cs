using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoyalCottages_API.Data;
using RoyalCottages_API.Models;
using RoyalCottages_API.Models.DTO;
using System.Collections;

namespace RoyalCottages_API.Controllers
{
    // Route attribute to specify the base route for the controller
    [Route("api/cottage")]
    [ApiController]
    public class CottageController : ControllerBase
    {
        // Access the Database - access entity framework core
        private readonly ApplicationDbContext _db;

        private readonly IMapper _mapper;

        // Constructor for Database - dependency injection
        public CottageController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        // Endpoint to get all Cottages
        // ActionResult - To return a response to the client
        // Async - To stop from blocking the thread
        // IEnumerable - To return a collection of cottages
        [HttpGet]
        // ProducesResponseType - To specify the response type and status code
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CottageDTO>>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<CottageDTO>>>> GetCottages()
        {
            var cottages = await _db.Cottage.ToListAsync();
            var dtoResponseCottage = _mapper.Map<List<CottageDTO>>(cottages);
            var response = ApiResponse<IEnumerable<CottageDTO>>.Ok(dtoResponseCottage, "Cottages retrieved successfully");
            return Ok(response);
        }

        // Endpoint to get  Cottage by Id
        // Route parameter binding
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<CottageDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CottageDTO>>> GetCottageById(int id)
        {
            try
            {
                // ID Validation
                if(id <= 0)
                {
                    return NotFound(ApiResponse<object>.NotFound("Cottage ID must be greater than 0"));

                    // Hardcoded Api Response
                    /*
                    return new ApiResponse<CottageDTO>()
                    {
                        StatusCode = 400,
                        Errors = "Cottage ID must be greater than 0",
                        Success = false,
                        Message = "Bad Request"
                    };
                    */
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

                // Hardcoded Api Response
                /*
                return new ApiResponse<CottageDTO>()
                {
                    StatusCode = 200,
                    Success = true,
                    Message = "Records retrieved successfully",
                    Data = _mapper.Map<CottageDTO>(cottage)
                };
                */
        }
        catch (Exception ex)
        {
                // Return a 500 status code and an error message
                var errorResponse = ApiResponse<object>.Error(500, "An error occurred while retrieving cottage with ID", ex.Message);
                return StatusCode(500, errorResponse);
                //return StatusCode(StatusCodes.Status500InternalServerError,
                //$"An error occurred while retrieving cottage with ID {id}: {ex.Message}");
        }
    }

        // Endpoint to create Cottage
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CottageDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<CottageDTO>>> CreateCottage(CottageCreateDTO cottageDTO)
        {
            try
            {
                // ID Validation
                if (cottageDTO == null)
                {
                        return BadRequest(ApiResponse<object>.BadRequest("Cottage data is required"));

                        //return BadRequest("Cottage data is required");
                }
                // Check for duplicate cottage name (case-insensitive) excluding the current cottage being updated
                var duplicateCottage = await _db.Cottage.FirstOrDefaultAsync(v => v.Name.ToLower() == cottageDTO.Name.ToLower());

                // Check if a duplicate cottage exists
                if (duplicateCottage != null)
                {
                        return Conflict(ApiResponse<object>.Conflict($"A Cottage with the name '{cottageDTO.Name}' already exists!!!"));
                        //return Conflict($"A Cottage with the name '{cottageDTO.Name}' already exists!!!");

                }

                // Convert to cottage object for _db
                // Automatic mapping using mapper
                Cottage cottage = _mapper.Map<Cottage>(cottageDTO);

                // Manual mapping
                /*
                Cottage cottage = new()
                {
                    Name = cottageDTO.Name,
                    Details = cottageDTO.Details,
                    ImageUrl = cottageDTO.ImageUrl,
                    Occupancy = cottageDTO.Occupancy,
                    SqMeters = cottageDTO.SqMeters,
                    Rate = cottageDTO.Rate,
                    CreatedDate = DateTime.Now
                };
                */
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
        [ProducesResponseType(typeof(ApiResponse<CottageDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CottageDTO>>> UpdateCottage(int id, CottageUpdateDTO cottageDTO)
        {
            try
            {
                // ID Validation
                if (cottageDTO == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Cottage data is required"));

                    //return BadRequest("Cottage data is required");
                }
                // Check if the ID in the URL matches the ID in the request body
                if (id != cottageDTO.Id)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Cottage ID in URL does not match Cottage ID in request body!!!"));

                    //return BadRequest("Cottage ID in URL does not match Cottage ID in request body!!!");

                }
                // Get the existing cottage from the database
                var existingCottage = await _db.Cottage.FirstOrDefaultAsync(v => v.Id == id);

                // Check if the existing cottage is null
                if (existingCottage == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"Cottage with ID {id} was not found!"));

                    //return NotFound($"Cottage with ID {id} was not found!!!");
                }

                // Check for duplicate cottage name (case-insensitive) excluding the current cottage being updated
                var duplicateCottage = await _db.Cottage.FirstOrDefaultAsync(v => v.Name.ToLower() == cottageDTO.Name.ToLower() && v.Id != id);

                // Check if a duplicate cottage exists
                if (duplicateCottage != null)
                {
                    return Conflict(ApiResponse<object>.Conflict($"A Cottage with the name '{cottageDTO.Name}' already exists!!!"));

                    //return Conflict($"A Cottage with hte name '{cottageDTO.Name}' already exists!!!");

                }

                // Map the updated properties from the DTO to the existing cottage entity
                _mapper.Map(cottageDTO, existingCottage);

                // Update the UpdatedDate property to the current date and time
                existingCottage.UpdatedDate = DateTime.UtcNow;

                // Save the changes to the database
                await _db.SaveChangesAsync();

                // Return the updated cottage with a 200 status code and a success message
                var response = ApiResponse<CottageDTO>.Ok(_mapper.Map<CottageDTO>(cottageDTO),"Cottage updated successfully");

                // Return the updated cottage with a 200 status code
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(500, "An error occurred while updating cottage", ex.Message);
                return StatusCode(500, errorResponse);
                //return StatusCode(StatusCodes.Status500InternalServerError,
                    //$"An error occurred while updating cottage!: {ex.Message}");
            }
        }

        // Endpoint to update Cottage 
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteCottage(int id)
        {
            try
            {
                // ID Validation
                var existingCottage = await _db.Cottage.FirstOrDefaultAsync(v => v.Id == id);

                // Check if the existing cottage is null
                if (existingCottage == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"Cottage with ID {id} was not found!"));

                    //return NotFound($"Cottage with ID {id} was not found!!!");
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
                //return StatusCode(StatusCodes.Status500InternalServerError,
                    //$"An error occurred while deleting the cottage!: {ex.Message}");
            }
        }

        // Example of route parameter binding with different sources (commented out)
        /*
        // Route parameter binding - string is default so it does not need to be declared
        [HttpGet("{id:int}/{name}")]
        public string GetCottageByIdAndName(int id, string name)
        {
            return "Get Cottage: " + id + " " +name;
        }

        // Route parameter binding - string is default 
        [HttpGet("{id:int}/{name}")]
        public string GetCottageByIdAndNameFromRoute([FromRoute] int id, [FromRoute] string name)
        {
            return "Get Cottage: " + id + " " + name;
        }        

        // Route parameter binding - string is default 
        [HttpGet("{id:int}/{name}")]
        public string GetCottageByIdAndNameFromQuery([FromQuery] int id, [FromQuery] string name)
        {
            return "Get Cottage: " + id + " " + name;
        }

        // Route parameter binding - string is default 
        [HttpGet("{id:int}/{name}")]
        public string GetCottageByIdAndNameFromHeader([FromHeader] int id, [FromHeader] string name)
        {
            return "Get Cottage: " + id + " " + name;
        }
        */
    }
}
