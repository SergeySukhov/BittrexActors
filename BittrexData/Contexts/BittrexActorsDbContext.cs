using System.Collections.Generic;
using BittrexData.Models;
using Microsoft.EntityFrameworkCore;

namespace BittrexData.Contexts
{
	public class BittrexActorsDbContext : DbContext
	{
		public DbSet<ActorDataDto> ActorDatas { get; set; }

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

			modelBuilder.Entity<AccountDto>().Property(x => x.BtcCount).HasColumnType("decimal(16,8)");
			modelBuilder.Entity<AccountDto>().Property(x => x.CurrencyCount).HasColumnType("decimal(16,8)");

			modelBuilder.Entity<TransactionDto>().Property(x => x.BtcCount).HasColumnType("decimal(16,8)");
			modelBuilder.Entity<TransactionDto>().Property(x => x.CurrencyPrice).HasColumnType("decimal(16,8)");

			modelBuilder.Entity<PredictionDto>().Property(x => x.OldPrice).HasColumnType("decimal(16,8)");
			//modelBuilder.Entity<PredictionUnit>().Property(x => x.Coefficient).HasColumnType("decimal(4,8)");

			//modelBuilder.Entity<BalancedRule>().Property(x => x.Coefficient).HasColumnType("decimal(4,8)");


			modelBuilder.Entity<BalancedRuleDto>().HasOne(a => a.ActorData).WithMany(aa => aa.Rules).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<TransactionDto>().HasOne(a => a.ActorData).WithMany(aa => aa.Transactions).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<PredictionDto>().HasOne(a => a.ActorData).WithMany(aa => aa.Predictions).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<AccountDto>().HasOne(a => a.ActorData).WithOne(aa => aa.Account).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<PredictionUnitDto>().HasOne(a => a.Prediction).WithMany(aa => aa.RulePredictions).OnDelete(DeleteBehavior.Cascade);
		}
				
		
	}
}
