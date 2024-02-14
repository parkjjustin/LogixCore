using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogixCore.Server.Features.Test;

[Authorize]
[Route("api")]
public class TestingController : ControllerBase
{
    [HttpGet("testing")]
    [ValidateAntiForgeryToken]
    public ActionResult Testing()
    {
        return this.Ok(true);
    }

    [HttpPost("testing")]
    [ValidateAntiForgeryToken]
    public ActionResult Testing([FromBody] Test test)
    {
        return this.Ok(test.test);
    }

    public record Test(string test);
}
