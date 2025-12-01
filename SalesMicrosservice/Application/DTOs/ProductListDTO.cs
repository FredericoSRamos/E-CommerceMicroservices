namespace Application.DTOs;

public class ProductListDTO
{
    public int CustomerId { get; set; }
    public IList<ProductDTO> Products { get; set; }
}