using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace TestEFCoreBinarySerializer.Models {
    [Serializable]
    public class Book {
        public int Id { get; set; }
        public ICollection<Page> Pages { get; set; }

        public static void OnModelCreating(ModelBuilder modelBuilder) {
            var e_tb = modelBuilder.Entity<Book>();
            e_tb.Property(e => e.Id);
            e_tb.HasKey(e => e.Id);
        }

        public static bool InitializeCollectionsExplicitly = false;

        public Book() {
            if (InitializeCollectionsExplicitly) {
                this.Pages = new HashSet<Page>();
            }
        }
    }
}
