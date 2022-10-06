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
        public async Task<Users> AddAsync(UserRegisterModel Item)
        {
            Guid NewGuid = Guid.Empty;         
            Users DapUser = new Users();

            DapUser.LoginName = Item.LoginName;
            DapUser.Firstname = Item.Firstname;
            DapUser.Lastname = Item.Lastname;
            DapUser.PasswordHash = Item.PasswordHash;
            DapUser.Email = Item.Email;
            DapUser.Active=true;

            string Fields = $"INSERT INTO {TableName} (,)  Output Inserted.ID ";
            string Values = " Values(,)";
            var Param = new DynamicParameters();

            CreateInsert<Users>(DapUser, ref Fields, ref Values, ref Param, false);

            DapUser.ID = await _db.ExecuteScalarAsync<Guid>(Fields + Values, Param, commandType: CommandType.Text).ConfigureAwait(false);
            return DapUser;
        }

        public async Task<Users> GetByGuidAsync(Guid ID)
        {
            if (ID == Guid.Empty)
                throw new ArgumentNullException("Invalid request");
            string SQL = $"select * from {TableName} Where {IDField}=@ID";
            var Results = await _db.QueryAsync<Users>(SQL, new
            {
                Id = ID
            }, null, null, CommandType.Text).ConfigureAwait(false);
            var dbroles = new Roles_TSQL(conn);
            var user = Results.First();
            user.roles = dbroles.GetByUserGuidAsync(ID).Result;
            return user;
        }
        public async Task<Users> GetByLoginNameAsync(String LoginName)
        {
            if (LoginName == String.Empty)
                throw new ArgumentNullException("Invalid request");
            string SQL = $"select * from {TableName} Where LoginName=@LoginName";
            var Results = await _db.QueryAsync<Users>(SQL, new
            {
                LoginName = LoginName
            }, null, null, CommandType.Text).ConfigureAwait(false);
            var user = Results.First();
            var dbroles = new Roles_TSQL(conn);
            user.roles = dbroles.GetByUserGuidAsync(user.ID).Result;
            return user;
        }

        public async Task<List<Users>> GetAllAsync()
        {
            var userlist = new List<Users>();
            string SQL = $"select * from {TableName}";
            var Result = await _db.QueryAsync<Users>(SQL, CommandType.Text).ConfigureAwait(false);
            foreach (var item in Result.ToList())
            {
                var dbroles = new Roles_TSQL(conn);
                item.roles = dbroles.GetByUserGuidAsync(item.ID).Result;
                userlist.Add(item);
            }
            return userlist;
        }
        public async Task<List<Users>> GetAllActiveAsync()
        {
            string SQL = $"select * from {TableName} Where Active=1";
            var results = await _db.QueryAsync<Users>(SQL, CommandType.Text).ConfigureAwait(false);
            return results.ToList();
        }
        private void CreateInsert<T>(T Source, ref string Fields, ref string Values, ref DynamicParameters param, bool IncludeNulls = false)
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            var tT = typeof(T);
            var properties = tT.GetProperties().Where(prop => prop.CanRead && prop.CanWrite
            && prop.Name.ToUpper() != "PASSWORDHASH" && prop.Name.ToUpper() != IDField);

            foreach (var prop in properties)
            {
                var Svalue = prop.GetValue(Source, null);
                if (!(Svalue == null))
                {
                    Fields = Fields.Replace(",)", prop.Name.Trim() + ",,)");
                    Values = Values.Replace(",)", "@" + prop.Name.Trim() + ",,)");
                    param.Add("@" + prop.Name, Svalue);
                }
                else if (IncludeNulls)
                {
                    Fields = Fields.Replace(",)", prop.Name.Trim() + ",,)");
                    Values = Values.Replace(",)", "@" + prop.Name.Trim() + ",,)");
                    param.Add("@" + prop.Name, null);
                }
            }
            Fields = Fields.Replace(",,)", ") ");
            Values = Values.Replace(",,)", ") ");
#pragma warning restore CS8601 // Possible null reference assignment.
        }
        private void CreateUpdate<T>(T Source, ref string Query, ref DynamicParameters param, bool IncludeNulls = true)
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            var tT = typeof(T);
            var properties = tT.GetProperties().Where(prop => prop.CanRead && prop.CanWrite
            && prop.Name.ToUpper() != "PASSWORDHASH" && prop.Name.ToUpper() != IDField);

            foreach (var prop in properties)
            {
                var Svalue = prop.GetValue(Source, null);
                if (!(Svalue == null))
                {
                    Query += "," + prop.Name + "=@" + prop.Name;
                    param.Add("@" + prop.Name, Svalue);
                }
                else if (IncludeNulls)
                {
                    Query += "," + prop.Name + "=@" + prop.Name;
                    param.Add("@" + prop.Name, default);
                }
            }
            Query = Query.Replace("Set ,", "Set ");
            Query = Query.Replace("Set ,", "Set ");
            Query += $" where {IDField}=@{IDField}";
        }
    }
}
