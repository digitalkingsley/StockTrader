﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;



namespace StockTrader.StockTrader_Model
{
    public partial class StockTraderContext : DbContext
    {
        public virtual DbSet<Stock> Stock { get; set; }
        public virtual DbSet<Transaction> Transaction { get; set; }
        public virtual DbSet<User> User { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(@"Server=HQ-IT-19116\SQLSERVERDEV; Database=StockTrader; User Id=kinso; Password=kinso;");
        //}
        public StockTraderContext(DbContextOptions<StockTraderContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Stock>(entity =>
            {
                entity.Property(e => e.StockPurchaseDate).HasColumnType("date");

                entity.Property(e => e.StockSecurityName)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.StockSecuritySymbol)
                    .IsRequired()
                    .HasColumnType("varchar(10)");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Stock)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Stock_User");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(e => e.TransactionDate).HasColumnType("date");

                entity.Property(e => e.TransactionDescription).HasColumnType("varchar(max)");

                entity.Property(e => e.TransactionType).HasColumnType("varchar(10)");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Transaction)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Transaction_User");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserAccountBalance).HasDefaultValueSql("500.0");

                entity.Property(e => e.UserEmail)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.UserPassword)
                    .IsRequired()
                    .HasColumnType("varchar(50)");
            });
        }
    }
}