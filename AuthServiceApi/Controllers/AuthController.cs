using Microsoft.AspNetCore.Mvc;

namespace AuthServiceApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly List<string> _validKeys;

        public AuthController(IConfiguration config)
        {
            _validKeys = config.GetSection("ApiKeys").Get<List<string>>() ?? new();
        }

        [HttpPost("validate")]
        public IActionResult Validate([FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
                return Unauthorized(new { isValid = false, message = "API Key is missing." });

            if (!_validKeys.Contains(apiKey))
                return Forbid("Invalid API Key.");

            return Ok(new { isValid = true });
        }
    }
}