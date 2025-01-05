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
    }
}
