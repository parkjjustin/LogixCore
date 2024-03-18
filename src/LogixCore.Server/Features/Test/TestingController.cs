using Duende.Bff;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogixCore.Server.Features.Test;

[Authorize]
[BffApi]
[Route("api")]
public class TestingController : ControllerBase
{
    [HttpGet("testing")]
    public ActionResult Testing()
    {
        return this.Ok(true);
    }

    [HttpPost("testing")]
    public ActionResult Testing([FromBody] Test test)
    {
        return this.Ok(test.test);
    }

    public record Test(string test);
}
