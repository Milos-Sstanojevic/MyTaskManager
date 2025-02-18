using System.ComponentModel.DataAnnotations;

namespace Models;

public class Members
{
    [Key]
    public int ID { get; set; }

    public User Member { get; set; } = null!;
    public ToDoTask Task { get; set; } = null!;
}