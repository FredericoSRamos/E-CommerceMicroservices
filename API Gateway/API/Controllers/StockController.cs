using System.Security.Claims;
using System.Text;
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("[controller]")]
[ApiController]
public class StockController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public StockController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("StockAPI");
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
            var requestBody = Request.Body;
            
            var content = new StringContent(
                requestBody.ToString(),
                Encoding.UTF8,
                Request.ContentType ?? "application/json"
            );
            
            var response = await _httpClient.PostAsync(_httpClient.BaseAddress, content);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "An error occurred!");
            }

            var createdProduct = await response.Content.ReadAsStringAsync();

            return Ok(createdProduct);
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