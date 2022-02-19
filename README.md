## IoTSharp.EntityFrameworkCore.MongoDB

[![latest version](https://img.shields.io/nuget/v/IoTSharp.EntityFrameworkCore.MongoDB)](https://www.nuget.org/packages/IoTSharp.EntityFrameworkCore.MongoDB) [![preview version](https://img.shields.io/nuget/vpre/IoTSharp.EntityFrameworkCore.MongoDB)](https://www.nuget.org/packages/IoTSharp.EntityFrameworkCore.MongoDB/absoluteLatest) [![downloads](https://img.shields.io/nuget/dt/IoTSharp.EntityFrameworkCore.MongoDB)](https://www.nuget.org/packages/IoTSharp.EntityFrameworkCore.MongoDB)

EFCore.MongoDB  is a MongoDB mapper for .NET. It supports LINQ queries, change tracking, updates, and schema migrations. EFCore.MongoDB   works with MongoDB through a provider plugin API.

### Installation

EFCore.MongoDB   is available on [NuGet](https://www.nuget.org/packages/IoTSharp.EntityFrameworkCore.MongoDB).

```sh
dotnet add package IoTSharp.EntityFrameworkCore.MongoDB

```

 
### Daily builds

We recommend using the [daily builds](docs/DailyBuilds.md) to get the latest code and provide feedback on EF Core. These builds contain latest features and bug fixes; previews and official releases lag significantly behind.

### Basic usage

The following code demonstrates basic usage of EFCore.MongoDB . For a full tutorial configuring the `DbContext`, defining the model, and creating the database, see [getting started](https://docs.microsoft.com/ef/core/get-started/) in the docs.

```cs
using (var db = new BloggingContext())
{
    // Inserting data into the database
    db.Add(new Blog { Url = "http://blogs.msdn.com/adonet" });
    db.SaveChanges();

    // Querying
    var blog = db.Blogs
        .OrderBy(b => b.BlogId)
        .First();

    // Updating
    blog.Url = "https://devblogs.microsoft.com/dotnet";
    blog.Posts.Add(
        new Post
        {
            Title = "Hello World",
            Content = "I wrote an app using EF Core!"
        });
    db.SaveChanges();

    // Deleting
    db.Remove(blog);
    db.SaveChanges();
}
```

### Build from source

Most people use EFCore.MongoDB  by installing pre-build NuGet packages, as shown above. Alternately, [the code can be built and packages can be created directly on your development machine](./docs/getting-and-building-the-code.md).

### Contributing

We welcome community pull requests for bug fixes, enhancements, and documentation. See [How to contribute](./.github/CONTRIBUTING.md) for more information.

### Getting support

If you have a specific question about using these projects, we encourage you to [ask it on Stack Overflow](https://stackoverflow.com/questions/tagged/entity-framework-core*?tab=Votes). If you encounter a bug or would like to request a feature, [submit an issue](https://github.com/dotnet/efcore/issues/new/choose). For more details, see [getting support](.github/SUPPORT.md).

  

## See also

* [Documentation](https://docs.microsoft.com/ef/core/)
* [How to write an EF Core provider](https://docs.microsoft.com/ef/core/providers/writing-a-provider)
* [Code of conduct](.github/CODE_OF_CONDUCT.md)
