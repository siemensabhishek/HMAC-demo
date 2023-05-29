using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using softaware.Authentication.Hmac;

namespace YourWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [Authorize(AuthenticationSchemes = HmacAuthenticationDefaults.AuthenticationScheme)]
        [HttpGet("hello")]
        public IActionResult Get()
        {
            return Ok("Hello, World!");
        }
    }

}
