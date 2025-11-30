using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SportsEvents.Web.Models
{
    public class Participant
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [Range(5, 100)]
        public int Age { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        public DateTime RegisteredOn { get; set; }

        [MaxLength(50)]
        public string City { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    }
}
