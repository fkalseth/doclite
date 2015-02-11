doclite, a very simple document database based on Sqlite.PCL

doclite lets you save and retrieve any plain object that implements Document (or IDocument directly).


```c#
// API:

Store Store.Open(dataFileName)
void store.Save<TDocument>(document)
TDocument store.Get<TDocument>(key)
void store.Delete<TDocument>(key)
IList<TDocument> store.All<TDocument>()

where TDocument : IDocument

// example:

using (var store = Store.Open("data.db"))
{
  store.Save(new UserDocument { Key = "user1", Name = "User 1" });
  
  var user = store.Get<UserDocument>("user1");
}

// given:

public class UserDocument : Document
{
  public string Name { get; set; }
}

```
