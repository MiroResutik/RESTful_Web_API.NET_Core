using AutoMapper;
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
                var response = await _cottageService.GetAllAsync<ApiResponse<List<CottageDTO>>>("");

                // Check if the response is successful and contains data
                if (response !=null && response.Success && response.Data !=null )
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
