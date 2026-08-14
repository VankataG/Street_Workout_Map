$ErrorActionPreference = "Stop"

$timestamp = Get-Date -Format "yyyy-MM-dd_HHmm"

$projectRoot = "D:\source\StreetWorkoutMap"
$backupRoot = "D:\source\SW-MAP_BACKUP"

$databaseBackupDir = Join-Path $backupRoot "Database"
$storageBackupDir = Join-Path $backupRoot "Storage\Storage_$timestamp"

$databaseBackupFile = Join-Path `
    $databaseBackupDir `
    "SWMAP_$timestamp.dump"


# =========================================================
# DATABASE CONFIG
# =========================================================

$dbHost = "aws-0-eu-west-1.pooler.supabase.com"
$dbPort = "5432"
$dbName = "postgres"
$dbUser = "postgres.ykpshelwhlkxgqjxqkuj"


# =========================================================
# STORAGE CONFIG
# =========================================================

$storageBucket = "workout-spot-images"


# =========================================================
# CREATE BACKUP DIRECTORIES
# =========================================================

New-Item `
    -ItemType Directory `
    -Force `
    -Path $databaseBackupDir `
    | Out-Null

New-Item `
    -ItemType Directory `
    -Force `
    -Path $storageBackupDir `
    | Out-Null


Write-Host ""
Write-Host "================================="
Write-Host "        SW-MAP BACKUP"
Write-Host "================================="
Write-Host "Timestamp: $timestamp"
Write-Host ""


# =========================================================
# DATABASE BACKUP
# =========================================================

Write-Host "1/2 Backing up PostgreSQL database..."

pg_dump `
    -h $dbHost `
    -p $dbPort `
    -U $dbUser `
    -d $dbName `
    -Fc `
    -f $databaseBackupFile

if ($LASTEXITCODE -ne 0)
{
    throw "Database backup failed."
}

Write-Host "Database backup completed."
Write-Host ""


# =========================================================
# VERIFY DATABASE BACKUP
# =========================================================

Write-Host "Checking database backup..."

pg_restore `
    --list `
    $databaseBackupFile `
    | Out-Null

if ($LASTEXITCODE -ne 0)
{
    throw "Database backup validation failed."
}

Write-Host "Database backup is readable."
Write-Host ""


# =========================================================
# STORAGE BACKUP
# =========================================================

Write-Host "2/2 Backing up Supabase Storage..."

Set-Location $projectRoot

supabase `
    --experimental `
    storage cp `
    -r `
    "ss:///$storageBucket/" `
    $storageBackupDir `
    --linked

if ($LASTEXITCODE -ne 0)
{
    throw "Storage backup failed."
}

Write-Host "Storage backup completed."
Write-Host ""


# =========================================================
# SUMMARY
# =========================================================

$dbSize = (Get-Item $databaseBackupFile).Length

$storageFiles = Get-ChildItem `
    $storageBackupDir `
    -Recurse `
    -File

$storageFileCount = $storageFiles.Count

$storageSize = (
    $storageFiles |
    Measure-Object `
        -Property Length `
        -Sum
).Sum


Write-Host "================================="
Write-Host "        BACKUP COMPLETE"
Write-Host "================================="
Write-Host ""

Write-Host "Database:"
Write-Host "  $databaseBackupFile"
Write-Host "  Size: $dbSize bytes"
Write-Host ""

Write-Host "Storage:"
Write-Host "  $storageBackupDir"
Write-Host "  Files: $storageFileCount"
Write-Host "  Size: $storageSize bytes"
Write-Host ""