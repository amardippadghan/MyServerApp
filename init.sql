-- ===================================================================
-- ASSET TRACKING & ZONE MANAGEMENT SYSTEM - DATABASE SCHEMA
-- ===================================================================

-- Drop existing tables (in reverse order due to foreign keys)
DROP TABLE IF EXISTS Alerts;
DROP TABLE IF EXISTS AssetLogs;
DROP TABLE IF EXISTS Assets;
DROP TABLE IF EXISTS Zones;
DROP TABLE IF EXISTS ZoneTypes;
DROP TABLE IF EXISTS Users; -- Keep existing Users table

-- ===================================================================
-- 1. ZONE TYPES TABLE (Master Data)
-- ===================================================================
CREATE TABLE ZoneTypes (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL UNIQUE COMMENT 'Display name like Warehouse, Production',
    Code VARCHAR(50) NOT NULL UNIQUE COMMENT 'System code like WAREHOUSE, PRODUCTION',
    Description TEXT COMMENT 'Detailed description of zone type',
    IsRestricted TINYINT NOT NULL DEFAULT 0 COMMENT '1 = Restricted access, 0 = Normal access',
    Color VARCHAR(7) NOT NULL DEFAULT '#0066cc' COMMENT 'Hex color for UI display',
    Priority INT NOT NULL DEFAULT 0 COMMENT 'Priority level for zone type (higher = more priority)',
    Status TINYINT NOT NULL DEFAULT 1 COMMENT '1 = Active, 0 = Soft Deleted',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    -- Indexes for performance
    INDEX idx_code (Code),
    INDEX idx_is_restricted (IsRestricted),
    INDEX idx_status (Status),
    INDEX idx_priority (Priority)
) COMMENT 'Master table for zone type definitions';

-- ===================================================================
-- 2. ZONES TABLE
-- ===================================================================
CREATE TABLE Zones (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL COMMENT 'Zone display name like Main Warehouse',
    ZoneTypeId INT NOT NULL COMMENT 'Reference to zone type',
    Description TEXT COMMENT 'Specific zone description',
    Location VARCHAR(255) COMMENT 'Physical location details',
    Capacity INT DEFAULT NULL COMMENT 'Maximum asset capacity (optional)',
    CurrentAssetCount INT NOT NULL DEFAULT 0 COMMENT 'Current number of assets in zone',
    Status TINYINT NOT NULL DEFAULT 1 COMMENT '1 = Active, 0 = Soft Deleted',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    -- Foreign key constraint
    FOREIGN KEY (ZoneTypeId) REFERENCES ZoneTypes(Id) ON DELETE RESTRICT ON UPDATE CASCADE,
    
    -- Indexes for performance
    INDEX idx_zone_type_id (ZoneTypeId),
    INDEX idx_status (Status),
    INDEX idx_name (Name),
    INDEX idx_current_asset_count (CurrentAssetCount)
) COMMENT 'Individual zones within different zone types';

-- ===================================================================
-- 3. ASSETS TABLE
-- ===================================================================
CREATE TABLE Assets (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL COMMENT 'Asset display name',
    AssetCode VARCHAR(50) UNIQUE COMMENT 'Unique asset identifier/barcode',
    ZoneId INT NOT NULL COMMENT 'Current zone location',
    Description TEXT COMMENT 'Asset description and details',
    AssetType VARCHAR(50) COMMENT 'Type of asset (Equipment, Material, etc.)',
    SerialNumber VARCHAR(100) COMMENT 'Asset serial number',
    PurchaseDate DATE COMMENT 'Asset purchase date',
    PurchaseValue DECIMAL(10,2) COMMENT 'Asset purchase value',
    Status TINYINT NOT NULL DEFAULT 1 COMMENT '1 = Active, 0 = Soft Deleted',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    -- Foreign key constraint
    FOREIGN KEY (ZoneId) REFERENCES Zones(Id) ON DELETE RESTRICT ON UPDATE CASCADE,
    
    -- Indexes for performance
    INDEX idx_zone_id (ZoneId),
    INDEX idx_status (Status),
    INDEX idx_name (Name),
    INDEX idx_asset_code (AssetCode),
    INDEX idx_asset_type (AssetType)
) COMMENT 'Assets that can be moved between zones';

