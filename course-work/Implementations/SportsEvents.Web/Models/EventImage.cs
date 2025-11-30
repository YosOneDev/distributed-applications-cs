using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SportsEvents.Web.Models
{
    public class EventImage
    {
        public int Id { get; set; }

        [Required]
        public int SportEventId { get; set; }

        public SportEvent SportEvent { get; set; }

        [Required]
        [MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        public string UploaderUserId { get; set; } = string.Empty;

        public bool IsApproved { get; set; } = false;

        public DateTime UploadedAt { get; set; } = DateTime.Now;
    }
}
