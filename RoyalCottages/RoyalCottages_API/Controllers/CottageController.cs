using Microsoft.AspNetCore.Mvc;

namespace RoyalCottages_API.Controllers
{
    
    [Route("api/cottage")]
    [ApiController]
    public class CottageController : ControllerBase
    {
        // Endpoint to get all Cottages
        [HttpGet]
        public string GetCottages()
        {
            return "List of all Cottages";
        }

        // Endpoint to get  Cottage by Id
        // Route parameter binding
        [HttpGet("{id:int}")]
        public string GetCottageById(int id)
        {
            return "Get Cottage: " +id;
        }

        //// Route parameter binding - string is default so it does not need to be declared
        //[HttpGet("{id:int}/{name}")]
        //public string GetCottageByIdAndName(int id, string name)
        //{
        //    return "Get Cottage: " + id + " " +name;
        //}

        //// Route parameter binding - string is default 
        //[HttpGet("{id:int}/{name}")]
        //public string GetCottageByIdAndNameFromRoute([FromRoute] int id, [FromRoute] string name)
        //{
        //    return "Get Cottage: " + id + " " + name;
        //}        
        
        //// Route parameter binding - string is default 
        //[HttpGet("{id:int}/{name}")]
        //public string GetCottageByIdAndNameFromQuery([FromQuery] int id, [FromQuery] string name)
        //{
        //    return "Get Cottage: " + id + " " + name;
        //}

        //// Route parameter binding - string is default 
        //[HttpGet("{id:int}/{name}")]
        //public string GetCottageByIdAndNameFromHeader([FromHeader] int id, [FromHeader] string name)
        //{
        //    return "Get Cottage: " + id + " " + name;
        //}
    }
}
