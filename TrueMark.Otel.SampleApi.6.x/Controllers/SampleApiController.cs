using Microsoft.AspNetCore.Mvc;
using TrueMark.Otel.SampleApi._6.x.Metrics;

namespace TrueMark.Otel.SampleApi._6.x.Controllers;

[ApiController]
[Route("[controller]")]
public class SampleApiController : ControllerBase
{
    private readonly ILogger<SampleApiController> logger;
    private readonly SampleApiOtelService sampleApiOtelService;

    public SampleApiController(ILogger<SampleApiController> logger, SampleApiOtelService sampleApiOtelService)
    {
        this.logger = logger;
        this.sampleApiOtelService = sampleApiOtelService;
    }

    
    [HttpGet("test")]
    public IActionResult Test()
    {
        logger.LogInformation("Test endpoint called");
        sampleApiOtelService.LogTestProcessedRequest();
        if (new Random().Next(2) == 0)
        {
            sampleApiOtelService.LogTestSuccessfulRequest();
            return Ok("It Works!");
        }
        else
        {
            sampleApiOtelService.LogTestFailedRequest();
            return Ok("It doesn't work!");
        }
    }
}