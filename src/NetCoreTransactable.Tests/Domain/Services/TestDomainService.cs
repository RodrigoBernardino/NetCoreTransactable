using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NetCoreTransactable.Tests.Domain.Models;
using NetCoreTransactable.Tests.SQLiteConfig;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreTransactable.Tests.Domain.Services
{
    public class TestDomainService : ITestDomainService
    {
        [Transactable]
        public Sale CreateNewSale(string productName, string clientName, TestDbContext context)
        {
            Product product = context.Products.FirstOrDefault(p => p.Name == productName);
            Client client = context.Clients.FirstOrDefault(p => p.Name == clientName);

            var newSale = new Sale
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                ClientId = client.Id,
                DateTime = DateTime.Now
            };

            EntityEntry<Sale> addedSale = context.Add(newSale);
            context.SaveChanges();

            return addedSale.Entity;
        }

        [Transactable]
        public async Task<Sale> CreateNewSaleAsync(string productName, string clientName, TestDbContext context)
        {
            Product product = await context.Products.FirstOrDefaultAsync(p => p.Name == productName);
            Client client = await context.Clients.FirstOrDefaultAsync(p => p.Name == clientName);

            var newSale = new Sale
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                ClientId = client.Id,
                DateTime = DateTime.Now
            };

            EntityEntry<Sale> addedSale = await context.AddAsync(newSale);
            await context.SaveChangesAsync();

            return addedSale.Entity;
        }

        [Transactable]
        public async Task RemoveLastSaleAsync(TestDbContext context)
        {
            Sale sale = await context.Sales.OrderByDescending(s => s.DateTime).FirstOrDefaultAsync();

            EntityEntry<Sale> removedSale = context.Remove(sale);
            await context.SaveChangesAsync();
        }
    }
}
