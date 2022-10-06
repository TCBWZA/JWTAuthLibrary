namespace JWTAuthLibrary
{
    public interface IUsers
    {
        public Task<List<Users>> GetAllAsync();
        public Task<List<Users>> GetAllActiveAsync();
        public Task<Users> GetByGuidAsync(Guid ID);
        public Task<Users> GetByLoginNameAsync(String LoginName);
        public void DeleteAsync(Guid ID);
        public void DeleteAsync(UserLoginModel Item);
        public string GetConnectionString();
    }
}
