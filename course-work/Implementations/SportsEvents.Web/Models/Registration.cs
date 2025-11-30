using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SportsEvents.Web.Models
{
    public class Registration
    {
        public int Id { get; set; }

        public int SportEventId { get; set; }

        [ValidateNever] 
        public SportEvent? SportEvent { get; set; }

        public int ParticipantId { get; set; }

        [ValidateNever] 
        public Participant? Participant { get; set; }

        [Required]
        public DateTime RegisteredAt { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; }

        public bool IsPaid { get; set; }

        [Range(0, 100000)]
        public decimal PaidAmount { get; set; }

        [MaxLength(200)]
        public string? Notes { get; set; }
    }
}
