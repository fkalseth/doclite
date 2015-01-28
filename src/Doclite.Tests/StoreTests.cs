using System;
using System.IO;
using NUnit.Framework;

namespace Doclite.Tests
{
    public class StoreTests
    {
        const string DataFile = "data.db";

        [SetUp]
        public void Setup()
        {
            GC.Collect();
            if (File.Exists(DataFile)) File.Delete(DataFile);
        }

        [Test]
        public void Can_connect()
        {
            using (var store = Store.Open(DataFile))
            {
            }
        }
        
        [Test]
        public void Can_store_document()
        {
            using (var store = Store.Open(DataFile))
            {
                store.Save(new Test { Key = "a", Foo = "bar" });
            }
        }

        [Test]
        public void Can_update_document()
        {
            using (var store = Store.Open(DataFile))
            {
                store.Save(new Test { Key = "a", Foo = "bar" });
                store.Save(new Test { Key = "a", Foo = "bar 2" });

                Test fetchedItem = store.Get<Test>("a");

                Assert.That(fetchedItem.Foo, Is.EqualTo("bar 2"));
            }
        }

        [Test]
        public void Can_retrieve_stored_document_by_key()
        {
            using (var store = Store.Open(DataFile))
            {
                store.Save(new Test { Key = "a", Foo = "bar" });

                var fetched = store.Get<Test>("a");

                Assert.IsNotNull(fetched, "Should have been added to store");
                Assert.That(fetched.Foo, Is.EqualTo("bar"));
            }
        }

        [Test]
        public void Can_delete_stored_document_by_key()
        {
            using (var store = Store.Open(DataFile))
            {
                store.Save(new Test { Key = "a", Foo = "bar" });
                Assert.IsNotNull(store.Get<Test>("a"), "Should have been added to store");
                
                store.Delete("test", "a");

                Assert.IsNull(store.Get<Test>("a"), "Should have been deleted from store");
            }
        }

        [Test]
        public void Retrieves_null_if_no_document_exists()
        {
            using (var store = Store.Open(DataFile))
            {
                store.Save(new Test { Key = "a", Foo = "bar" });
                Assert.IsNull(store.Get<Test>("no-such-key"));
            }
        }

        [Test]
        public void Retrieves_null_if_table_does_not_exist()
        {
            using (var store = Store.Open(DataFile))
            {
                Assert.IsNull(store.Get<Test>("no-such-key"));
            }
        }
    }

    public class Test : IDocument
    {
        public string Foo { get; set; }
    }
}
