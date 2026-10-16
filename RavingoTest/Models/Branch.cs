namespace RavingoTest.Models
{
    public class Branch
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigation property: Branch එක සතු Stock Items ලැයිස්තුව
        public ICollection<StockItem> StockItems { get; set; } = new List<StockItem>();
    }
}
