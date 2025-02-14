using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models;

namespace Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    public Context Context;
    private IConfiguration configuration;

    public LoginController(Context context, IConfiguration config)
    {
        Context = context;
        configuration = config;
    }

    [HttpGet("LoginUser/{Email}/{Password}")]
    public async Task<ActionResult> LoginUser(string Email, string Password)
    {
        try
        {
            var user = await Context.Users.Where(u => u.Email == Email).FirstOrDefaultAsync();

            if (user == null)
                return BadRequest("Error, Wrong Email!");

            if (VerifyPassword(Password, user.Password, user.Salt))
                return Ok(user);
            else
                return BadRequest("Error, Wrong Password!");
        }

        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

    }

    private bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
    {
        if (password == null) throw new ArgumentNullException("password");
        if (String.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or be white space", "password");
        if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash", "passwordHash");
        if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt", "passwordHash");

        using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
        {
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            for (int i = 0; i < computedHash.Length; i++)
                if (computedHash[i] != storedHash[i]) return false;
        }

        return true;
    }

    [HttpPost("SignUp/{Username}/{Email}/{Password}/{ConfirmPassword}")]

    public async Task<ActionResult> SignUp(string Username, string Email, string Password, string ConfirmPassword)
    {
        try
        {

            if (string.IsNullOrWhiteSpace(Username) && Username.Length > 20)
                return BadRequest("Pick different Username!");

            if (string.IsNullOrWhiteSpace(Email))
                return BadRequest("Enter Email!");

            if (string.IsNullOrWhiteSpace(Password) && !Password.Any(char.IsUpper) && !Password.Any(ch => !char.IsLetterOrDigit(ch)) && Password.Length < 10)
                return BadRequest("Enter new password!");

            if (Password != ConfirmPassword)
                return BadRequest("Passwords not matching!");

            foreach (var u in Context.Users.ToList())
            {
                if (u.UserName.CompareTo(Username) == 0)
                    return BadRequest("Already existing!");

                if (u.Email.CompareTo(Email) == 0)
                    return BadRequest("Already existing!");
            }

            User user = new User
            {
                UserName = Username,
                Email = Email
            };

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(Password, out passwordHash, out passwordSalt);
            user.Password = passwordHash;
            user.Salt = passwordSalt;

            await Context.Users.AddAsync(user);
            await Context.SaveChangesAsync();

            var verify = await Context.Users.Where(p => p.Email == Email).FirstOrDefaultAsync();

            try
            {
                Verification(verify);
            }
            catch (Exception)
            {
                return BadRequest("Email address is not valid");
            }

            return Ok(null);

        }

        catch (Exception e)
        {

            return BadRequest(e.Message);

        }
    }
    private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        if (password == null) throw new ArgumentNullException("password");
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or white space");

        using (var hmac = new System.Security.Cryptography.HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }

    private static async void Verification(User user)
    {
        string message;
        message = $"{user.UserName} \n Welcome in Sparkle Nest.\n\n With respect, \n Sparkle Nest";

        SmtpClient Client = new SmtpClient()
        {
            Host = "smtp.outlook.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential()
            {
                UserName = "sparklenest2001@outlook.com",
                Password = "Sparklenest1808"
            }
        };

        MailAddress fromMail = new MailAddress("sparklenest2001@outlook.com", "Sparkle Nest");
        MailAddress toMail = new MailAddress(user.Email, user.UserName);
        MailMessage mesg = new MailMessage()
        {
            From = fromMail,
            Subject = "Welcoming mail",
            Body = message
        };

        mesg.To.Add(toMail);
        await Client.SendMailAsync(mesg);
    }

    [HttpPost("GetToken")]
    public async Task<ActionResult> GetToken([FromBody] UserAuth user)
    {
        try
        {
            var u = await Context.Users.Where(p => p.Email == user.Email).FirstOrDefaultAsync();

            if (u == null)
                return BadRequest(null);

            if (VerifyPassword(user.Password, u.Password, u.Salt))
            {
                var token = Generate(u);
                return Ok(new { token = token });
            }
            else
            {
                return Ok(null);
            }
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    private object Generate(User p)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]{
            new Claim(ClaimTypes.NameIdentifier,p.UserName),
            new Claim(ClaimTypes.Email,p.Email),
            new Claim(ClaimTypes.Sid,p.ID.ToString())
        };

        var token = new JwtSecurityToken(configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddHours(5),
                signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}