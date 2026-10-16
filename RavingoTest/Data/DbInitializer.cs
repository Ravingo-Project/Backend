using RavingoTest.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace RavingoTest.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            
            if (context.Branches.Any()) return;

            // 1. Add three
            var colombo = new Branch { Name = "Colombo" };
            var kandy = new Branch { Name = "Kandy" };
            var galle = new Branch { Name = "Galle" };

            context.Branches.AddRange(colombo, kandy, galle);

            // 2. Add 5 products
            var p1 = new Product { Name = "Keeri Samba Rice 5kg", Sku = "RICE-01" };
            var p2 = new Product { Name = "White Sugar 1kg", Sku = "SUGAR-01" };
            var p3 = new Product { Name = "Ceylon Tea 400g", Sku = "TEA-01" };
            var p4 = new Product { Name = "Milk Powder 400g", Sku = "MILK-01" };
            var p5 = new Product { Name = "Wheat Flour 1kg", Sku = "FLOUR-01" };

            context.Products.AddRange(p1, p2, p3, p4, p5);
            context.SaveChanges(); // Ids auto-generate කරගැනීම සඳහා save කරයි

            // 3. Add Starting Stock 
            // Colombo Stock
            context.StockItems.AddRange(
                new StockItem { BranchId = colombo.Id, ProductId = p1.Id, Quantity = 100 },
                new StockItem { BranchId = colombo.Id, ProductId = p2.Id, Quantity = 50 },
                new StockItem { BranchId = colombo.Id, ProductId = p3.Id, Quantity = 40 },
                new StockItem { BranchId = colombo.Id, ProductId = p4.Id, Quantity = 60 },
                new StockItem { BranchId = colombo.Id, ProductId = p5.Id, Quantity = 80 }
            );

            // Kandy Stock
            context.StockItems.AddRange(
                new StockItem { BranchId = kandy.Id, ProductId = p1.Id, Quantity = 25 },
                new StockItem { BranchId = kandy.Id, ProductId = p2.Id, Quantity = 30 },
                new StockItem { BranchId = kandy.Id, ProductId = p3.Id, Quantity = 15 },
                new StockItem { BranchId = kandy.Id, ProductId = p4.Id, Quantity = 20 },
                new StockItem { BranchId = kandy.Id, ProductId = p5.Id, Quantity = 40 }
            );

            // Galle Stock
            context.StockItems.AddRange(
                new StockItem { BranchId = galle.Id, ProductId = p1.Id, Quantity = 10 },
                new StockItem { BranchId = galle.Id, ProductId = p2.Id, Quantity = 15 },
                new StockItem { BranchId = galle.Id, ProductId = p3.Id, Quantity = 10 },
                new StockItem { BranchId = galle.Id, ProductId = p4.Id, Quantity = 10 },
                new StockItem { BranchId = galle.Id, ProductId = p5.Id, Quantity = 20 }
            );

            context.SaveChanges();
        }
    }

}
