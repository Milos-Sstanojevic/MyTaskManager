using System.ComponentModel.DataAnnotations;
using Models;

public class ToDoTask
{
    [Key]
    public int ID { get; set; }
    public DateTime DateTaskStarted { get; set; }
    public DateTime DateTaskShouldEnd { get; set; }
    public string? TaskName { get; set; }
    public string? Description { get; set; }
    public StateOfTask StateOfTask { get; set; } = StateOfTask.Default;
    public User? OwnerOfTask { get; set; }
    public List<Members>? MembersOfTask { get; set; }
}

public enum StateOfTask
{
    Default,
    Urgent,
    Important,
    NextDay,
    Done
}