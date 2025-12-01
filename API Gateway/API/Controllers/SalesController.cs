using System.Security.Claims;
using System.Text;
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class SalesController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public SalesController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("SalesAPI");
    }
    
    [Authorize(Roles = "Admin")]
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

            var sales = await response.Content.ReadAsStringAsync();

            return Ok(sales);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "An error occurred"!);
        }
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (User.FindFirstValue(ClaimTypes.Role) != "Admin" && userId != id.ToString())
        {
            return Forbid();
        }
        
        try
        {
            var response = await _httpClient.GetAsync(_httpClient.BaseAddress + id.ToString());

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "An error occurred!");
            }

            var sale = await response.Content.ReadAsStringAsync();

            return Ok(sale);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "An error occurred"!);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] SaleDTO saleData)
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

            var createdSale = await response.Content.ReadAsStringAsync();

            return Ok(createdSale);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "An error occurred"!);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] SaleDTO saleData)
    {
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

            var updatedSale = await response.Content.ReadAsStringAsync();

            return Ok(updatedSale);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "An error occurred"!);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
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

    [HttpPost("Purchase")]
    public async Task<IActionResult> Purchase([FromBody] ProductListDTO productList)
    {
        productList.CustomerId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        
        try
        {
            var requestBody = Request.Body;

            var content = new StringContent(
                requestBody.ToString(),
                Encoding.UTF8,
                Request.ContentType ?? "application/json"
            );

            var response = await _httpClient.PostAsync(_httpClient.BaseAddress + "Purchase", content);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "An error occurred!");
            }

            var sale = await response.Content.ReadAsStringAsync();

            return Ok(sale);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "An error occurred!");
        }
    }
}