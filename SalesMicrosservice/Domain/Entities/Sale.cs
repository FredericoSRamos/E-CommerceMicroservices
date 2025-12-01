namespace Domain.Entities;

public class Sale
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime Date { get; set; }
    public decimal Price { get; set; }
    public IList<SaleProduct> SaleProducts { get; set; }
}