﻿namespace JWTAuthLibrary
{
    public interface IRoles
    {
        public Task<List<Roles>> GetAllAsync();
        public Task<List<Roles>> GetAllActiveAsync();
        public Task<Roles> GetByGuidAsync(Guid ID);
        public Task<List<Roles>> GetByLoginNameAsync(string LoginName);
        public void DeleteAsync(Guid ID);
        public void InsertAsync(Roles Item);
        public string GetConnectionString();
    }
}
