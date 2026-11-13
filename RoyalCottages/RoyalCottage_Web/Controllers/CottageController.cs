using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RoyalCottage_DTO;
using RoyalCottage_Web.Models;
using RoyalCottage_Web.Services.IServices;
using System.Diagnostics;

namespace RoyalCottage_Web.Controllers
{
    public class CottageController : Controller
    {
        // Inject Dependencies 
        private readonly ICottageService _cottageService;

        // Inject Dependencies
        private readonly IMapper _mapper;

        // Constructor to initialize dependencies
        public CottageController(ICottageService cottageService, IMapper mapper)
        {
            _cottageService = cottageService;
            _mapper = mapper;
        }

        // Action method to handle the Index page request
        public async Task<IActionResult> Index()
        {
            // Initialize an empty list to hold CottageDTO objects
            List<CottageDTO> cottageList = new();
            try
            {
                // Call the service to get all cottages and handle the response
                var response = await _cottageService.GetAllAsync<ApiResponse<List<CottageDTO>>>();

                // Check if the response is successful and contains data
                if (response != null && response.Success && response.Data != null)
                {
                    cottageList = response.Data; // Assign the retrieved data to the cottageList
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the API call and log the error message
                TempData["error"] = $"An error occurred: {ex.Message}";
            }
            // Return the view with the list of cottages
            return View(cottageList);
        }

        // Action method to display the Create Cottage form
        [Authorize(Roles = "Admin")] 
        public IActionResult Create()
        {
            return View();
        }

        // Action method to handle the form submission for creating a new cottage
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CottageCreateDTO createDTO)
        {
            // Check if the model state is valid before proceeding with the creation
            if (!ModelState.IsValid)
            {
                return View(createDTO);
            }

            try
            {
                // Call the service to create a new cottage and handle the response
                var response = await _cottageService.CreateAsync<ApiResponse<CottageDTO>>(createDTO);

                // Check if the response is successful and contains data
                if (response != null && response.Success && response.Data != null)
                {
                    TempData["success"] = "Cottage created successfully";
                    return RedirectToAction(nameof(Index)); // Redirect to the Index action after successful creation
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the API call and log the error message
                TempData["error"] = $"An error occurred: {ex.Message}";
            }

            return View(createDTO);
        }

        // Action method to display the Delete Cottage confirmation page
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            // Check if the provided ID is valid before proceeding with the deletion
            if (id <= 0)
            {
                TempData["error"] = "Invalid cottage ID";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                // Call the service to get the specific cottage details and handle the response
                var response = await _cottageService.GetAsync<ApiResponse<CottageDTO>>(id);

                // Check if the response is successful and contains data
                if (response != null && response.Success && response.Data != null)
                {
                    return View(response.Data); // Return the view with the cottage details for confirmation
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the API call and log the error message
                TempData["error"] = $"An error occurred: {ex.Message}";
            }

            return View();
        }

        // Action method to handle the form submission for creating a new cottage
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(CottageDTO cottageDTO)
        {
            try
            {
                // Call the service to delete the specific cottage and handle the response
                var response = await _cottageService.DeleteAsync<ApiResponse<object>>(cottageDTO.Id);

                // Check if the response is successful and contains data
                if (response != null && response.Success && response.Data != null)
                {
                    // NOT WORKING - NEED FIXING
                    TempData["success"] = "Cottage deleted successfully";
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the API call and log the error message
                TempData["error"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // Action method to display the Edit Cottage form
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            // Check if the provided ID is valid before proceeding with the edit
            if (id <= 0)
            {
                TempData["error"] = "Invalid cottage ID";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                // Call the service to get the specific cottage details and handle the response
                var response = await _cottageService.GetAsync<ApiResponse<CottageDTO>>(id);

                // Check if the response is successful and contains data
                if (response != null && response.Success && response.Data != null)
                {
                    // Map the retrieved CottageDTO to CottageUpdateDTO using AutoMapper and return the view for editing
                    return View(_mapper.Map<CottageUpdateDTO>(response.Data));
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the API call and log the error message
                TempData["error"] = $"An error occurred: {ex.Message}";
            }

            return View();
        }

        // Action method to handle the form submission for updating an existing cottage
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CottageUpdateDTO updateDTO)
        {
            try
            {
                // Call the service to update the specific cottage and handle the response
                var response = await _cottageService.UpdateAsync<ApiResponse<object>>(updateDTO);

                // Check if the response is successful and contains data
                if (response != null && response.Success && response.Data != null)
                {
                    TempData["success"] = "Cottage updated successfully";
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the API call and log the error message
                TempData["error"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // Action method to display the Privacy page
        public IActionResult Privacy()
        {
            return View();
        }

        // Action method to handle errors and display the Error page
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
