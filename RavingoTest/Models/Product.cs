namespace RavingoTest.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;

        // Navigation property: Product එක ඇති Stock Items ලැයිස්තුව
        public ICollection<StockItem> StockItems { get; set; } = new List<StockItem>();
    }
}
