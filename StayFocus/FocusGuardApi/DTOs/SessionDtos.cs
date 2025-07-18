using System;
using System.ComponentModel.DataAnnotations;

namespace FocusGuardApi.DTOs
{
    public class SessionCreateDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Range(1, 480)] // Max 8 hours in minutes
        public int PlannedDurationMinutes { get; set; } = 25;
    }

    public class SessionUpdateDto
    {
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Range(1, 480)] // Max 8 hours in minutes
        public int? PlannedDurationMinutes { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }
    }

    public class SessionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int PlannedDurationMinutes { get; set; }
        public int? ActualDurationMinutes { get; set; }
        public bool IsCompleted { get; set; }
        public string Notes { get; set; }
    }

    public class SessionEndDto
    {
        [StringLength(500)]
        public string Notes { get; set; }
    }
}
