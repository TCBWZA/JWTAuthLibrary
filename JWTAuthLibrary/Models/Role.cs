using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace JWTAuthLibrary
{
    [Table("Roles")]
    public class Roles
    {
        [Key()]
        [Column("ID")]
        public Guid Id { get; set; }

        [Column("Name")]
        [MaxLength(50)]
        [Required]
        public string Name { get; set; }

        [Column("Active")]
        public bool IsActive { get; set; } = true;

        public List<Users> Users { get; } = new List<Users>();
    }
}
