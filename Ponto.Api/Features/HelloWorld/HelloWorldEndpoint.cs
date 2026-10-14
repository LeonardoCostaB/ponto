namespace Ponto.Api.Features.HelloWorld;

[ApiController]
public class HelloWorldController : ControllerBase
{
    [HttpGet("/")]
    public IActionResult Get()
    {
        return Ok("Hello World");
    }
}