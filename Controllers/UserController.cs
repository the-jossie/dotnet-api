using Api_Tutorial.Data;
using Api_Tutorial.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api_Tutorial.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{

    DapperDataContext _dapper;
    public UserController(IConfiguration config)
    {
        _dapper = new DapperDataContext(config);
    }

    [HttpGet("")]
    public IEnumerable<User> GetUsers()
    {

        string sql = @"
            SELECT [UserId],
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active]
            FROM TutorialAppSchema.Users;";

        IEnumerable<User> users = _dapper.LoadData<User>(sql);

        return users;
    }

    [HttpGet("{userId}")]
    public User GetUser(int userId)
    {

        string sql = @"
            SELECT [UserId],
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active]
            FROM TutorialAppSchema.Users
            WHERE UserId = " + userId.ToString();

        User user = _dapper.LoadSingleData<User>(sql);

        return user;
    }
}
