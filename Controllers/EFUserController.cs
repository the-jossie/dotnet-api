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
    public EFUserController(IConfiguration config)
    {
        _entityFramework = new EFDataContext(config);
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
        User? userFromDb = new User();

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
}
