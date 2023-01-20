using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class BusinessController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new List<string> { "data1 from t1", "data2" });
    }
}
