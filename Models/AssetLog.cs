using System.ComponentModel.DataAnnotations;

namespace MyServerApp.Models
{
    public class AssetLog : BaseEntity
    {
        [Required]
        public int AssetId { get; set; }

        public int? FromZoneId { get; set; }

        [Required]
        public int ToZoneId { get; set; }

        [Required]
        [StringLength(50)]
        public string MovementType { get; set; } = "TRANSFER";

        [Required]
        [StringLength(50)]
        public string ShiftTime { get; set; } = string.Empty;

        [StringLength(100)]
        public string? MovedBy { get; set; }

        public string? Reason { get; set; }

        public string? Notes { get; set; }

        // Navigation properties
        public virtual Asset? Asset { get; set; }
        public virtual Zone? FromZone { get; set; }
        public virtual Zone? ToZone { get; set; }

        // Computed properties
        public bool IsInitialCheckIn => FromZoneId == null;
        public bool IsTransferBetweenZones => FromZoneId.HasValue && ToZoneId != FromZoneId;
        public string MovementDescription => FromZoneId.HasValue
            ? $"Moved from {FromZone?.Name} to {ToZone?.Name}"
            : $"Checked into {ToZone?.Name}";
    }

    // Enum for movement types
    public enum MovementType
    {
        TRANSFER,
        CHECKIN,
        CHECKOUT,
        MAINTENANCE,
        INSPECTION,
        RETURN,
        LOAN,
        DISPOSAL,
        AUDIT
    }

    // Enum for shift times
    public enum ShiftTime
    {
        Morning,
        Evening,
        Night,
        Weekend,
        Holiday
    }
}
