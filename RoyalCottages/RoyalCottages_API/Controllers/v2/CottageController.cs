using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoyalCottage_DTO;
using RoyalCottages_API.Data;
using RoyalCottages_API.Models;
using RoyalCottages_API.Services.IServices;
using System.Collections;

namespace RoyalCottages_API.Controllers.v2
{
    // Route attribute to specify the base route for the controller
    [Route("api/v{version:apiVersion}/cottage")]
    [ApiVersion("2.0")]
    [ApiController]
    //[Authorize] // Authorize attribute to restrict access to authenticated users
    [AllowAnonymous] // Allow anyone to access - only for development phase
    public class CottageController : ControllerBase
    {
        // Access the Database - access entity framework core
        private readonly ApplicationDbContext _db;

        // Access the Image Service - access the image service for image upload and deletion
        private readonly IImageService _imageService;

        // Access the Mapper - access AutoMapper
        private readonly IMapper _mapper;

        // Constructor for Database - dependency injection
        public CottageController(ApplicationDbContext db, IMapper mapper, IImageService imageService)
        {
            _db = db;
            _mapper = mapper;
            _imageService = imageService;
        }
        // Endpoint to get all Cottages with filtering, sorting, and pagination
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CottageDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<CottageDTO>>>> GetCottages(
            [FromQuery] string? filterBy,
            [FromQuery] string? filterQuery, // Accept filterBy and filterQuery as query parameters for filtering the cottages
            [FromQuery] string? sortBy, // Accept sortBy as a query parameter for sorting the cottages
            [FromQuery] string? sortOrder = "asc",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20) // Accept page and pageSize as query parameters for pagination
        {
            if (page < 1) page = 1; // Ensure that the page number is at least 1
            if (pageSize < 1) pageSize = 20; // Ensure that the page size is at least 1
            if (pageSize > 100) pageSize = 100; // Ensure that the page size does not exceed 100

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

            // Apply sorting based on the sortBy and sortOrder parameters if they are provided
            if (!string.IsNullOrEmpty(sortBy))
            {
                // Determine if the sort order is descending based on the sortOrder parameter
                var isDescending = sortOrder?.ToLower() == "desc";

                // Apply sorting based on the sortBy parameter using a switch expression
                cottagesQuery = sortBy.ToLower() switch
                {
                    "name" => isDescending ? cottagesQuery.OrderByDescending(u => u.Name)
                    : cottagesQuery.OrderBy(u => u.Name),
                    "rate" => isDescending ? cottagesQuery.OrderByDescending(u => u.Rate)
                    : cottagesQuery.OrderBy(u => u.Rate),
                    "occupancy" => isDescending ? cottagesQuery.OrderByDescending(u => u.Occupancy)
                    : cottagesQuery.OrderBy(u => u.Occupancy),
                    "sqmeters" => isDescending ? cottagesQuery.OrderByDescending(u => u.SqMeters)
                    : cottagesQuery.OrderBy(u => u.SqMeters),
                    "id" => isDescending ? cottagesQuery.OrderByDescending(u => u.Id)
                    : cottagesQuery.OrderBy(u => u.Id),
                    _ => cottagesQuery.OrderBy(u => u.Id)
                };
            }
            else
            {
                cottagesQuery = cottagesQuery.OrderBy(u => u.Id);
            }

            // Calculate the number of records to skip based on the page number and page size for pagination
            var skip = (page - 1) * pageSize;

            // Calculate the total number of records in the cottagesQuery for pagination metadata
            var totalCount = await cottagesQuery.CountAsync();

            // Calculate the total number of pages based on the total count and page size
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            // Retrieve the cottages from the database with pagination applied using Skip and Take methods    
            var cottages = await cottagesQuery.Skip(skip).Take(pageSize).ToListAsync();

            // Map the cottages to CottageDTO objects and return them in an ApiResponse object with a 200 status code and a success message
            var dtoResponseCottage = _mapper.Map<List<CottageDTO>>(cottages);

            // Build a message string to include in the ApiResponse object with pagination and filtering information
            var messageBuilder = new System.Text.StringBuilder();

            // Append the number of cottages retrieved, the current page, total pages, and total records to the message
            messageBuilder.Append($"Successfully retrieved {dtoResponseCottage.Count} cottage(s)");
            messageBuilder.Append($"(Page {page} of {totalPages}, {totalCount} total records");

            // Append filtering information to the message if filterBy and filterQuery are provided
            if (!string.IsNullOrEmpty(filterQuery) && (!string.IsNullOrEmpty(filterBy)))
            {
                messageBuilder.Append($"(filtered by {filterBy}: '{filterQuery}'");
            }

            // Append sorting information to the message if sortBy is provided
            if (!string.IsNullOrEmpty(sortBy))
            {
                messageBuilder.Append($"(sorted by {sortBy}: '{sortOrder?.ToLower() ?? "asc"}'");
            }

            // Append closing parentheses to the message
            Response.Headers.Append("Pagination_CurrentPage", page.ToString());
            Response.Headers.Append("Pagination_PageSize", pageSize.ToString());
            Response.Headers.Append("Pagination_TotalCount", totalCount.ToString());
            Response.Headers.Append("Pagination_TotalPages", totalPages.ToString());

            // Return the cottages with a 200 status code and a success message
            var response = ApiResponse<IEnumerable<CottageDTO>>.Ok(dtoResponseCottage, messageBuilder.ToString());
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
        [Consumes("multipart/form-data")] // Specify that the endpoint consumes multipart/form-data for file uploads
        //[Authorize(Roles = "Admin")] // Authorize attribute to restrict access to users with the "Customer" or "Admin" roles
        [ProducesResponseType(typeof(ApiResponse<CottageDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<CottageDTO>>> CreateCottage([FromForm]CottageCreateDTO cottageDTO)
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

                // Validate the image file if it is provided in the request
                if (cottageDTO.Image != null)
                {
                    // Validate the image file using the _imageService.ValidateImage method
                    if (!_imageService.ValidateImage(cottageDTO.Image))
                    {
                        return BadRequest(ApiResponse<object>.BadRequest("Invalid image file. Allowed formats: jpg, jpeg, png. Max size 5MB"));
                    }

                    // Upload the image file using the _imageService.UploadImageAsync method and set the ImageUrl property of the cottage object
                    cottage.ImageUrl = await _imageService.UploadImageAsync(cottageDTO.Image);
                }

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
        [Consumes("multipart/form-data")] // Specify that the endpoint consumes multipart/form-data for file uploads
        //[Authorize(Roles = "Admin")] // Authorize attribute to restrict access to users with the "Customer" or "Admin" roles
        [ProducesResponseType(typeof(ApiResponse<CottageDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CottageDTO>>> UpdateCottage(int id,[FromForm] CottageUpdateDTO cottageDTO)
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

                // Validate the image file if it is provided in the request
                if (cottageDTO.Image != null && !_imageService.ValidateImage(cottageDTO.Image))
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Invalid image file. Allowed formats: jpg, jpeg, png. Max size 5MB"));
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

                // Store the old image URL before updating the cottage
                var oldImageUrl = existingCottage.ImageUrl;

                // Map the updated properties from the DTO to the existing cottage entity
                _mapper.Map(cottageDTO, existingCottage);

                // Update the UpdatedDate property to the current date and time
                existingCottage.UpdatedDate = DateTime.UtcNow;

                // If a new image is provided, upload it and update the ImageUrl property
                if (cottageDTO.Image!= null)
                {
                    // Upload the new image and update the ImageUrl property of the existing cottage
                    existingCottage.ImageUrl = await _imageService.UploadImageAsync(cottageDTO.Image);

                    // Update the ImageUrl property of the DTO to reflect the new image URL
                    cottageDTO.ImageUrl = existingCottage.ImageUrl;

                    // Delete the old image if it exists and is different from the new image
                    if (!string.IsNullOrEmpty(oldImageUrl) && oldImageUrl!= existingCottage.ImageUrl)
                    {
                        await _imageService.DeleteImageAsync(oldImageUrl); // Delete the old image from the image service
                    }
                }

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

                // Delete the image associated with the cottage if it exists
                if (!string.IsNullOrEmpty(existingCottage.ImageUrl))
                {
                    await _imageService.DeleteImageAsync(existingCottage.ImageUrl);
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
