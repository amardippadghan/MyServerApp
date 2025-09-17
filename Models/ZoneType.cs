using System.ComponentModel.DataAnnotations;

namespace MyServerApp.Models
{
    public class ZoneType : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int IsRestricted { get; set; } = 0; // 1 = Restricted, 0 = Normal

        [StringLength(7)]
        [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$")]
        public string Color { get; set; } = "#0066cc";

        public int Priority { get; set; } = 0;

        // Navigation properties
        public virtual ICollection<Zone> Zones { get; set; } = new List<Zone>();

        // Computed properties
        public bool IsRestrictedZone => IsRestricted == 1;
        public string RestrictedStatus => IsRestricted == 1 ? "Restricted" : "Normal";
    }

    // Enum for common zone type codes (for reference, but system uses dynamic database values)
    public enum ZoneTypeCode
    {
        WAREHOUSE,
        PRODUCTION,
        RESTRICTED,
        QC,
        SHIPPING,
        MAINTENANCE,
        OFFICE,
        SECURITY
    }
}
