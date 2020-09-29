using Microsoft.EntityFrameworkCore;
using NetCoreTransactable.Tests.Domain.Models;
using System;
using System.Collections.Generic;

namespace NetCoreTransactable.Tests.SQLiteConfig
{
    public class DatabaseTests
    {
        protected DatabaseTests(DbContextOptions<TestDbContext> contextOptions)
        {
            ContextOptions = contextOptions;
            Seed();
        }

        protected DbContextOptions<TestDbContext> ContextOptions { get; }

        private void Seed()
        {
            using var context = new TestDbContext(ContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var products = new List<Product>
            {
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Soda",
                    Type = "Drink",
                    Price = 2.15
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Milk",
                    Type = "Drink",
                    Price = 1.50
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Cheese",
                    Type = "Food",
                    Price = 4.00
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Bread",
                    Type = "Food",
                    Price = 0.50
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Beer",
                    Type = "Drink",
                    Price = 3.00
                }
            };

            var clients = new List<Client>
            {
                new Client
                {
                    Id = Guid.NewGuid(),
                    Name = "John Doe",
                    Phone = "123456789"
                },
                new Client
                {
                    Id = Guid.NewGuid(),
                    Name = "Mary Jane",
                    Phone = "987654321"
                },
                new Client
                {
                    Id = Guid.NewGuid(),
                    Name = "Miles Morales",
                    Phone = "0005558881"
                }
            };

            context.Products.AddRange(products);
            context.Clients.AddRange(clients);
            context.SaveChanges();
        }
    }
}
