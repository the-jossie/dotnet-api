using Api_Tutorial.Models;

namespace Api_Tutorial.Data
{
    public class UserRepository: IUserRepository
    {

        EFDataContext _entityFramework;
        public UserRepository(IConfiguration config)
        {
            _entityFramework = new EFDataContext(config);
        }

        public bool SaveChanges()
        {
            return _entityFramework.SaveChanges() > 0;
        }

        public void AddEntity<T>(T entity)
        {
            if (entity != null)
            {
                _entityFramework.Add(entity);
            }
        }


        public void RemoveEntity<T>(T entity)
        {
            if (entity != null)
            {
                _entityFramework.Remove(entity);
            }
        }

        public IEnumerable<User> GetUsers()
    {
        IEnumerable<User> users = _entityFramework.Users.ToList<User>();

        return users;
    }

    public User GetUser(int userId)
    {
        User? user = _entityFramework.Users.Where(u => u.UserId == userId).FirstOrDefault<User>();

        if (user != null)
        {
            return user;
        }

        throw new Exception("Failed to Get User");
    }

    public UserSalary GetUserSalary(int userId)
    {
        UserSalary? userSalary = _entityFramework.UserSalary.Where(u => u.UserId == userId).FirstOrDefault<UserSalary>();

        if (userSalary != null)
        {
            return userSalary;
        }

        throw new Exception("Failed to Get User Salary");
    }
    }
}
