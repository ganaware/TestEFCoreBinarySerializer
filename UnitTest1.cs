using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using TestEFCoreBinarySerializer.Models;

namespace Tests {
    public class Tests {
        const int BookId = 123;
        const int PageId1 = 45;
        const int PageId2 = 67;
        const int PageId3 = 68;

        DbContextOptions<MyContext> m_options;

        [SetUp]
        public void Setup() {
            Book book = new Book {
                Id = BookId,
            };
            var page1 = new Page {
                Id = PageId1,
                BookId = BookId,
            };
            var page2 = new Page {
                Id = PageId2,
                BookId = BookId,
            };
            var page3 = new Page {
                Id = PageId3,
                BookId = BookId,
            };

            var conn = new SqliteConnection("DataSource=:memory:");
            conn.Open();
            m_options = new DbContextOptionsBuilder<MyContext>()
                .UseSqlite(conn)
                .Options;
            using (var db = new MyContext(m_options)) {
                db.Database.Migrate();
                db.Books.Add(book);
                db.Pages.Add(page1);
                db.Pages.Add(page2);
                db.Pages.Add(page3);
                db.SaveChanges();
            }
        }

        [Test]
        public void TestInitializeCollectionsExplicitly() {
            Book.InitializeCollectionsExplicitly = true;
            using (var db = new MyContext(m_options)) {
                Book book = db.Books
                    .Where(b => b.Id == BookId)
                    .Include(b => b.Pages)
                    .First();
                Assert.That(book.Pages.Count, Is.EqualTo(3));
                Assert.That(book.Pages.Any(p => p.Id == PageId1), Is.True);
                Assert.That(book.Pages.Any(p => p.Id == PageId2), Is.True);
                Assert.That(book.Pages.Any(p => p.Id == PageId3), Is.True);
                Assert.That(book, Is.BinarySerializable);
                Assert.That(() => {
                    var m = new MemoryStream();
                    var bf = new BinaryFormatter();
                    bf.Serialize(m, book);
                }, Throws.Nothing);
            }

            Book.InitializeCollectionsExplicitly = false;
            using (var db = new MyContext(m_options)) {
                Book book = db.Books
                    .Where(b => b.Id == BookId)
                    .Include(b => b.Pages)
                    .First();
                Assert.That(book.Pages.Count, Is.EqualTo(3));
                Assert.That(book.Pages.Any(p => p.Id == PageId1), Is.True);
                Assert.That(book.Pages.Any(p => p.Id == PageId2), Is.True);
                Assert.That(book.Pages.Any(p => p.Id == PageId3), Is.True);
                Assert.That(book, Is.Not.BinarySerializable);
                Assert.That(() => {
                    var m = new MemoryStream();
                    var bf = new BinaryFormatter();
                    bf.Serialize(m, book);
                }, Throws.InstanceOf<SerializationException>());
            }
        }
    }
}