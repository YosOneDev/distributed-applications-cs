using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SportsEvents.Web.Models
{
    public class SportEvent
    {
        public int Id { get; set; } 

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }  

        [Required]
        [MaxLength(50)]
        public string SportType { get; set; }  

        [Required]
        [MaxLength(100)]
        public string Location { get; set; }  

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Range(0, 10000)]
        public int MaxParticipants { get; set; }

        [Range(0, 100000)]
        public decimal ParticipationFee { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();

        public ICollection<EventImage> Images { get; set; } = new List<EventImage>();

    }
}
