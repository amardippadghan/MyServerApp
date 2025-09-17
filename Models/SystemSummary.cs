namespace MyServerApp.Models
{
    public class SystemSummary
    {
        public int TotalZoneTypes { get; set; }
        public int TotalZones { get; set; }
        public int TotalAssets { get; set; }
        public int ActiveAssets { get; set; }
        public int TotalMovements { get; set; }
        public int UnreadAlerts { get; set; }
        public int RestrictedZones { get; set; }
        public int AssetsInRestrictedZones { get; set; }
        public Dictionary<string, int> AssetsByZoneType { get; set; } = new();
        public Dictionary<string, int> AlertsBySeverity { get; set; } = new();
        public List<string> RecentMovements { get; set; } = new();
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
