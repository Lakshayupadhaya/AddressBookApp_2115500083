using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Entity
{
    public class AddressBookEntity
    {
        [Key]
        public int Id { get; set; }

        //[Required]
        public string Name { get; set; }

        //[Required]
        //[EmailAddress]
        public string Email { get; set; }

        //[Required]
        //[MaxLength(20)]
        public string PhoneNumber { get; set; }

        //[Required]
        //[MaxLength(200)]
        public string Address { get; set; }

        //[Required]
        [ForeignKey("Users")] // ✅ Defines UserId as Foreign Key but does NOT create navigation behavior
        public int UserId { get; set; } // Still a Foreign Key in DB but no automatic joins
    }
}
