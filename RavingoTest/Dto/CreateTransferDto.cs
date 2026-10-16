namespace RavingoTest.Dto
{
    public class CreateTransferDto
    {
        public int FromBranchId { get; set; }
        public int ToBranchId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
