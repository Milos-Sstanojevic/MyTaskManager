using System.Runtime.CompilerServices;
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

            if (user == null)
                return BadRequest("No user is logged in!");

            var tasks = await Context.ToDoTasks.Include(u => u.OwnerOfTask).Where(t => t.TaskName.Equals(task.TaskName) && t.OwnerOfTask.UserName.Equals(user.UserName)).FirstOrDefaultAsync();

            if (tasks != null)
                return BadRequest("Task with that name already exist for this user");

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

    [HttpGet("GetAllMembersOfTask/{TaskName}")]
    public async Task<ActionResult> GetAllMembersOfTask(string TaskName)
    {
        try
        {
            var user = CheckIfUserIsLoggedIn();

            if (user == null)
                return BadRequest("User has to be logged in!");

            if (string.IsNullOrEmpty(TaskName))
                return BadRequest("You have to provide name of task!");

            var task = await Context.ToDoTasks.Where(t => t.TaskName.Equals(TaskName)).FirstOrDefaultAsync();

            if (task == null)
                return NotFound("No task with that name was found!");

            var members = await Context.Members.Where(t => t.Task == task).Include(t => t.Member).ToListAsync();

            if (members == null)
                return BadRequest("Error with geting members of this task");
            if (members.Count == 0)
                return NotFound("There are no members on this task!");

            return Ok(members);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("DeleteTaskByName/{TaskName}")]
    public async Task<ActionResult> DeleteTaskByName(string TaskName)
    {
        try
        {
            var user = CheckIfUserIsLoggedIn();

            if (user == null)
                return BadRequest("User has to be logged in for this operation!");

            var task = await Context.ToDoTasks
                .Where(t => t.TaskName.Equals(TaskName))
                .Include(t => t.OwnerOfTask)
                .Include(t => t.MembersOfTask) // Ensure members are loaded
                .FirstOrDefaultAsync();

            if (task == null)
                return NotFound("Task with the given name was not found!");

            if (task.OwnerOfTask.ID != user.ID)
                return BadRequest("You are not the owner of this task.");

            if (task.MembersOfTask != null && task.MembersOfTask.Count > 0)
                Context.Members.RemoveRange(task.MembersOfTask);

            user.TasksOfUser?.Remove(task);
            Context.Update(user);

            Context.ToDoTasks.Remove(task);

            await Context.SaveChangesAsync();

            return Ok("Task successfully deleted!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpPut("UpdateTaskInfo/{TaskName}")]
    public async Task<ActionResult> UpdateTaskInfo(string TaskName, [FromBody] ToDoTask updatedTask)
    {
        try
        {
            var user = CheckIfUserIsLoggedIn();

            if (user == null)
                return BadRequest("User has to be logged in!");

            var task = await Context.ToDoTasks.Where(t => t.TaskName.Equals(TaskName)).Include(t => t.OwnerOfTask).FirstOrDefaultAsync();

            if (task == null)
                return NotFound("Task not found!");

            if (TaskName != updatedTask.TaskName)
            {
                var checkExistingNames = await Context.ToDoTasks.Where(t => t.TaskName.Equals(updatedTask.TaskName) && t.OwnerOfTask.UserName.Equals(user.UserName)).FirstOrDefaultAsync();

                //proveri dal radi ovo
                if (checkExistingNames != null)
                    return BadRequest("Think of a new name for task you already have one with this!");
            }

            if (user != task.OwnerOfTask)
                return BadRequest("You are not owner of task so you can't change anything!");

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

    [HttpGet("GetAllPersonalTasksOfUser")]
    public async Task<ActionResult> GetAllPersonalTasksOfUser()
    {
        try
        {
            var user = CheckIfUserIsLoggedIn();

            if (user == null)
                return BadRequest("User is not logged in!");

            List<ToDoTask> tasks = await Context.ToDoTasks.Include(t => t.MembersOfTask).Where(t => t.OwnerOfTask == user && t.MembersOfTask!.Count == 0).ToListAsync();

            if (tasks == null)
                return BadRequest("Tasks of this user are not initialized!");

            if (tasks.Count == 0)
                return Ok(tasks);

            return Ok(tasks);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("GetAllTeamTasksOfUser")]
    public async Task<ActionResult> GetAllTeamTasksOfUser()
    {
        try
        {
            var user = CheckIfUserIsLoggedIn();

            if (user == null)
                return BadRequest("User is not logged in!");

            List<ToDoTask> tasks = await Context.ToDoTasks.Include(t => t.MembersOfTask).Where(t => t.OwnerOfTask == user && t.MembersOfTask!.Count != 0).ToListAsync();

            if (tasks == null)
                return BadRequest("Tasks of this user are not initialized!");

            if (tasks.Count == 0)
                return Ok(tasks);

            return Ok(tasks);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("GetAllActiveTasksOfUserPersonal")]
    public async Task<ActionResult> GetAllActiveTasksOfUserPersonal()
    {
        try
        {
            var user = CheckIfUserIsLoggedIn();
            if (user == null)
                return BadRequest("User must be logged in, how did you get here!");

            List<ToDoTask> tasksOfUser = await Context.ToDoTasks.Include(t => t.MembersOfTask).Where(t => t.OwnerOfTask == user && t.StateOfTask != StateOfTask.Done && t.MembersOfTask!.Count == 0).ToListAsync();

            if (tasksOfUser == null || tasksOfUser.Count == 0)
                return Ok("No active tasks of this user!");

            return Ok(tasksOfUser);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("GetAllUrgentTasksOfUserPersonal")]
    public async Task<ActionResult> GetAllUrgentTasksOfUserPersonal()
    {
        try
        {
            var user = CheckIfUserIsLoggedIn();
            if (user == null)
                return BadRequest("User must be logged in!");

            List<ToDoTask> tasksOfUser = await Context.ToDoTasks.Include(t => t.MembersOfTask).Where(t => t.OwnerOfTask == user && t.StateOfTask == StateOfTask.Urgent && t.MembersOfTask!.Count == 0).ToListAsync();

            if (tasksOfUser == null || tasksOfUser.Count == 0)
                return Ok("No urgent tasks of user!");
            return Ok(tasksOfUser);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("GetAllImportantTasksOfUserPersonal")]
    public async Task<ActionResult> GetAllImportantTasksOfUserPersonal()
    {
        try
        {
            var user = CheckIfUserIsLoggedIn();
            if (user == null)
                return BadRequest("User must be logged in!");

            List<ToDoTask> tasksOfUser = await Context.ToDoTasks.Include(t => t.MembersOfTask).Where(t => t.OwnerOfTask == user && t.StateOfTask == StateOfTask.Important && t.MembersOfTask!.Count == 0).ToListAsync();
            if (tasksOfUser == null || tasksOfUser.Count == 0)
                return Ok("No important tasks for user!");
            return Ok(tasksOfUser);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("GetAllNextDayTasksOfUserPersonal")]
    public async Task<ActionResult> GetAllNextDayTasksOfUserPersonal()
    {
        try
        {
            var user = CheckIfUserIsLoggedIn();
            if (user == null)
                return BadRequest("User must be logged in!");

            List<ToDoTask> tasksOfUser = await Context.ToDoTasks.Include(t => t.MembersOfTask).Where(t => t.OwnerOfTask == user && t.StateOfTask == StateOfTask.NextDay && t.MembersOfTask!.Count == 0).ToListAsync();

            if (tasksOfUser == null || tasksOfUser.Count == 0)
                return Ok("No tasks for next days!");
            return Ok(tasksOfUser);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("GetAllDoneTasksOfUserPersonal")]
    public async Task<ActionResult> GetAllDoneTasksOfUserPersonal()
    {
        try
        {
            var user = CheckIfUserIsLoggedIn();

            if (user == null)
                return BadRequest("User has to be logged in!");

            List<ToDoTask> tasks = await Context.ToDoTasks.Include(t => t.MembersOfTask).Where(t => t.OwnerOfTask == user && t.StateOfTask == StateOfTask.Done && t.MembersOfTask!.Count == 0).ToListAsync();

            if (tasks == null)
                return BadRequest("Error with getting finished tasks!");

            if (tasks.Count == 0)
                return Ok("User has no done tasks!");

            return Ok(tasks);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("GetAllActiveTasksOfUserTeam")]
    public async Task<ActionResult> GeGetAllActiveTasksOfUserTeam()
    {
        try
        {
            var user = CheckIfUserIsLoggedIn();
            if (user == null)
                return BadRequest("User must be logged in, how did you get here!");

            List<ToDoTask> tasksOfUser = await Context.ToDoTasks.Include(t => t.MembersOfTask).Where(t => t.OwnerOfTask == user && t.StateOfTask != StateOfTask.Done && t.MembersOfTask!.Count != 0).ToListAsync();

            if (tasksOfUser == null || tasksOfUser.Count == 0)
                return Ok("No active tasks of this user!");

            return Ok(tasksOfUser);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("GetAllUrgentTasksOfUserTeam")]
    public async Task<ActionResult> GeGetAllUrgentTasksOfUserTeam()
    {
        try
        {
            var user = CheckIfUserIsLoggedIn();
            if (user == null)
                return BadRequest("User must be logged in!");

            List<ToDoTask> tasksOfUser = await Context.ToDoTasks.Include(t => t.MembersOfTask).Where(t => t.OwnerOfTask == user && t.StateOfTask == StateOfTask.Urgent && t.MembersOfTask!.Count != 0).ToListAsync();

            if (tasksOfUser == null || tasksOfUser.Count == 0)
                return Ok("No urgent tasks of user!");
            return Ok(tasksOfUser);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("GetAllImportantTasksOfUserTeam")]
    public async Task<ActionResult> GetAlGetAllImportantTasksOfUserTeam()
    {
        try
        {
            var user = CheckIfUserIsLoggedIn();
            if (user == null)
                return BadRequest("User must be logged in!");

            List<ToDoTask> tasksOfUser = await Context.ToDoTasks.Include(t => t.MembersOfTask).Where(t => t.OwnerOfTask == user && t.StateOfTask == StateOfTask.Important && t.MembersOfTask!.Count != 0).ToListAsync();
            if (tasksOfUser == null || tasksOfUser.Count == 0)
                return Ok("No important tasks for user!");
            return Ok(tasksOfUser);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("GetAllNextDayTasksOfUserTeam")]
    public async Task<ActionResult> GetGetAllNextDayTasksOfUserTeam()
    {
        try
        {
            var user = CheckIfUserIsLoggedIn();
            if (user == null)
                return BadRequest("User must be logged in!");

            List<ToDoTask> tasksOfUser = await Context.ToDoTasks.Include(t => t.MembersOfTask).Where(t => t.OwnerOfTask == user && t.StateOfTask == StateOfTask.NextDay && t.MembersOfTask!.Count != 0).ToListAsync();

            if (tasksOfUser == null || tasksOfUser.Count == 0)
                return Ok("No tasks for next days!");
            return Ok(tasksOfUser);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("GetAllDoneTasksOfUserTeam")]
    public async Task<ActionResult> GetAllDoneTasksOfUserTeam()
    {
        try
        {
            var user = CheckIfUserIsLoggedIn();

            if (user == null)
                return BadRequest("User has to be logged in!");

            List<ToDoTask> tasks = await Context.ToDoTasks.Include(t => t.MembersOfTask).Where(t => t.OwnerOfTask == user && t.StateOfTask == StateOfTask.Done && t.MembersOfTask!.Count != 0).ToListAsync();

            if (tasks == null)
                return BadRequest("Error with getting finished tasks!");

            if (tasks.Count == 0)
                return Ok("User has no done tasks!");

            return Ok(tasks);
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

    [HttpGet("GetAllUrgentTasksUserIsMemberOf")]
    public async Task<ActionResult> GetAllUrgentTasksUserIsMemberOf()
    {
        try
        {
            var user = CheckIfUserIsLoggedIn();

            if (user == null)
                return BadRequest("User has to be logged in!");

            List<ToDoTask> tasks = await Context.Members.Where(u => u.Member == user && u.Task.StateOfTask == StateOfTask.Urgent).Include(t => t.Task).Select(t => t.Task).ToListAsync();

            if (tasks == null)
                return BadRequest("Error with getting urgent tasks!");

            if (tasks.Count == 0)
                return Ok("List of urgent tasks!");

            return Ok(tasks);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("GetAllNextDayTasksUserIsMemberOf")]

    public async Task<ActionResult> GetAllNextDayTasksUserIsMemberOf()
    {
        try
        {
            var user = CheckIfUserIsLoggedIn();

            if (user == null)
                return BadRequest("User has to be logged in!");

            List<ToDoTask> tasks = await Context.Members.Where(u => u.Member == user && u.Task.StateOfTask == StateOfTask.NextDay).Include(t => t.Task).Select(t => t.Task).ToListAsync();

            if (tasks == null)
                return BadRequest("Error with getting tasks for next day");

            if (tasks.Count == 0)
                return Ok("No tasks for next day");

            return Ok(tasks);

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("GetAllImportantTasksUserIsMemberOf")]
    public async Task<ActionResult> GetAllImportantTasksUserIsMemeberOf()
    {
        try
        {
            var user = CheckIfUserIsLoggedIn();

            if (user == null)
                return BadRequest("User has to be logged in!");

            List<ToDoTask> tasks = await Context.Members.Where(u => u.Member == user && u.Task.StateOfTask == StateOfTask.Important).Include(t => t.Task).Select(t => t.Task).ToListAsync();

            if (tasks == null)
                return BadRequest("Error with getting important tasks user is member of!");

            if (tasks.Count == 0)
                return Ok("No important tasks!");

            return Ok(tasks);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("GetAllDoneTasksUserIsMemberOf")]
    public async Task<ActionResult> GetAllDoneTasksUserIsMemeberOf()
    {
        try
        {
            var user = CheckIfUserIsLoggedIn();

            if (user == null)
                return BadRequest("User has to be logged in!");

            List<ToDoTask> tasks = await Context.Members.Where(u => u.Member == user && u.Task.StateOfTask == StateOfTask.Done).Include(t => t.Task).Select(t => t.Task).ToListAsync();

            if (tasks == null)
                return BadRequest("Error with getting finished tasks!");

            if (tasks.Count == 0)
                return Ok("No finished tasks");

            return Ok(tasks);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpGet("GetAllTasksUserIsMemberOf")]
    public async Task<ActionResult> GetAllTasksUserIsMemeberOf()
    {
        try
        {
            var user = CheckIfUserIsLoggedIn();

            if (user == null)
                return BadRequest("User has to be logged in!");

            List<ToDoTask> tasks = await Context.Members.Where(u => u.Member == user).Include(t => t.Task).ThenInclude(u => u.OwnerOfTask).Select(t => t.Task).ToListAsync();

            if (tasks == null)
                return BadRequest("Error with getting tasks user is member of!");

            if (tasks.Count == 0)
                return BadRequest("User is not memeber of any tasks!");

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

            if (loggedInUser != task.OwnerOfTask)
                return BadRequest("You are not owner of task so you can't change anything!");

            if (Emails == null || Emails.Count == 0)
                return BadRequest("No members added to the task.");

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

    [HttpPut("RemoveMemberFromTask/{Email}/{TaskName}")]
    public async Task<ActionResult> RemoveMemberFromTask(string Email, string TaskName)
    {
        try
        {
            var user = CheckIfUserIsLoggedIn();

            if (user == null)
                return BadRequest("User has to be logged in!");

            if (string.IsNullOrEmpty(Email))
                return BadRequest("You have to select email!");

            if (string.IsNullOrEmpty(TaskName))
                return BadRequest("You have to provide name of task");

            var userToRemove = await Context.Users.Where(u => u.Email.Equals(Email)).Include(u => u.UserMemeberOfTasks).FirstOrDefaultAsync();

            if (userToRemove == null)
                return BadRequest("No user with provided email found");

            var task = await Context.ToDoTasks.Where(t => t.TaskName.Equals(TaskName)).Include(t => t.MembersOfTask).FirstOrDefaultAsync();

            if (task == null)
                return BadRequest("Task with provided name not found");

            var membersClass = await Context.Members.Where(u => u.Member == userToRemove && u.Task == task).FirstOrDefaultAsync();

            if (membersClass == null)
                return BadRequest("That member is not a member of this task");

            userToRemove.UserMemeberOfTasks.Remove(membersClass);
            task.MembersOfTask.Remove(membersClass);

            Context.Members.Remove(membersClass);
            await Context.SaveChangesAsync();

            return Ok("Successfully removed member from this task");
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

            if (user != task.OwnerOfTask)
                return BadRequest("You are not owner of task so you can't change anything!");

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