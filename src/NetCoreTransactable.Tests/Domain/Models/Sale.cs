using System;

namespace NetCoreTransactable.Tests.Domain.Models
{
    public class Sale
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid ClientId { get; set; }
        public DateTime DateTime { get; set; }

        public virtual Product Product { get; set; }
        public virtual Client Client { get; set; }
    }
}
