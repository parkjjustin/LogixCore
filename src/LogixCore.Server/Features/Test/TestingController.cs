using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogixCore.Server.Features.Test;

[Authorize]
[Route("api")]
public class TestingController : ControllerBase
{
    [HttpGet("testing")]
    public ActionResult Testing()
    {
        return this.Ok(true);
    }
}
