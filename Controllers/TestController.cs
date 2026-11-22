using Microsoft.AspNetCore.Mvc;

namespace DemoAPINET.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [HttpGet("error")]
        public IActionResult ThrowError()
        {
            _logger.LogError("Lỗi thử nghiệm: Chuẩn bị ném lỗi thử nghiệm từ TestController.");
            throw new Exception("Lỗi thử nghiệm");
        }
    }
}
