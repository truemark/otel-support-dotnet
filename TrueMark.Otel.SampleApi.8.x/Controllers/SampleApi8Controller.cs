using Microsoft.AspNetCore.Mvc;
using TrueMark.Otel.SampleApi._6.x.Metrics;

namespace TrueMark.Otel.SampleApi._6.x.Controllers;

[ApiController]
[Route("[controller]")]
public class SampleApi8Controller : ControllerBase
{
    private readonly ILogger<SampleApi8Controller> logger;
    private readonly SampleApi8OtelService _sampleApi8OtelService;

    public SampleApi8Controller(ILogger<SampleApi8Controller> logger, SampleApi8OtelService sampleApi8OtelService)
    {
        this.logger = logger;
        this._sampleApi8OtelService = sampleApi8OtelService;
    }

    
    [HttpGet("test")]
    public IActionResult Test()
    {
        logger.LogInformation("Test endpoint called");
        _sampleApi8OtelService.LogTestProcessedRequest();
        if (new Random().Next(2) == 0)
        {
            _sampleApi8OtelService.LogTestSuccessfulRequest();
            return Ok("It Works!");
        }
        else
        {
            _sampleApi8OtelService.LogTestFailedRequest();
            return Ok("It doesn't work!");
        }
    }
}