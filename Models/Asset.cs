using System.ComponentModel.DataAnnotations;

namespace MyServerApp.Models
{
    public class Asset : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string? AssetCode { get; set; }

        [Required]
        public int ZoneId { get; set; }

        public string? Description { get; set; }

        [StringLength(50)]
        public string? AssetType { get; set; }

        [StringLength(100)]
        public string? SerialNumber { get; set; }

        public DateTime? PurchaseDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? PurchaseValue { get; set; }

        // Navigation properties
        public virtual Zone? Zone { get; set; }
        public virtual ICollection<AssetLog> AssetLogs { get; set; } = new List<AssetLog>();
        public virtual ICollection<Alert> Alerts { get; set; } = new List<Alert>();

        // Computed properties
        public bool IsInRestrictedZone => Zone?.IsRestricted ?? false;
        public string CurrentZoneName => Zone?.Name ?? "Unknown";
        public string CurrentZoneTypeName => Zone?.ZoneType?.Name ?? "Unknown";
        public int AssetAge => PurchaseDate.HasValue
            ? DateTime.Now.Year - PurchaseDate.Value.Year
            : 0;
    }

    // Enum for common asset types
    public enum AssetType
    {
        Equipment,
        Material,
        ITEquipment,
        SecurityEquipment,
        Tools,
        Vehicle,
        Furniture,
        Other
    }
}
