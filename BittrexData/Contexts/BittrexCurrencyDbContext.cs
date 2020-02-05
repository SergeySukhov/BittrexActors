using System.Collections.Generic;
using BittrexData.Models;
using Microsoft.EntityFrameworkCore;

namespace BittrexData.Contexts
{
	public class BittrexCurrencyDbContext : DbContext
	{
		public DbSet<Currency> CurrencyDatas { get; set; }

		public BittrexCurrencyDbContext()
		{
			Database.EnsureCreated();
		}
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(
				@"Server=(localdb)\mssqllocaldb;Database=BittrexCurrency;Trusted_Connection=True;");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Account>().Property(x => x.BtcCount).HasColumnType("decimal(16,8)");
			
		}
				
		
	}
}
