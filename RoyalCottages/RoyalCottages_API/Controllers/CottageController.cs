using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoyalCottages_API.Data;
using RoyalCottages_API.Models;
using RoyalCottages_API.Models.DTO;
using System.Collections;

namespace RoyalCottages_API.Controllers
{
    
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
        public async Task<ActionResult<IEnumerable<Cottage>>> GetCottages()
        {
            return Ok(await _db.Cottage.ToListAsync());
        }

        // Endpoint to get  Cottage by Id
        // Route parameter binding
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Cottage>> GetCottageById(int id)
        {
            try
            {
                // ID Validation
                if(id <= 0)
                {
                    return BadRequest("Cottage ID must be greater than 0");
                }
                // Get cottage by ID using linq query operation
                var cottage = await _db.Cottage.FirstOrDefaultAsync(x => x.Id == id);

                // Check if cottage is populated
                if (cottage == null)
                {
                    return NotFound($"Cottage with ID {id} was not found!");
                }
                return Ok(cottage);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occurred while retrieving cottage with ID {id}: {ex.Message}");
            }
        }

        // Endpoint to create Cottage
        [HttpPost]
        public async Task<ActionResult<Cottage>> CreateCottage(CottageCreateDTO cottageDTO)
        {
            try
            {
                // ID Validation
                if (cottageDTO == null)
                {
                    return BadRequest("Cottage data is required");
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

                // Return the created cottage with a 201 status code and the location of the new resource
                return CreatedAtAction(nameof(GetCottageById), new { id = cottage.Id }, cottage);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occurred while creating cottage!: {ex.Message}");
            }
        }

        // Endpoint to update Cottage - ActionResult - To return a response to the client
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Cottage>> UpdateCottage(int id, CottageUpdateDTO cottageDTO)
        {
            try
            {
                // ID Validation
                if (cottageDTO == null)
                {
                    return BadRequest("Cottage data is required");
                }
                // Check if the ID in the URL matches the ID in the request body
                if (id != cottageDTO.Id)
                {
                    return BadRequest("Cottage ID in URL does not match Cottage ID in request body!!!");

                }
                // Get the existing cottage from the database
                var existingCottage = await _db.Cottage.FirstOrDefaultAsync(v => v.Id == id);

                // Check if the existing cottage is null
                if (existingCottage == null)
                {
                    return NotFound($"Cottage with ID {id} was not found!!!");
                }

                // Map the updated properties from the DTO to the existing cottage entity
                _mapper.Map(cottageDTO, existingCottage);

                // Update the UpdatedDate property to the current date and time
                existingCottage.UpdatedDate = DateTime.UtcNow;

                // Save the changes to the database
                await _db.SaveChangesAsync();

                // Return the updated cottage with a 200 status code
                return Ok(cottageDTO);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occurred while updating cottage!: {ex.Message}");
            }
        }

        // Endpoint to update Cottage 
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Cottage>> DeleteCottage(int id)
        {
            try
            {
                // ID Validation
                var existingCottage = await _db.Cottage.FirstOrDefaultAsync(v => v.Id == id);
                // Check if the existing cottage is null
                if (existingCottage == null)
                {
                    return NotFound($"Cottage with ID {id} was not found!!!");
                }

                // Remove the existing cottage from the database
                _db.Cottage.Remove(existingCottage);

                await _db.SaveChangesAsync();
                // Return a 204 No Content response to indicate successful deletion
                return NoContent();
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occurred while deleting the cottage!: {ex.Message}");
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
