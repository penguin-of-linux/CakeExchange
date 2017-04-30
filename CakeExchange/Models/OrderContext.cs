using System.Data.Common;
using System.Data.Entity;

namespace CakeExchange.Models
{
    public class OrderContext : DbContext
    {
        public OrderContext() : base("DbConnection") { }
        public OrderContext(DbConnection connection) : base(connection, true) { }

        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<HistoryUnit> History { get; set; }
    }
}