using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Zepcom.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        [HttpGet]
        public Task<IActionResult> GetValue()
        {
            var value = "API WORKING";
            return Task.FromResult<IActionResult>(Ok(value));
        }
    }
}
