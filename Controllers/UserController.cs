using Api_Tutorial.Data;
using Api_Tutorial.Dtos;
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

    [HttpPut]
    public IActionResult EditUser(User user)
    {

        string sql = @"
            UPDATE TutorialAppSchema.Users
            SET [FirstName] = '" + user.FirstName +
            "', [LastName] = '" + user.LastName +
            "', [Email] = '" + user.Email +
            "', [Gender] = '" + user.Gender +
            "', [Active] = '" + user.Active +
            "' WHERE UserId = " + user.UserId.ToString();

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Update User");
    }

    [HttpPost]
    public IActionResult AddUser(UserDto user)
    {
        string sql = @"
            INSERT INTO TutorialAppSchema.Users(
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active]
            ) VALUES(
                '" + user.FirstName +
                "', '" + user.LastName +
                "', '" + user.Email +
                "', '" + user.Gender +
                "', '" + user.Active +
            "')";

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Add User");
    }


    [HttpDelete("{userId}")]
    public IActionResult DeleteUser(int userId)
    {

        string sql = @"
            DELETE FROM TutorialAppSchema.Users
            WHERE UserId = " + userId.ToString()
        ;

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Delete User");
    }

    [HttpGet("salary/{userId}")]
    public UserSalary GetUserSalary(int userId)
    {

        string sql = @"
            SELECT [UserId],
                [Salary]
            FROM TutorialAppSchema.UserSalary
            WHERE UserId = " + userId.ToString();

        UserSalary userSalary = _dapper.LoadSingleData<UserSalary>(sql);

        return userSalary;
    }

    [HttpPost("salary")]
    public IActionResult AddUserSalary(UserSalary userSalary)
    {
        string sql = @"
            INSERT INTO TutorialAppSchema.UserSalary(
                [UserId],
                [Salary]
            ) VALUES(
                " + userSalary.UserId +
                ", " + userSalary.Salary +
            ")";

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Add User Salary");
    }

    [HttpPut("salary")]
    public IActionResult EditUserSalary(UserSalary userSalary)
    {
        string sql = @"
            UPDATE TutorialAppSchema.UserSalary
            SET [Salary] = " + userSalary.Salary +
            " WHERE UserId = " + userSalary.UserId;

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Update User Salary");
    }

    [HttpDelete("salary/{userId}")]
    public IActionResult DeleteUserSalary(int userId)
    {

        string sql = @"
            DELETE FROM TutorialAppSchema.UserSalary
            WHERE UserId = " + userId.ToString()
        ;

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Delete User Salary");
    }
}
