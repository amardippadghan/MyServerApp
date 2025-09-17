using System.ComponentModel.DataAnnotations;

namespace MyServerApp.Models.DTOs.ZoneType
{
    public class CreateZoneTypeDto
    {
        [Required(ErrorMessage = "Zone type name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zone type code is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Code must be between 2 and 50 characters")]
        [RegularExpression(@"^[A-Z_]+$", ErrorMessage = "Code must contain only uppercase letters and underscores")]
        public string Code { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Range(0, 1, ErrorMessage = "IsRestricted must be 0 (Normal) or 1 (Restricted)")]
        public int IsRestricted { get; set; } = 0;

        [Required(ErrorMessage = "Color is required")]
        [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Color must be a valid hex color (e.g., #FF5733)")]
        public string Color { get; set; } = "#0066cc";

        [Range(0, 10, ErrorMessage = "Priority must be between 0 and 10")]
        public int Priority { get; set; } = 0;
    }
}
