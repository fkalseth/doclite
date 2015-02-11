using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
                store.Save(new TestDocument { Key = "a", Foo = "bar" });
            }
        }

        [Test]
        public void Can_update_document()
        {
            using (var store = Store.Open(DataFile))
            {
                store.Save(new TestDocument { Key = "a", Foo = "bar" });
                store.Save(new TestDocument { Key = "a", Foo = "bar 2" });

                TestDocument fetchedItem = store.Get<TestDocument>("a");

                Debug.WriteLine(fetchedItem.Timestamp);

                Assert.That(fetchedItem.Foo, Is.EqualTo("bar 2"));
            }
        }

        [Test]
        public void Can_retrieve_stored_document_by_key()
        {
            using (var store = Store.Open(DataFile))
            {
                store.Save(new TestDocument { Key = "a", Foo = "bar" });

                var fetched = store.Get<TestDocument>("a");

                Assert.IsNotNull(fetched, "Should have been added to store");
                Assert.That(fetched.Foo, Is.EqualTo("bar"));
            }
        }

        [Test]
        public void Can_delete_stored_document_by_key()
        {
            using (var store = Store.Open(DataFile))
            {
                store.Save(new TestDocument { Key = "a", Foo = "bar" });
                Assert.IsNotNull(store.Get<TestDocument>("a"), "Should have been added to store");
                
                store.Delete<TestDocument>("a");

                Assert.IsNull(store.Get<TestDocument>("a"), "Should have been deleted from store");
            }
        }

        [Test]
        public void Retrieves_null_if_no_document_exists()
        {
            using (var store = Store.Open(DataFile))
            {
                store.Save(new TestDocument { Key = "a", Foo = "bar" });
                Assert.IsNull(store.Get<TestDocument>("no-such-key"));
            }
        }
        
        [Test]
        public void Can_retrieve_all_documents_of_type()
        {
            using (var store = Store.Open(DataFile))
            {
                store.Save(new TestDocument { Key = "a", Foo = "bar" });
                store.Save(new TestDocument { Key = "b", Foo = "bar 2" });

                var all = store.GetAll<TestDocument>();

                Assert.That(all.Count(), Is.EqualTo(2));
            }
        }
    }

    public class TestDocument : Document
    {
        public string Foo { get; set; }
    }
}
