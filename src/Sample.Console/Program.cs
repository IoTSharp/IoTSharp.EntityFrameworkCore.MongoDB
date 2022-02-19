using IoTSharp.EntityFrameworkCore.MongoDB.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

string dbname = "iotsharp";
var connStr = "mongodb://root:kissme@mongodb:27017";
var client = new MongoDB.Driver.MongoClient(connStr);

IMongoDatabase db = client.GetDatabase(dbname);

var  collection = db.GetCollection<User>("User");

User MongodbLog = new User();
MongodbLog.Id = ObjectId.GenerateNewId();
MongodbLog.Name = "测试信息name";
MongodbLog.EX = "错误信息";
MongodbLog.Sex = "男";
MongodbLog.Address = "北京市";
MongodbLog.Age = "228";
MongodbLog.Creator = "liusqd";

MongodbLog.CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
MongodbLog.LastEditDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
//collection.InsertOneAsync(MongodbLog);
//collection.DeleteOne(u => u.Id == MongodbLog.Id);
//db.DropCollection("User");

var serviceCollection = new ServiceCollection();
serviceCollection.AddMongoDB<ApplicationDbContext>(connStr, dbname);

serviceCollection.AddEntityFrameworkMongoDB();
var services = serviceCollection.BuildServiceProvider(validateScopes: true);

using (var serviceScope = services
           .GetRequiredService<IServiceScopeFactory>()
           .CreateScope())
{

    using (var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>())
    {
        context.Users.Add(MongodbLog);
        context.SaveChanges();
        MongodbLog.LastEditDate = DateTime.Now.ToString();
        MongodbLog.Age = "2222222";
        context.Users.Update(MongodbLog);
        context.SaveChanges();
        context.Users.Remove(MongodbLog);
        context.SaveChanges();
    }

}
    

 




