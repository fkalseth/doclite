doclite, a very simple document database based on Sqlite.PCL

doclite lets you save and retrieve any plain object that implements the IDocument interface (or the abstract Document).


```c#
// API:

Store Store.Open(dataFileName)
void store.Save<TDocument>(document)
TDocument store.Get<TDocument>(key)
void store.Delete<TDocument>(key)
IList<TDocument> store.All<TDocument>()

// example:

using (var store = Store.Open("data.db"))
{
  store.Save(new UserDocument { Key = "user1", Name = "User 1" });
  
  var user = store.Get<UserDocument>("user1");
}
```
