using Microsoft.EntityFrameworkCore;

namespace Models;


public class Context : DbContext
{
    public Context(DbContextOptions options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<ToDoTask> ToDoTasks { get; set; }
    public DbSet<Members> Members { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Members>()
            .HasOne(m => m.Member)
            .WithMany(u => u.UserMemeberOfTasks)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Members>()
            .HasOne(m => m.Task)
            .WithMany(t => t.MembersOfTask)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }

}