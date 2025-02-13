using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using MyTaskManager.Auth;
using Microsoft.AspNetCore.Authorization;
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

    // [HttpGet("")]
}