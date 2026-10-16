using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RavingoTest.Data;
using RavingoTest.Dto;

using RavingoTest.Models;
using RavingoTest.Service.Impl;


namespace MihiraStores.Tests;

public class UnitTest1
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        var context = new AppDbContext(options);

        // Test Data Seeding
        var branchColombo = new Branch { Id = 1, Name = "Colombo" };
        var branchKandy = new Branch { Id = 2, Name = "Kandy" };
        var product = new Product { Id = 1, Name = "Keeri Samba Rice 5kg", Sku = "RICE-01" };

        context.Branches.AddRange(branchColombo, branchKandy);
        context.Products.Add(product);

        
        context.StockItems.AddRange(
            new StockItem { Id = 1, BranchId = 1, ProductId = 1, Quantity = 20 },
            new StockItem { Id = 2, BranchId = 2, ProductId = 1, Quantity = 10 }
        );

        context.SaveChanges();
        return context;
    }

    
    // TEST 1: "not enough stock" (A rejected transfer must not change any stock)
    
    [Fact]
    public async Task CreateTransfer_WhenSenderHasInsufficientStock_ShouldThrowAndNotChangeStock()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var service = new TransferService(context);

        
        var request = new CreateTransferDto
        {
            FromBranchId = 1,
            ToBranchId = 2,
            ProductId = 1,
            Quantity = 50
        };

        // Act
        Func<Task> act = async () => await service.CreateTransferAsync(request);

        // Assert 1: 
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Sending branch does not have enough stock.");

        // Assert 2: 
        var senderStock = await context.StockItems.FirstAsync(s => s.BranchId == 1 && s.ProductId == 1);
        var receiverStock = await context.StockItems.FirstAsync(s => s.BranchId == 2 && s.ProductId == 1);

        senderStock.Quantity.Should().Be(20, "Sender stock should remain unchanged");
        receiverStock.Quantity.Should().Be(10, "Receiver stock should remain unchanged");

        // Assert 3: 
        var transfersCount = await context.StockTransfers.CountAsync();
        transfersCount.Should().Be(0, "No transfer history should be recorded on rejection");
    }

    
    // TEST 2: "a good transfer moves the stock"
    
    [Fact]
    public async Task CreateTransfer_WhenRequestIsValid_ShouldMoveStockAndCreateTransferRecord()
    {
        
        using var context = GetInMemoryDbContext();
        var service = new TransferService(context);

        
        var request = new CreateTransferDto
        {
            FromBranchId = 1,
            ToBranchId = 2,
            ProductId = 1,
            Quantity = 5
        };

        // Act
        var result = await service.CreateTransferAsync(request);

        // Assert 1: 
        result.Should().NotBeNull();
        result.Quantity.Should().Be(5);
        result.FromBranchName.Should().Be("Colombo");
        result.ToBranchName.Should().Be("Kandy");

        // Assert 2: 
        var senderStock = await context.StockItems.FirstAsync(s => s.BranchId == 1 && s.ProductId == 1);
        senderStock.Quantity.Should().Be(15);

        // Assert 3: 
        var receiverStock = await context.StockItems.FirstAsync(s => s.BranchId == 2 && s.ProductId == 1);
        receiverStock.Quantity.Should().Be(15);

        // Assert 4: 
        var recordedTransfer = await context.StockTransfers.FirstOrDefaultAsync();
        recordedTransfer.Should().NotBeNull();
        recordedTransfer!.Quantity.Should().Be(5);
        recordedTransfer.FromBranchId.Should().Be(1);
        recordedTransfer.ToBranchId.Should().Be(2);
    }

    
    // TEST 3: Identical Branches Reject (Rule 2)
    
    [Fact]
    public async Task CreateTransfer_WhenSourceAndDestinationSame_ShouldThrowArgumentException()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var service = new TransferService(context);

        var request = new CreateTransferDto
        {
            FromBranchId = 1,
            ToBranchId = 1,
            ProductId = 1,
            Quantity = 5
        };

        // Act
        Func<Task> act = async () => await service.CreateTransferAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Source and destination branches cannot be the same.");
    }
}