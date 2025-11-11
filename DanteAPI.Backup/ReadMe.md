# Dante Backup Utility

## Overview
Command-line tool that creates automated backups of your Dante system data by exporting entities to Excel files. Each backup creates a dated folder containing Excel files and a detailed log.

## Requirements
- .NET 8 Runtime
- Valid Dante API credentials
- Sufficient disk space for backups

## Installation
1. Extract files to a directory (e.g., `C:\DanteBackup\`)
2. Configure `appsettings.json` (see below)

## Configuration

### appsettings.json
{
  "DanteURL": "https://your-dante-instance.com",
  "DanteAPIKey": "your-api-key-here",
  "Entities": "Booking|Company|Delegate|Schedule",
  "BackupStorageFolder": "C:\\DanteBackup",
  "MaxCollectionDepth": 1
}

**Configuration Parameters:**

- **DanteURL**: Your Dante system URL (without trailing slash)
- **DanteAPIKey**: Your API key from Dante system settings
- **Entities**: Pipe-separated list of entities to backup (see Available Entities below)
- **BackupStorageFolder**: Full path where backups will be stored
- **MaxCollectionDepth**: How deep to traverse collections (default: 1)

### Available Entities
- `Booking`
- `BookingItem`
- `Company`
- `Delegate`
- `Schedule`
- `ScheduleDelegate`
- `User`
- `Status`
- `Country`

**Example:** `"Entities": "Booking|Company|Delegate|Schedule|ScheduleDelegate"`

## Running the Backup

### Manual Execution
1. Open Command Prompt or PowerShell
2. Navigate to the installation directory
3. Run: `DanteAPI.Backup.exe`

### Output Structure
```
C:\Backups\Dante\
  └── 2025-11-11\
      ├── backup.log
      ├── Booking.xlsx
      ├── Company.xlsx
      └── Delegate.xlsx
```

## Automating with Windows Task Scheduler

**Recommended Schedule: Run between 11pm and 4am for optimal performance and to avoid user disruption.**

### Quick Setup:

1. **Open Task Scheduler**
   - Press `Win + R`, type `taskschd.msc`, press Enter

2. **Create Basic Task**
   - Click "Create Basic Task"
   - Name: `Dante Backup`

3. **Set Trigger**
   - Select "Daily"
   - Start time: `02:00:00 AM` (recommended)
   - Recur every: `1` days

4. **Set Action**
   - Action: "Start a program"
   - Program/script: Browse to `DanteAPI.Backup.exe`
   - Start in: Enter the directory containing the executable

5. **Configure**
   - Under "General" tab:
     - Select "Run whether user is logged on or not"
     - Check "Run with highest privileges"
   - Under "Settings" tab:
     - Check "Run task as soon as possible after a scheduled start is missed"

## Excel File Format

**Main Sheets:**
- One sheet per entity with headers in bold
- Related objects flattened as `ObjectName.Property` (e.g., `Status.Name`, `Company.Name`)
- Cells exceeding 32,767 characters are truncated and highlighted in yellow

**Collection Sheets:**
- Collections exported to separate sheets named `{Entity}_{Collection}`
- Example: `Booking_Items` contains all items with `BookingID` linking column

## Troubleshooting

**"Invalid API key" error**
- Verify `DanteAPIKey` in `appsettings.json`

**"Could not find entity type" error**
- Check entity names match exactly (case-sensitive)

**Backup folder not created**
- Ensure path exists and application has write permissions

**Empty Excel files**
- Check `backup.log` for API errors
- Verify API key has read permissions

**Task Scheduler not running**
- Select "Run whether user is logged on or not"
- Review Task Scheduler History for errors

## Support
For issues, review the `backup.log` file for detailed error messages and contact your Dante system administrator.

---

**Best Practice**: Test the backup manually before scheduling to ensure correct configuration.