using NetCoreTransactable.Tests.Domain.Models;
using NetCoreTransactable.Tests.SQLiteConfig;
using System.Threading.Tasks;

namespace NetCoreTransactable.Tests.Domain.Services
{
    public interface ITestDomainService
    {
        Sale CreateNewSale(string productName, string clientName, TestDbContext context);
        Task<Sale> CreateNewSaleAsync(string productName, string clientName, TestDbContext context);
        Task RemoveLastSaleAsync(TestDbContext context);
    }
}