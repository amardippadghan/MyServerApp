using System.ComponentModel.DataAnnotations;

namespace MyServerApp.Models
{
    public class Zone : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int ZoneTypeId { get; set; }

        public string? Description { get; set; }

        [StringLength(255)]
        public string? Location { get; set; }

        public int? Capacity { get; set; }

        public int CurrentAssetCount { get; set; } = 0;

        // Navigation properties
        public virtual ZoneType? ZoneType { get; set; }
        public virtual ICollection<Asset> Assets { get; set; } = new List<Asset>();
        public virtual ICollection<AssetLog> AssetLogsFrom { get; set; } = new List<AssetLog>();
        public virtual ICollection<AssetLog> AssetLogsTo { get; set; } = new List<AssetLog>();
        public virtual ICollection<Alert> Alerts { get; set; } = new List<Alert>();

        // Computed properties
        public bool IsRestricted => ZoneType?.IsRestrictedZone ?? false;
        public bool IsAtCapacity => Capacity.HasValue && CurrentAssetCount >= Capacity.Value;
        public bool IsNearCapacity => Capacity.HasValue && CurrentAssetCount >= (Capacity.Value * 0.8);
        public double CapacityUtilization => Capacity.HasValue && Capacity.Value > 0
            ? (double)CurrentAssetCount / Capacity.Value * 100
            : 0;
    }
}
