namespace RavingoTest.Dto
{
    public class TransferResponseDto
    {
        public int Id { get; set; }
        public int FromBranchId { get; set; }
        public string FromBranchName { get; set; } = string.Empty;
        public int ToBranchId { get; set; }
        public string ToBranchName { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
