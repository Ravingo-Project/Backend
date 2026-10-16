using RavingoTest.Dto;

namespace RavingoTest.Service.Interface
{
    public interface ITransferService
    {
        Task<TransferResponseDto> CreateTransferAsync(CreateTransferDto dto);
        Task<IEnumerable<TransferResponseDto>> GetTransferHistoryAsync();
    }
}
