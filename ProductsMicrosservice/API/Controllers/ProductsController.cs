using Application.Services;
using Domain.Entities;
using Infrastructure.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IRepository<Product> _productsRepository;

    public ProductsController(IRepository<Product> productsRepository)
    {
        _productsRepository = productsRepository;
    }
    
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var products = await _productsRepository.GetAll();
        
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var product = await _productsRepository.Read(id);

        if (product == null)
        {
            return NotFound("Product not found!");
        }
        
        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Product productData)
    {
        try
        {
            var product = await new CreateProductService(_productsRepository).CreateProduct(productData);

            return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "An error occured while trying to create the product!");
        }
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> DecreaseStock(int id, [FromBody] int amount)
    {
        var product = await _productsRepository.Read(id);
        if (product == null)
        {
            return NotFound("Product not found!");
        }

        product.Amount -= amount;
        try
        {
            await _productsRepository.Update(product);
            
            return Ok(product);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "An error occured while trying to update the product!");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Product productData)
    {
        var product = await _productsRepository.Read(id);

        if (product == null)
        {
            return NotFound("Product does not exist!");
        }
        
        productData.Id = product.Id;

        try
        {
            await _productsRepository.Update(productData);

            return Ok(productData);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "An error occured while trying to update the product!");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _productsRepository.Delete(id);

            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "An error occured while trying to delete the product!");
        }
    }
}