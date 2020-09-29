using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetCoreTransactable.Extensions;
using NetCoreTransactable.Tests.Domain.Models;
using NetCoreTransactable.Tests.Domain.Services;
using NetCoreTransactable.Tests.SQLiteConfig;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreTransactable.Tests
{
    public class NetCoreTransactableTests : DatabaseTests
    {
        public NetCoreTransactableTests()
            : base(
                  new DbContextOptionsBuilder<TestDbContext>()
                    .UseSqlite("Filename=TestDb.db")
                    .Options)
        {
        }

        private ITestDomainService _testService { get; set; }

        [SetUp]
        public void SetUp()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddOptions();
            serviceCollection.EnableCoreProxy(p => p
                .AddInterceptor<TransactableAttribute, TransactionControlInterceptor>());

            serviceCollection.AddScopedWithProxy<ITestDomainService, TestDomainService>();

            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            _testService = serviceProvider.GetService<ITestDomainService>();
        }

        [Test]
        public void Create_New_Sale()
        {
            using (var context = new TestDbContext(ContextOptions))
            {
                Product product = context.Products.FirstOrDefault(p => p.Name == "Milk");
                Sale newSale = _testService.CreateNewSale("Milk", "John Doe", context);

                Assert.AreEqual(product.Id, newSale.ProductId);
            }

            using (var context = new TestDbContext(ContextOptions))
            {
                Sale sale = context.Sales
                    .Include(s => s.Product)
                    .OrderByDescending(s => s.DateTime)
                    .FirstOrDefault();

                Assert.AreEqual("Milk", sale.Product.Name);
            }
        }

        [Test]
        public async Task Create_New_Sale_Async()
        {
            using (var context = new TestDbContext(ContextOptions))
            {
                Product product = await context.Products.FirstOrDefaultAsync(p => p.Name == "Beer");
                Sale newSale = await _testService.CreateNewSaleAsync("Beer", "Mary Jane", context);

                Assert.AreEqual(product.Id, newSale.ProductId);
            }

            using (var context = new TestDbContext(ContextOptions))
            {
                Sale sale = await context.Sales
                    .Include(s => s.Product)
                    .OrderByDescending(s => s.DateTime)
                    .FirstOrDefaultAsync();

                Assert.AreEqual("Beer", sale.Product.Name);
            }
        }

        [Test]
        public async Task Remove_Last_Sale_Async()
        {
            using var context = new TestDbContext(ContextOptions);
            Sale lastSale = await context.Sales
                .Include(s => s.Product)
                .OrderByDescending(s => s.DateTime)
                .FirstOrDefaultAsync();

            await _testService.RemoveLastSaleAsync(context);

            Sale newLastSale = await context.Sales
                .Include(s => s.Product)
                .OrderByDescending(s => s.DateTime)
                .FirstOrDefaultAsync();

            Assert.AreNotEqual(lastSale.Id, newLastSale.Id);
        }
    }
}
