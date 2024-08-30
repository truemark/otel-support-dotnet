using Microsoft.AspNetCore.Mvc;
using TrueMark.Otel.SampleApi._6.x.Metrics;

namespace TrueMark.Otel.SampleApi._6.x.Controllers;

[ApiController]
[Route("[controller]")]
public class SampleApi6Controller : ControllerBase
{
    private readonly ILogger<SampleApi6Controller> logger;
    private readonly SampleApi6OtelService _sampleApi6OtelService;

    public SampleApi6Controller(ILogger<SampleApi6Controller> logger, SampleApi6OtelService sampleApi6OtelService)
    {
        this.logger = logger;
        this._sampleApi6OtelService = sampleApi6OtelService;
    }

    
    [HttpGet("test")]
    public IActionResult Test()
    {
        logger.LogInformation("Test endpoint called");
        _sampleApi6OtelService.LogTestProcessedRequest();
        if (new Random().Next(2) == 0)
        {
            _sampleApi6OtelService.LogTestSuccessfulRequest();
            return Ok("It Works!");
        }
        else
        {
            _sampleApi6OtelService.LogTestFailedRequest();
            return Ok("It doesn't work!");
        }
    }
}