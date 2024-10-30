using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeIDB2cMultiTenantApi.Controllers;

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
