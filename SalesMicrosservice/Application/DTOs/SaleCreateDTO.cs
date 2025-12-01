namespace Application.DTOs;

public class SaleCreateDTO
{
    public int CustomerId { get; set; }
    public decimal Price { get; set; }
    public IList<ProductDTO> SaleProducts { get; set; }
}