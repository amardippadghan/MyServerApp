namespace MyServerApp.Models
{
    public class AppSettings
    {
        public AlertSettings Alerts { get; set; } = new();
        public AssetSettings Assets { get; set; } = new();
        public ZoneSettings Zones { get; set; } = new();
    }

    public class AlertSettings
    {
        public bool AutoGenerateRestrictedZoneAlerts { get; set; } = true;
        public bool AutoGenerateCapacityAlerts { get; set; } = true;
        public int MaxAlertsPerAsset { get; set; } = 100;
        public int AlertRetentionDays { get; set; } = 90;
    }

    public class AssetSettings
    {
        public bool RequireAssetCode { get; set; } = false;
        public bool AllowDuplicateAssetCodes { get; set; } = false;
        public string DefaultAssetType { get; set; } = "Equipment";
        public int AssetHistoryRetentionDays { get; set; } = 365;
    }

    public class ZoneSettings
    {
        public bool EnforceCapacityLimits { get; set; } = false;
        public double CapacityWarningThreshold { get; set; } = 0.8; // 80%
        public bool AllowMovementToInactiveZones { get; set; } = false;
    }
}
