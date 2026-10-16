namespace RavingoTest.Models
{
    public class StockItem
    {
        public int Id { get; set; }

        public int BranchId { get; set; }
        public Branch Branch { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }
    }
}
