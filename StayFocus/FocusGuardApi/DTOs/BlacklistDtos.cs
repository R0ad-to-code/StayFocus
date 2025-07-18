using System.ComponentModel.DataAnnotations;

namespace FocusGuardApi.DTOs
{
    public class BlacklistItemCreateDto
    {
        [Required]
        [Url]
        [MaxLength(255)]
        public string Url { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Reason { get; set; }
    }

    public class BlacklistItemUpdateDto
    {
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Reason { get; set; }

        public bool? IsActive { get; set; }
    }

    public class BlacklistItemDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
