using Dapper;

using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

//using AutoMapper;
using DapperExtensions.Predicate;
using System.Xml.Linq;

namespace JWTAuthLibrary
{
    public class Users_TSQL : IUsers
    {
        private string TableName = "Users";
        private string IDField = "ID";
        private IDbConnection _db;
        private string conn;

        public Users_TSQL(string Conn)
        {
            conn = Conn;
            _db = new SqlConnection(Conn);
        }
        public string GetConnectionString()
        {
            return conn;
        }

        public async void DeleteAsync(Guid ID)
        {
            if (ID == Guid.Empty)
                throw new ArgumentNullException("Invalid request");
            string SQL = $"Delete from {TableName} Where {IDField}=@ID";
            await _db.ExecuteAsync(SQL, new { ID = ID }, null, null, CommandType.Text).ConfigureAwait(false);
        }

        public async Task<Users> GetBuGuidAsync(Guid ID)
        {
            if (ID == Guid.Empty)
                throw new ArgumentNullException("Invalid request");
            string SQL = $"select * from {TableName} Where {IDField}=@ID";
            var Results = await _db.QueryAsync<Users>(SQL, new
            {
                Id = ID
            }, null, null, CommandType.Text).ConfigureAwait(false);
            return Results.First();
        }
        public async Task<Users> GetByNameAsync(String LoginName)
        {
            if (LoginName == String.Empty)
                throw new ArgumentNullException("Invalid request");
            string SQL = $"select * from {TableName} Where LoginName=@LoginName";
            var Results = await _db.QueryAsync<Users>(SQL, new
            {
                LoginName = LoginName
            }, null, null, CommandType.Text).ConfigureAwait(false);
            return Results.First();
        }

        public async Task<List<Users>> GetAllAsync()
        {
            string SQL = $"select * from {TableName}";
            var results = await _db.QueryAsync<Users>(SQL, CommandType.Text).ConfigureAwait(false);
            return results.ToList();
        }
        public async Task<List<Users>> GetAllActiveAsync()
        {
            string SQL = $"select * from {TableName} Where Active=1";
            var results = await _db.QueryAsync<Users>(SQL, CommandType.Text).ConfigureAwait(false);
            return results.ToList();
        }
    }
}
