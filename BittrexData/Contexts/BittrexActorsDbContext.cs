using System.Collections.Generic;
using BittrexData.Models;
using Microsoft.EntityFrameworkCore;

namespace BittrexData.Contexts
{
	public class BittrexActorsDbContext : DbContext
	{
		public DbSet<ActorData> ActorDatas { get; set; }

		public BittrexActorsDbContext()
		{
			Database.EnsureCreated();
		}
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(
				@"Server=(localdb)\mssqllocaldb;Database=BittrexActors;Trusted_Connection=True;");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Account>().Property(x => x.BtcCount).HasColumnType("decimal(16,8)");
			modelBuilder.Entity<Account>().Property(x => x.CurrencyCount).HasColumnType("decimal(16,8)");

			modelBuilder.Entity<Transaction>().Property(x => x.BtcCount).HasColumnType("decimal(16,8)");
			modelBuilder.Entity<Transaction>().Property(x => x.CurrencyPrice).HasColumnType("decimal(16,8)");

		}
				
		
	}
}
