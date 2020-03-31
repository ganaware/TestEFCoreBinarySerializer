using Microsoft.EntityFrameworkCore;
using System;

namespace TestEFCoreBinarySerializer.Models {
    [Serializable]
    public class Page {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }

        public static void OnModelCreating(ModelBuilder modelBuilder) {
            var e_tb = modelBuilder.Entity<Page>();
            e_tb.Property(e => e.Id);
            e_tb.Property(e => e.BookId);
            e_tb.HasKey(e => e.Id);
            e_tb.HasOne(e => e.Book)
                .WithMany(e => e.Pages)
                .HasForeignKey(e => e.BookId);
        }
    }
}
