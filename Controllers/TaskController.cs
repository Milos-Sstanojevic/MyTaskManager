using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Controllers;

//TREBA REFAKTORISATI OVU KLASU

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

    // TESTIRAJ OPET OVU FUNKCIJU KAD NAPRAVIS VISE KORISNIKA
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

            var userMember = new User();

            if (task.MembersOfTask != null && task.MembersOfTask.Count() > 0)
            {
                foreach (Members member in task.MembersOfTask)
                {
                    userMember = await Context.Users.Where(u => u.ID == member.Member.ID).FirstOrDefaultAsync();

                    if (userMember != null && userMember.UserMemeberOfTasks != null && userMember.UserMemeberOfTasks.Contains(member))
                        userMember.UserMemeberOfTasks.Remove(member);

                    Context.Members.Remove(member);

                    if (userMember != null)
                        Context.Update(userMember);
                }
            }

            Context.ToDoTasks.Remove(task);
            user.TasksOfUser!.Remove(task);
            Context.Update(user);

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

    [HttpGet("GetAllActiveTasksOfUser")]
    public async Task<ActionResult> GetAllActiveTasksOfUser()
    {
        try
        {
            var user = CheckIfUserIsLoggedIn();
            if (user == null)
                return BadRequest("User must be logged in, how did you get here!");

            List<ToDoTask> tasksOfUser = await Context.ToDoTasks.Where(t => t.OwnerOfTask == user && t.StateOfTask != StateOfTask.Done).ToListAsync();

            if (tasksOfUser == null || tasksOfUser.Count == 0)
                return Ok("No active tasks of this user!");

            return Ok(tasksOfUser);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("GetAllUrgentTasksOfUser")]
    public async Task<ActionResult> GetAllUrgentTasksOfUser()
    {
        try
        {
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("GetAllImportantTasksOfUser")]
    public async Task<ActionResult> GetAllImportantTasksOfUser()
    {
        try
        {
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("GetAllNextDayTasksOfUser")]
    public async Task<ActionResult> GetAllNextDayTasksOfUser()
    {
        try
        {
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("GetAllDoneTasksOfUser")]
    public async Task<ActionResult> GetAllDoneTasksOfUser()
    {
        try
        {
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("GetAllActiveTasksUserIsMemberOf")]
    public async Task<ActionResult> GetAllActiveTasksUserIsMemberOf()
    {
        try
        {
            var user = CheckIfUserIsLoggedIn();

            if (user == null)
                return BadRequest("User has to be logged in!");

            List<ToDoTask> tasks = await Context.Members.Where(u => u.Member == user && u.Task.StateOfTask != StateOfTask.Done).Include(t => t.Task).Select(t => t.Task).ToListAsync();

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

    //Da ti vrati sve taskove kojih je korisnik memeber ali samo one koji su Urgent
    //Da ti vrati sve taskove kojih je korisnik memeber ali samo one koji su NextDay
    //Da ti vrati sve taskove kojih je korisnik memeber ali samo one koji su Important
    //Da ti vrati sve taskove kojih je korisnik memeber ali samo one koji su Done

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

    [HttpPost("AddMembersToTheTaskByMail/{TaskName}")]
    public async Task<ActionResult> AddMembersToTheTaskByMail([FromBody] List<string> Emails, string TaskName)
    {
        try
        {
            var loggedInUser = CheckIfUserIsLoggedIn();

            if (loggedInUser == null)
                return BadRequest("How did you get here? You are not logged in!");

            if (string.IsNullOrEmpty(TaskName))
                return BadRequest("No task name was provided!");

            var task = await Context.ToDoTasks
                .Where(t => t.TaskName.Equals(TaskName))
                .Include(t => t.MembersOfTask)
                .FirstOrDefaultAsync();

            if (task == null)
                return BadRequest("No task with the provided name was found!");

            if (Emails == null || Emails.Count == 0)
                return Ok("No members added to the task.");

            List<string> errors = new();
            List<string> addedUsers = new();

            foreach (string mail in Emails)
            {
                if (!mail.Contains("@gmail.com") && !mail.Contains("@outlook.com"))
                {
                    errors.Add($"Invalid email address: {mail}");
                    continue;
                }

                var user = await Context.Users
                    .Where(u => u.Email == mail)
                    .Include(u => u.UserMemeberOfTasks)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    errors.Add($"User with email {mail} not found.");
                    continue;
                }

                if (user.Email == loggedInUser.Email)
                {
                    errors.Add($"You are owner of this task no need to add youself as member!");
                    continue;
                }

                Members members = new()
                {
                    Member = user,
                    Task = task
                };

                user.UserMemeberOfTasks ??= new List<Members>();
                task.MembersOfTask ??= new List<Members>();

                user.UserMemeberOfTasks.Add(members);
                task.MembersOfTask.Add(members);

                await Context.AddAsync(members);
                Context.Update(user);
                Context.Update(task);

                await Context.SaveChangesAsync();

                addedUsers.Add(mail);
            }

            var response = new
            {
                Message = "Process completed.",
                AddedUsers = addedUsers,
                Errors = errors
            };

            return Ok(response);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("MarkTaskAsFinished/{TaskName}")]
    public async Task<ActionResult> MarkTaskAsFinished(string TaskName)
    {
        try
        {
            var user = CheckIfUserIsLoggedIn();

            if (user == null)
                return BadRequest("How did you get here, you must be logged in!!!");

            if (string.IsNullOrWhiteSpace(TaskName))
                return BadRequest("Task name must be provided!");

            var task = await Context.ToDoTasks.Where(t => t.TaskName.Equals(TaskName)).FirstOrDefaultAsync();

            if (task == null)
                return NotFound("No task with given name exist!");

            task.StateOfTask = StateOfTask.Done;

            Context.Update(task);

            await Context.SaveChangesAsync();

            return Ok("Task successfully set as finished!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}