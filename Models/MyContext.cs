﻿using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TestEFCoreBinarySerializer.Models {
    public class MyContext : DbContext {
        public DbSet<Book> Books { get; set; }
        public DbSet<Page> Pages { get; set; }

        public MyContext() {
        }

        public MyContext(DbContextOptions<MyContext> options)
            : base(options) {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (!optionsBuilder.IsConfigured) {
                optionsBuilder.UseSqlite(new SqliteConnection("DataSource=:memory:"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            Book.OnModelCreating(modelBuilder);
            Page.OnModelCreating(modelBuilder);
        }
    }

    public class MyContextForMigrationFactory : IDesignTimeDbContextFactory<MyContext> {
        public MyContext CreateDbContext(string[] args) {
            return new MyContext();
        }
    }
}
