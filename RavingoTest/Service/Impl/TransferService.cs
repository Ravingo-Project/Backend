using RavingoTest.Data;
using RavingoTest.Dto;
using RavingoTest.Models;
using RavingoTest.Service.Interface;
using Microsoft.EntityFrameworkCore;
namespace RavingoTest.Service.Impl
{
    public class TransferService:ITransferService
    {
        private readonly AppDbContext _context;

        public TransferService(AppDbContext context)
        {
            _context = context;
        }

        // POST /transfers
        public async Task<TransferResponseDto> CreateTransferAsync(CreateTransferDto dto)
        {
            
            if (dto.Quantity <= 0)
                throw new ArgumentException("Transfer quantity must be greater than zero.");

            
            if (dto.FromBranchId == dto.ToBranchId)
                throw new ArgumentException("Source and destination branches cannot be the same.");

            
            var fromBranch = await _context.Branches.FindAsync(dto.FromBranchId);
            var toBranch = await _context.Branches.FindAsync(dto.ToBranchId);
            var product = await _context.Products.FindAsync(dto.ProductId);

            if (fromBranch == null || toBranch == null)
                throw new KeyNotFoundException("One or both branches do not exist.");

            if (product == null)
                throw new KeyNotFoundException("Product does not exist.");

            // Rule: A rejected transfer must not change any stock.
            
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var senderStock = await _context.StockItems
                    .FirstOrDefaultAsync(s => s.BranchId == dto.FromBranchId && s.ProductId == dto.ProductId);

                
                if (senderStock == null || senderStock.Quantity < dto.Quantity)
                    throw new InvalidOperationException("Sending branch does not have enough stock.");

               
                senderStock.Quantity -= dto.Quantity;

                
                var receiverStock = await _context.StockItems
                    .FirstOrDefaultAsync(s => s.BranchId == dto.ToBranchId && s.ProductId == dto.ProductId);

                if (receiverStock == null)
                {
                    receiverStock = new StockItem
                    {
                        BranchId = dto.ToBranchId,
                        ProductId = dto.ProductId,
                        Quantity = dto.Quantity
                    };
                    _context.StockItems.Add(receiverStock);
                }
                else
                {
                    receiverStock.Quantity += dto.Quantity;
                }

                
                var transfer = new StockTransfer
                {
                    FromBranchId = dto.FromBranchId,
                    ToBranchId = dto.ToBranchId,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    CreatedAtUtc = DateTime.UtcNow
                };

                _context.StockTransfers.Add(transfer);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new TransferResponseDto
                {
                    Id = transfer.Id,
                    FromBranchId = fromBranch.Id,
                    FromBranchName = fromBranch.Name,
                    ToBranchId = toBranch.Id,
                    ToBranchName = toBranch.Name,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Quantity = transfer.Quantity,
                    CreatedAtUtc = transfer.CreatedAtUtc
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // GET /transfers 
        public async Task<IEnumerable<TransferResponseDto>> GetTransferHistoryAsync()
        {
            return await _context.StockTransfers
                .AsNoTracking()
                .OrderByDescending(t => t.CreatedAtUtc)
                .Select(t => new TransferResponseDto
                {
                    Id = t.Id,
                    FromBranchId = t.FromBranchId,
                    FromBranchName = t.FromBranch.Name,
                    ToBranchId = t.ToBranchId,
                    ToBranchName = t.ToBranch.Name,
                    ProductId = t.ProductId,
                    ProductName = t.Product.Name,
                    Quantity = t.Quantity,
                    CreatedAtUtc = t.CreatedAtUtc
                })
                .ToListAsync();
        }
    }
}