-- ===================================================================
-- 4. ASSET LOGS TABLE (Movement History)
-- ===================================================================
CREATE TABLE AssetLogs (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    AssetId INT NOT NULL COMMENT 'Asset being moved',
    FromZoneId INT COMMENT 'Previous zone (NULL for new assets)',
    ToZoneId INT NOT NULL COMMENT 'Destination zone',
    MovementType VARCHAR(50) NOT NULL DEFAULT 'TRANSFER' COMMENT 'TRANSFER, CHECKIN, CHECKOUT, etc.',
    ShiftTime VARCHAR(50) NOT NULL COMMENT 'Morning, Evening, Night shift',
    MovedBy VARCHAR(100) COMMENT 'Person/system who moved the asset',
    Reason TEXT COMMENT 'Reason for movement',
    Notes TEXT COMMENT 'Additional notes',
    Status TINYINT NOT NULL DEFAULT 1 COMMENT '1 = Active, 0 = Soft Deleted',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    -- Foreign key constraints
    FOREIGN KEY (AssetId) REFERENCES Assets(Id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (FromZoneId) REFERENCES Zones(Id) ON DELETE SET NULL ON UPDATE CASCADE,
    FOREIGN KEY (ToZoneId) REFERENCES Zones(Id) ON DELETE RESTRICT ON UPDATE CASCADE,
    
    -- Indexes for performance
    INDEX idx_asset_id (AssetId),
    INDEX idx_from_zone_id (FromZoneId),
    INDEX idx_to_zone_id (ToZoneId),
    INDEX idx_shift_time (ShiftTime),
    INDEX idx_movement_type (MovementType),
    INDEX idx_created_at (CreatedAt),
    INDEX idx_status (Status)
) COMMENT 'Complete history of asset movements';

-- ===================================================================
-- 5. ALERTS TABLE
-- ===================================================================
CREATE TABLE Alerts (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    AssetId INT NOT NULL COMMENT 'Asset that triggered the alert',
    ZoneId INT NOT NULL COMMENT 'Zone involved in the alert',
    AlertType VARCHAR(50) NOT NULL DEFAULT 'RESTRICTED_ZONE_ENTRY' COMMENT 'Type of alert',
    Title VARCHAR(255) NOT NULL COMMENT 'Alert title/summary',
    Message TEXT NOT NULL COMMENT 'Detailed alert message',
    Severity ENUM('LOW', 'MEDIUM', 'HIGH', 'CRITICAL') NOT NULL DEFAULT 'MEDIUM' COMMENT 'Alert severity level',
    IsRead TINYINT NOT NULL DEFAULT 0 COMMENT '0 = Unread, 1 = Read',
    ReadAt DATETIME NULL COMMENT 'When alert was marked as read',
    ReadBy VARCHAR(100) COMMENT 'Who marked the alert as read',
    AutoGenerated TINYINT NOT NULL DEFAULT 1 COMMENT '1 = System generated, 0 = Manual',
    Status TINYINT NOT NULL DEFAULT 1 COMMENT '1 = Active, 0 = Soft Deleted',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    -- Foreign key constraints
    FOREIGN KEY (AssetId) REFERENCES Assets(Id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (ZoneId) REFERENCES Zones(Id) ON DELETE CASCADE ON UPDATE CASCADE,
    
    -- Indexes for performance
    INDEX idx_asset_id (AssetId),
    INDEX idx_zone_id (ZoneId),
    INDEX idx_alert_type (AlertType),
    INDEX idx_is_read (IsRead),
    INDEX idx_severity (Severity),
    INDEX idx_created_at (CreatedAt),
    INDEX idx_status (Status)
) COMMENT 'System alerts for asset movements and violations';

-- ===================================================================
-- SAMPLE DATA INSERTION
-- ===================================================================

-- Insert Zone Types
INSERT INTO ZoneTypes (Name, Code, Description, IsRestricted, Color, Priority) VALUES
('Warehouse', 'WAREHOUSE', 'General storage and inventory areas', 0, '#28a745', 1),
('Production', 'PRODUCTION', 'Manufacturing and assembly areas', 0, '#007bff', 2),
('Restricted', 'RESTRICTED', 'High-security restricted access areas', 1, '#dc3545', 5),
('Quality Control', 'QC', 'Quality testing and inspection areas', 0, '#ffc107', 3),
('Shipping', 'SHIPPING', 'Loading dock and shipping areas', 0, '#17a2b8', 2),
('Maintenance', 'MAINTENANCE', 'Equipment maintenance areas', 0, '#6c757d', 2),
('Office', 'OFFICE', 'Administrative office areas', 0, '#6f42c1', 1),
('Security', 'SECURITY', 'Security monitoring and control areas', 1, '#e83e8c', 4);

-- Insert Zones
INSERT INTO Zones (Name, ZoneTypeId, Description, Location, Capacity) VALUES
-- Warehouse zones
('Main Warehouse', 1, 'Primary storage facility for raw materials', 'Building A, Floor 1', 500),
('Cold Storage', 1, 'Temperature controlled storage area', 'Building A, Floor 2', 200),

-- Production zones  
('Assembly Line A', 2, 'Main production line for electronic components', 'Building B, Floor 1', 50),
('Assembly Line B', 2, 'Secondary production line', 'Building B, Floor 2', 50),
('Packaging Area', 2, 'Final packaging and labeling', 'Building B, Floor 3', 100),

-- Quality Control zones
('QC Lab 1', 4, 'Primary quality control laboratory', 'Building C, Floor 1', 25),
('QC Lab 2', 4, 'Secondary testing facility', 'Building C, Floor 2', 25),

-- Shipping zones
('Loading Dock A', 5, 'Main shipping and receiving area', 'Building A, Ground Floor', 150),
('Loading Dock B', 5, 'Secondary shipping area', 'Building A, Ground Floor', 100),

-- Restricted zones
('Secure Vault', 3, 'High-security storage for valuable assets', 'Building D, Basement', 10),
('Server Room', 3, 'IT infrastructure and sensitive equipment', 'Building D, Floor 2', 5),

-- Maintenance zones
('Maintenance Workshop', 6, 'Equipment repair and maintenance', 'Building E, Floor 1', 75),

-- Office zones
('Admin Office', 7, 'Administrative headquarters', 'Building F, Floor 3', 30),

-- Security zones
('Security Control Room', 8, 'Main security monitoring center', 'Building D, Floor 1', 5);

-- Insert Sample Assets
INSERT INTO Assets (Name, AssetCode, ZoneId, Description, AssetType, SerialNumber) VALUES
-- Equipment Assets
('Forklift Unit 001', 'FL001', 1, 'Heavy duty warehouse forklift', 'Equipment', 'FL-2024-001'),
('Conveyor Belt System', 'CB001', 3, 'Automated conveyor for assembly line', 'Equipment', 'CB-SYS-001'),
('Quality Scanner A1', 'QS001', 6, 'Automated quality control scanner', 'Equipment', 'QS-A1-2024'),
('Packaging Machine X1', 'PM001', 5, 'Automatic packaging machine', 'Equipment', 'PM-X1-001'),

-- Material Assets
('Raw Material Batch R001', 'RM001', 1, 'Electronic components for production', 'Material', 'RM-R001-2024'),
('Finished Goods Batch F001', 'FG001', 8, 'Completed products ready for shipping', 'Material', 'FG-F001-2024'),

-- IT Assets
('Server Rack Unit 1', 'SR001', 11, 'Main database server', 'IT Equipment', 'SR-001-2024'),
('Security Camera System', 'SC001', 14, 'Main surveillance equipment', 'Security Equipment', 'SC-SYS-001'),

-- Tools
('Inspection Tools Set A', 'IT001', 6, 'Quality control inspection tools', 'Tools', 'ITS-A-001'),
('Maintenance Tool Kit B', 'MT001', 12, 'General maintenance tools', 'Tools', 'MTK-B-001');

-- Insert Sample Asset Logs (Movement History)
INSERT INTO AssetLogs (AssetId, FromZoneId, ToZoneId, MovementType, ShiftTime, MovedBy, Reason) VALUES
(1, NULL, 1, 'CHECKIN', 'Morning', 'System Admin', 'New asset registration'),
(1, 1, 3, 'TRANSFER', 'Morning', 'John Supervisor', 'Moved to production for assembly support'),
(2, NULL, 3, 'CHECKIN', 'Morning', 'System Admin', 'New equipment installation'),
(3, NULL, 6, 'CHECKIN', 'Evening', 'QC Manager', 'Quality control equipment setup'),
(7, NULL, 11, 'CHECKIN', 'Morning', 'IT Admin', 'Server installation in restricted area');

-- Insert Sample Alerts
INSERT INTO Alerts (AssetId, ZoneId, AlertType, Title, Message, Severity) VALUES
(7, 11, 'RESTRICTED_ZONE_ENTRY', 'Asset Moved to Restricted Zone', 
 'Server Rack Unit 1 (SR001) has been moved to Secure Vault - a restricted access zone. This movement requires security clearance.', 'HIGH'),
 
(1, 3, 'ASSET_TRANSFER', 'Asset Transferred Between Zones', 
 'Forklift Unit 001 (FL001) has been transferred from Main Warehouse to Assembly Line A for production support.', 'MEDIUM');

-- ===================================================================
-- UPDATE ZONE ASSET COUNTS (Trigger-like functionality)
-- ===================================================================
UPDATE Zones SET CurrentAssetCount = (
    SELECT COUNT(*) FROM Assets WHERE ZoneId = Zones.Id AND Status = 1
);

-- ===================================================================
-- USEFUL VIEWS FOR REPORTING
-- ===================================================================

-- View: Asset Current Status with Zone Information
CREATE VIEW AssetCurrentStatus AS
SELECT 
    a.Id AS AssetId,
    a.Name AS AssetName,
    a.AssetCode,
    a.AssetType,
    z.Id AS ZoneId,
    z.Name AS ZoneName,
    zt.Name AS ZoneTypeName,
    zt.IsRestricted,
    a.Status AS AssetStatus,
    a.CreatedAt AS AssetCreatedAt
FROM Assets a
JOIN Zones z ON a.ZoneId = z.Id
JOIN ZoneTypes zt ON z.ZoneTypeId = zt.Id
WHERE a.Status = 1 AND z.Status = 1;

-- View: Recent Asset Movements (Last 30 days)
CREATE VIEW RecentAssetMovements AS
SELECT 
    al.Id AS LogId,
    a.Name AS AssetName,
    a.AssetCode,
    fz.Name AS FromZone,
    tz.Name AS ToZone,
    al.MovementType,
    al.ShiftTime,
    al.MovedBy,
    al.CreatedAt AS MovementTime
FROM AssetLogs al
JOIN Assets a ON al.AssetId = a.Id
LEFT JOIN Zones fz ON al.FromZoneId = fz.Id
JOIN Zones tz ON al.ToZoneId = tz.Id
WHERE al.Status = 1 
AND al.CreatedAt >= DATE_SUB(NOW(), INTERVAL 30 DAY)
ORDER BY al.CreatedAt DESC;

-- View: Unread Alerts Summary
CREATE VIEW UnreadAlerts AS
SELECT 
    alt.Id AS AlertId,
    a.Name AS AssetName,
    a.AssetCode,
    z.Name AS ZoneName,
    alt.AlertType,
    alt.Title,
    alt.Severity,
    alt.CreatedAt
FROM Alerts alt
JOIN Assets a ON alt.AssetId = a.Id
JOIN Zones z ON alt.ZoneId = z.Id
WHERE alt.IsRead = 0 AND alt.Status = 1
ORDER BY alt.CreatedAt DESC;

-- ===================================================================
-- PERFORMANCE OPTIMIZATION
-- ===================================================================

-- Composite indexes for common queries
CREATE INDEX idx_assets_zone_status ON Assets(ZoneId, Status);
CREATE INDEX idx_logs_asset_created ON AssetLogs(AssetId, CreatedAt);
CREATE INDEX idx_alerts_unread_created ON Alerts(IsRead, CreatedAt);
CREATE INDEX idx_zones_type_status ON Zones(ZoneTypeId, Status);

-- ===================================================================
-- SCHEMA CREATION COMPLETE
-- ===================================================================
