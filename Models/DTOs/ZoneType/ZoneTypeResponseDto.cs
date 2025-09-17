namespace MyServerApp.Models.DTOs.ZoneType
{
    public class ZoneTypeResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int IsRestricted { get; set; }
        public string RestrictedStatus => IsRestricted == 1 ? "Restricted" : "Normal";
        public bool IsRestrictedZone => IsRestricted == 1;
        public string Color { get; set; } = string.Empty;
        public int Priority { get; set; }
        public int Status { get; set; }
        public string StatusName => Status == 1 ? "Active" : "Deleted";
        public int ZoneCount { get; set; } // Number of zones using this type
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
