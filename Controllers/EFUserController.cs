using Api_Tutorial.Data;
using Api_Tutorial.Dtos;
using Api_Tutorial.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Api_Tutorial.Controllers;

[ApiController]
[Route("[controller]")]
public class EFUserController : ControllerBase
{
    EFDataContext _entityFramework;
    IMapper _mapper;
    public EFUserController(IConfiguration config)
    {
        _entityFramework = new EFDataContext(config);

        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserDto, User>();
        }));
    }

    [HttpGet("")]
    public IEnumerable<User> GetUsers()
    {
        IEnumerable<User> users = _entityFramework.Users.ToList<User>();

        return users;
    }

    [HttpGet("{userId}")]
    public User GetUser(int userId)
    {
        User? user = _entityFramework.Users.Where(u => u.UserId == userId).FirstOrDefault<User>();

        if (user != null)
        {
            return user;
        }

        throw new Exception("Failed to Get User");
    }

    [HttpPut]
    public IActionResult EditUser(User user)
    {
        User? userFromDb = _entityFramework.Users.Where(u => u.UserId == user.UserId).FirstOrDefault<User>();

        if (userFromDb != null)
        {
            userFromDb.Active = user.Active;
            userFromDb.Email = user.Email;
            userFromDb.FirstName = user.FirstName;
            userFromDb.LastName = user.LastName;
            userFromDb.Gender = user.Gender;

            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }

            throw new Exception("Failed to Update User");
        }

        throw new Exception("Failed to Update User");
    }

    [HttpPost]
    public IActionResult AddUser(UserDto user)
    {
        User? userFromDb = _mapper.Map<User>(user);

        _entityFramework.Add(userFromDb);

        if (_entityFramework.SaveChanges() > 0)
        {
            return Ok();
        }


        throw new Exception("Failed to Add User");
    }


    [HttpDelete("{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        User? userFromDb = _entityFramework.Users.Where(u => u.UserId == userId).FirstOrDefault<User>();

        if (userFromDb != null)
        {
            _entityFramework.Users.Remove(userFromDb);

            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }

            throw new Exception("Failed to Delete User");
        }

        throw new Exception("Failed to Delete User");
    }

    [HttpGet("salary/{userId}")]
    public UserSalary GetUserSalary(int userId)
    {
        UserSalary? userSalary = _entityFramework.UserSalary.Where(u => u.UserId == userId).FirstOrDefault<UserSalary>();

        if (userSalary != null)
        {
            return userSalary;
        }

        throw new Exception("Failed to Get User Salary");
    }

    [HttpPost("salary")]
    public IActionResult AddUserSalary(UserSalary userSalary)
    {
        _entityFramework.UserSalary.Add(userSalary);

        if (_entityFramework.SaveChanges() > 0)
        {
            return Ok();
        }

        throw new Exception("Failed to Add User Salary");
    }

    [HttpPut("salary")]
    public IActionResult EditUserSalary(UserSalary userSalary)
    {
        UserSalary? userSalaryFromDb = _entityFramework.UserSalary.Where(u => u.UserId == userSalary.UserId).FirstOrDefault<UserSalary>();

        if (userSalaryFromDb != null)
        {
            userSalaryFromDb.Salary = userSalary.Salary;

            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }

            throw new Exception("Failed to Update User Salary");
        }

        throw new Exception("Failed to Find User Salary");
    }

    [HttpDelete("salary/{userId}")]
    public IActionResult DeleteUserSalary(int userId)
    {
        UserSalary? userSalaryFromDb = _entityFramework.UserSalary.Where(u => u.UserId == userId).FirstOrDefault<UserSalary>();

        if (userSalaryFromDb != null)
        {
            _entityFramework.UserSalary.Remove(userSalaryFromDb);

            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }

            throw new Exception("Failed to Delete User Salary");
        }

        throw new Exception("Failed to Find User Salary");
    }
}
