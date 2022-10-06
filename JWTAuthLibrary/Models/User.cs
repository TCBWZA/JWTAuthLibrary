using System.ComponentModel.DataAnnotations;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace JWTAuthLibrary
{
    [Table("Users")]
    public partial class Users
    {
        [Key()]
        [Column("ID")]
        public Guid ID { get; set; }

        [Column("LoginName")]
        [MaxLength(50)]
        [Required]
        public String LoginName { get; set; }

        [Column("Firstname")]
        [MaxLength(50)]
        [Required]
        public String Firstname { get; set; }

        [Column("Lastname")]
        [MaxLength(50)]
        [Required]
        public String Lastname { get; set; }

        [Column("PasswordHash")]
        [MaxLength(200)]
        [JsonIgnore]
        [Required]
        public String PasswordHash { get; set; }

        [Column("Active")]
        public Boolean Active { get; set; }=true;

        [Column("Email")]
        [MaxLength(250)]
        public String? Email { get; set; }
        public List<Roles> roles { get; } = new List<Roles>();
    }

    public class UserLoginModel
    {
        [Required(ErrorMessage = "UserName is required.")]
        public string LoginName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [JsonIgnore]
        public String PasswordHash
        {
            get { return GetPasswordHash(Password); }
        }
        private static String GetPasswordHash(string value)
        {
            using SHA512 sha512Hash = SHA512.Create();
            return System.Text.Encoding.UTF8.GetString(sha512Hash.ComputeHash(Encoding.UTF8.GetBytes(value)));
        }
    }

    public class UserRegisterModel
    {
        [Column("LoginName")]
        [MaxLength(50)]
        [Required(ErrorMessage = "Login Name is required.")]
        public String LoginName { get; set; }

        [Column("Firstname")]
        [MaxLength(50)]
        [Required(ErrorMessage = "Firstname is required.")]
        public String Firstname { get; set; }

        [Column("Lastname")]
        [MaxLength(50)]
        [Required(ErrorMessage = "Lastname is required.")]
        public String Lastname { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
        public String? Email { get; set; }
        [JsonIgnore]
        public String PasswordHash
        {
            get { return GetPasswordHash(Password); }
        }
        private static String GetPasswordHash(string value)
        {
            using SHA512 sha512Hash = SHA512.Create();
            return System.Text.Encoding.UTF8.GetString(sha512Hash.ComputeHash(Encoding.UTF8.GetBytes(value)));
        }

    }
}
