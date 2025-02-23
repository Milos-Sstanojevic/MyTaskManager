using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Models;

public class Members
{
    [Key]
    public int ID { get; set; }

    public User Member { get; set; } = null!;
    [JsonIgnore]
    public ToDoTask Task { get; set; } = null!;
}