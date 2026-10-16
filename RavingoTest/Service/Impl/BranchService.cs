using Microsoft.EntityFrameworkCore;
using RavingoTest.Data;
using RavingoTest.Dto;
using RavingoTest.Service.Interface;

namespace RavingoTest.Service.Impl
{
    public class BranchService: IBranchService
    {
        private readonly AppDbContext _context;

        public BranchService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BranchStockDto>> GetStockByBranchIdAsync(int branchId)
        {
            var branchExists = await _context.Branches.AnyAsync(b => b.Id == branchId);
            if (!branchExists)
                throw new KeyNotFoundException($"Branch with ID {branchId} does not exist.");

            return await _context.StockItems
                .AsNoTracking()
                .Where(s => s.BranchId == branchId)
                .Select(s => new BranchStockDto
                {
                    ProductId = s.ProductId,
                    ProductName = s.Product.Name,
                    Sku = s.Product.Sku,
                    Quantity = s.Quantity
                })
                .ToListAsync();
        }
    }
}
