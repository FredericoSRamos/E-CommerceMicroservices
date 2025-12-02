using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Application.DTOs;
using Domain.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("[controller]")]
[ApiController]
public class StockController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IMessageBus _bus;

    public StockController(IHttpClientFactory httpClientFactory, IMessageBus bus)
    {
        _httpClient = httpClientFactory.CreateClient("StockAPI");
        _bus = bus;
    }
    
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var response = await _httpClient.GetAsync(_httpClient.BaseAddress);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "An error occurred!");
            }

            var products = await response.Content.ReadAsStringAsync();

            return Ok(products);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "An error occurred"!);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync(_httpClient.BaseAddress + id.ToString());

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "An error occurred!");
            }

            var product = await response.Content.ReadAsStringAsync();

            return Ok(product);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "An error occurred"!);
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ProductDTO productData)
    {
        try
        {
            using var reader = new StreamReader(Request.Body);
            var bodyString = await reader.ReadToEndAsync();
            
            var product = JsonSerializer.Deserialize<ProductDTO>(bodyString);
            
            await _bus.PublishAsync("create_product_queue", product);
            
            return Accepted("Product sent to processing");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "An error occurred"!);
        }
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] ProductDTO productData)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (User.FindFirstValue(ClaimTypes.Role) != "Admin" && userId != id.ToString())
        {
            return Forbid();
        }
        
        try
        {
            var requestBody = Request.Body;
            
            var content = new StringContent(
                requestBody.ToString(),
                Encoding.UTF8,
                Request.ContentType ?? "application/json"
            );
            
            var response = await _httpClient.PutAsync(_httpClient.BaseAddress + id.ToString(), content);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "An error occurred!");
            }

            var updatedProduct = await response.Content.ReadAsStringAsync();

            return Ok(updatedProduct);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "An error occurred"!);
        }
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (User.FindFirstValue(ClaimTypes.Role) != "Admin" && userId != id.ToString())
        {
            return Forbid();
        }
        
        try
        {
            var response = await _httpClient.DeleteAsync(_httpClient.BaseAddress + id.ToString());

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "An error occurred!");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "An error occurred"!);
        }
    }
}