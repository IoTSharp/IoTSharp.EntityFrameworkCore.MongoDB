using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;

public class User
{
 //[Key,Column("_id" , Order = 0, TypeName =nameof(ObjectId)), DatabaseGenerated(DatabaseGeneratedOption.None)]
  [Column("_id")]
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public string EX { get; set; }
    public string Sex { get; set; }
    public string Address { get; set; }
    public string Age { get; set; }
    public string Creator { get; set; }
    public string CreateDate { get; set; }
    public string LastEditDate { get; set; }


}
