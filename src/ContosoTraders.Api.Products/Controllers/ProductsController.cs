using System.Diagnostics;
using Microsoft.ApplicationInsights;

namespace ContosoTraders.Api.Products.Controllers;

[Route("v1/[controller]")]
[Produces("application/json")]
public class ProductsController : ContosoTradersControllerBase
{
    protected TelemetryClient telemetryClient;
    public ProductsController(IMediator mediator, TelemetryClient telemetryClient) : base(mediator)
    {
        this.telemetryClient = telemetryClient;
    }


    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts(
        [FromQuery(Name = "brand")] int[] brands,
        [FromQuery(Name = "type")] string[] types)
    {
        telemetryClient.TrackTrace("Obtaining All Products");

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        var request = new GetProductsRequest
        {
            Brands = brands,
            Types = types
        };
        stopwatch.Stop();

        telemetryClient.TrackMetric("Obtain Products", stopwatch.ElapsedMilliseconds);

        return await ProcessHttpRequestAsync(request);
    }


    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProduct(int id)
    {
        telemetryClient.TrackTrace($"Get Details for Product: {id}");
        var request = new GetProductRequest
        {
            ProductId = id
        };

        return await ProcessHttpRequestAsync(request);
    }


    [HttpGet("landing")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPopularProducts()
    {
        var request = new GetPopularProductsRequest();

        return await ProcessHttpRequestAsync(request);
    }


    [HttpPost("imageclassifier")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostImage(IFormFile file)
    {
        var request = new PostImageRequest
        {
            File = file
        };

        return await ProcessHttpRequestAsync(request);
    }

    [HttpGet("search/{text}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Search(string text)
    {
        telemetryClient.TrackTrace($"Searching for Product: {text}");

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        var request = new SearchTextRequest
        {
            Text = text
        };

        stopwatch.Stop();
        telemetryClient.TrackMetric("Search Products", stopwatch.ElapsedMilliseconds);

        return await ProcessHttpRequestAsync(request);
    }
}