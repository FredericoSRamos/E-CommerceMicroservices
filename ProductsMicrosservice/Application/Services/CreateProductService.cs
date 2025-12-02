using Domain.Entities;
using Infrastructure.Contracts;

namespace Application.Services;

public class CreateProductService
{
    private readonly IRepository<Product> _productsRepository;

    public CreateProductService(IRepository<Product> productsRepository)
    {
        _productsRepository = productsRepository;
    }
    
    public async Task<Product> CreateProduct(Product productData)
    {
        var product = await _productsRepository.Create(productData);
        
        return product;
    }
}