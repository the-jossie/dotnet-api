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
    IUserRepository _userRepository;
    IMapper _mapper;
    public EFUserController(IConfiguration config, IUserRepository userRepository)
    {
        _userRepository = userRepository;

        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserDto, User>();
        }));
    }

    [HttpGet("")]
    public IEnumerable<User> GetUsers()
    {
        IEnumerable<User> users = _userRepository.GetUsers();

        return users;
    }

    [HttpGet("{userId}")]
    public User GetUser(int userId)
    {
        return _userRepository.GetUser(userId);
    }

    [HttpPut]
    public IActionResult EditUser(User user)
    {
        User? userFromDb = _userRepository.GetUser(user.UserId);

        if (userFromDb != null)
        {
            userFromDb.Active = user.Active;
            userFromDb.Email = user.Email;
            userFromDb.FirstName = user.FirstName;
            userFromDb.LastName = user.LastName;
            userFromDb.Gender = user.Gender;

            if (_userRepository.SaveChanges())
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

        _userRepository.AddEntity<User>(userFromDb);

        if (_userRepository.SaveChanges())
        {
            return Ok();
        }


        throw new Exception("Failed to Add User");
    }


    [HttpDelete("{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        User? userFromDb = _userRepository.GetUser(userId);

        if (userFromDb != null)
        {
            _userRepository.RemoveEntity<User>(userFromDb);

            if (_userRepository.SaveChanges())
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
        return _userRepository.GetUserSalary(userId);
    }

    [HttpPost("salary")]
    public IActionResult AddUserSalary(UserSalary userSalary)
    {
        _userRepository.AddEntity<UserSalary>(userSalary);

        if (_userRepository.SaveChanges())
        {
            return Ok();
        }

        throw new Exception("Failed to Add User Salary");
    }

    [HttpPut("salary")]
    public IActionResult EditUserSalary(UserSalary userSalary)
    {
        UserSalary? userSalaryFromDb = _userRepository.GetUserSalary(userSalary.UserId);

        if (userSalaryFromDb != null)
        {
            userSalaryFromDb.Salary = userSalary.Salary;

            if (_userRepository.SaveChanges())
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
        UserSalary? userSalaryFromDb = _userRepository.GetUserSalary(userId);

        if (userSalaryFromDb != null)
        {
            _userRepository.RemoveEntity<UserSalary>(userSalaryFromDb);

            if (_userRepository.SaveChanges())
            {
                return Ok();
            }

            throw new Exception("Failed to Delete User Salary");
        }

        throw new Exception("Failed to Find User Salary");
    }
}
