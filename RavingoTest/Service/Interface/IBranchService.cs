using RavingoTest.Dto;

namespace RavingoTest.Service.Interface
{
    public interface IBranchService
    {
        Task<IEnumerable<BranchStockDto>> GetStockByBranchIdAsync(int branchId);
    }
}
