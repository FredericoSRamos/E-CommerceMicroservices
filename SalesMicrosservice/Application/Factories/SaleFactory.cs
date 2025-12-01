using Application.DTOs;
using Domain.Entities;

namespace Application.Factories;

public class SaleFactory
{
    public Sale CreateSale(SaleCreateDTO saleData)
    {
        var sale = new Sale
        {

            Date = DateTime.Now,
            CustomerId = saleData.CustomerId,
            Price = saleData.Price,
            SaleProducts = new List<SaleProduct>()
        };
            
        foreach (var product in saleData.SaleProducts)
        {
            sale.SaleProducts.Add(new SaleProduct
            {
                ProductId = product.Id,
                Amount = product.Amount
            });
        }

        return sale;
    }
}