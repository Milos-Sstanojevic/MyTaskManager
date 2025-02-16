using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Controllers;


[ApiController]
[Route("[controller]")]
public class TaskController : ControllerBase
{
    public Context Context;

    public TaskController(Context context)
    {
        Context = context;
    }

    [HttpPost("CreateTask")]
    public async Task<ActionResult> CreateTask([FromBody] ToDoTask task)
    {
        try
        {
            var user = CheckIfUserIsLoggedIn();

            ToDoTask toDoTask = new ToDoTask()
            {
                DateTaskStarted = DateTime.Now,
                DateTaskShouldEnd = task.DateTaskShouldEnd,
                TaskName = task.TaskName,
                Description = task.Description,
                StateOfTask = task.StateOfTask,
                OwnerOfTask = user,
                MembersOfTask = new List<Members>()
            };

            await Context.ToDoTasks.AddAsync(toDoTask);

            if (user.TasksOfUser == null)
                user.TasksOfUser = new List<ToDoTask>();

            user.TasksOfUser.Add(toDoTask);

            Context.Update(user);

            await Context.SaveChangesAsync();

            return Ok(toDoTask);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private User CheckIfUserIsLoggedIn()
    {
        if (!User.Identity.IsAuthenticated)
            return null;

        var identity = HttpContext.User.Identity as ClaimsIdentity;

        if (identity == null)
            return null;

        var userClaims = identity.Claims;
        int id = int.Parse(userClaims.FirstOrDefault(p => p.Type == ClaimTypes.Sid)!.Value);

        var user = Context.Users.Where(u => u.ID == id).FirstOrDefault();

        if (user == null)
            return null;

        return user;
    }


    [HttpGet("GetTaskByName/{TaskName}")]
    public async Task<ActionResult> GetTaskByName(string TaskName)
    {
        try
        {
            var user = CheckIfUserIsLoggedIn();

            if (user == null)
                return BadRequest("User has to be logged in!");

            if (string.IsNullOrEmpty(TaskName))
                return BadRequest("You have to provide name of task");

            var task = await Context.ToDoTasks.Where(t => t.TaskName.Equals(TaskName)).Include(t => t.OwnerOfTask).FirstOrDefaultAsync();

            if (task == null)
                return NotFound("No task with that name was found!");

            return Ok(task);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    //KAD BUDES PISAO FUNKCIJE ZA MEMEBERE VRATI SE SVUDA JER TREBA VEROVATNO DA SE OBRISU I ODAVDE
    [HttpDelete("DeleteTaskByName/{TaskName}")]
    public async Task<ActionResult> DeleteTaskByName(string TaskName)
    {
        try
        {
            var user = CheckIfUserIsLoggedIn();

            if (user == null)
                return BadRequest("User has to be logged in for this operation!");

            var task = await Context.ToDoTasks.Where(t => t.TaskName.Equals(TaskName)).Include(t => t.OwnerOfTask).FirstOrDefaultAsync();

            if (task == null)
                return NotFound("Task with given name is not found");

            if (task.OwnerOfTask != user)
                return BadRequest("You are not owner of this task. How did you get it?");

            Context.ToDoTasks.Remove(task);
            user.TasksOfUser!.Remove(task);

            await Context.SaveChangesAsync();

            return Ok("Task successfully deleted!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("UpdateTaskInfo")]
    public async Task<ActionResult> UpdateTaskInfo([FromBody] ToDoTask updatedTask)
    {
        try
        {
            var user = CheckIfUserIsLoggedIn();

            if (user == null)
                return BadRequest("User has to be logged in!");

            var task = await Context.ToDoTasks.Where(t => t.TaskName.Equals(updatedTask.TaskName)).Include(t => t.OwnerOfTask).FirstOrDefaultAsync();

            if (task == null)
                return NotFound("Task not found!");

            task.TaskName = updatedTask.TaskName;
            task.Description = updatedTask.Description;
            task.DateTaskShouldEnd = updatedTask.DateTaskShouldEnd;
            task.StateOfTask = updatedTask.StateOfTask;

            Context.ToDoTasks.Update(task);
            await Context.SaveChangesAsync();

            return Ok("Task successfully updated!");
        }

        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("GetAllTasksOfUser")]
    public async Task<ActionResult> GetAllTasksOfUser()
    {
        try
        {
            var user = CheckIfUserIsLoggedIn();

            if (user == null)
                return BadRequest("User is not logged in!");

            List<ToDoTask> tasks = await Context.ToDoTasks.Where(t => t.OwnerOfTask == user).ToListAsync();

            if (tasks == null)
                return BadRequest("Tasks of this user are not initialized!");

            if (tasks.Count == 0)
                return Ok("This user is not owner of any tasks");

            return Ok(tasks);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    //OVO TREBA TESTIRATI
    [HttpGet("GetAllTasksUserIsMemberOf")]
    public async Task<ActionResult> GetAllTasksUserIsMemeberOf()
    {
        try
        {
            var user = CheckIfUserIsLoggedIn();

            if (user == null)
                return BadRequest("User has to be logged in!");

            List<ToDoTask> tasks = await Context.Members.Where(u => u.Member == user).Include(t => t.Task).Select(t => t.Task).ToListAsync();

            if (tasks == null)
                return BadRequest("Error with getting tasks user is member of!");

            if (tasks.Count == 0)
                return Ok("User is not memeber of any tasks!");

            return Ok(tasks);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

//create and organize tasks, da oznaci zavrsen task, collaborate with team members, track progrss, customize workflow, 