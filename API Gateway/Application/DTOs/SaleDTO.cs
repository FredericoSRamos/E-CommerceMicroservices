namespace Application.DTOs;

public class SaleDTO
{
    public int CustomerId { get; set; }
    public decimal Price { get; set; }
    public IList<(int, int)> SaleProducts { get; set; }
}