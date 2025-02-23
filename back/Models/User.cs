using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Models;

public class User
{
    [Key]
    public int ID { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public byte[] Password { get; set; } = null!;
    public byte[] Salt { get; set; } = null!;

    [JsonIgnore]
    public List<ToDoTask>? TasksOfUser { get; set; }
    [JsonIgnore]
    public List<Members>? UserMemeberOfTasks { get; set; }
}