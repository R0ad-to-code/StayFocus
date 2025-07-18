using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FocusGuardApi.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLogin { get; set; }

        // Navigation properties
        public virtual ICollection<Session> Sessions { get; set; }
        public virtual ICollection<BlacklistItem> BlacklistedItems { get; set; }
    }
}
