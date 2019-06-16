using System.Data.Entity;


namespace DataManager.Models
{
    public class BittrexDbContext: DbContext
    {

        public BittrexDbContext(): base("testDb0.1")
        {
            Configuration.LazyLoadingEnabled = true;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().Property(x => x.BtcCount).HasPrecision(16, 8);
            modelBuilder.Entity<Account>().Property(x => x.CurrencyCount).HasPrecision(16, 8);

            modelBuilder.Entity<Observation>().Property(x => x.BidPrice).HasPrecision(16, 8);
            modelBuilder.Entity<Observation>().Property(x => x.AskPrice).HasPrecision(16, 8);
            modelBuilder.Entity<Observation>().Property(x => x.OrderBidSum).HasPrecision(16, 8);
            modelBuilder.Entity<Observation>().Property(x => x.OrderAskSum).HasPrecision(16, 8);

            modelBuilder.Entity<Transaction>().Property(x => x.CurrencySum).HasPrecision(16, 8);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Actor> Actors { get; set; }
        public DbSet<Observation> Observations { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Rule> Rules { get; set; }
        //public DbSet<Actor> Actors { get; set; }

    }
}
