$GMS_RUNTIME_VERSION = "2026.0.0.23"
$GMS_USER_FOLDER = "$env:APPDATA\GameMakerStudio2-LTS2026\mathew.odwyer_4882385"
$GMS_RUNTIME_BASE = "$env:PROGRAMDATA\GameMakerStudio2-LTS2026\Cache\runtimes\runtime-$GMS_RUNTIME_VERSION"
$IGOR = "$GMS_RUNTIME_BASE\bin\igor\windows\x64\igor.exe"

$PROJECT_ROOT = $PSScriptRoot
$PROJECT_FILE = "$PROJECT_ROOT\Room.yyp"
$CACHE_DIR = "$PROJECT_ROOT\cache"
$TEMP_DIR = "$PROJECT_ROOT\temp"
$OUTPUT_FILENAME = "$PROJECT_ROOT\output\interim\Room"
$TARGET_FILE = "$PROJECT_ROOT\output\Room.zip"
$DEVICE_NAME = "GM Builder"

if (-not (Test-Path $IGOR)) {
    Write-Error "Igor.exe not found at: $IGOR`n    Check GMS_RUNTIME_VERSION is correct."
    exit 1
}

if (-not (Test-Path $PROJECT_FILE)) {
    Write-Error "Project file not found at: $PROJECT_FILE"
    exit 1
}

Write-Host "Starting builder container..."
docker-compose up -d builder

Write-Host "Waiting for SSH to be ready..."
Start-Sleep -Seconds 2

Remove-Item -Recurse -Force -ErrorAction SilentlyContinue "$PROJECT_ROOT\output"

New-Item -ItemType Directory -Force -Path "$PROJECT_ROOT\output" | Out-Null
New-Item -ItemType Directory -Force -Path "$PROJECT_ROOT\output\interim" | Out-Null

Write-Host "Building Linux package..."

$IgorArgs = @(
    "/uf=$GMS_USER_FOLDER",
    "/rp=$GMS_RUNTIME_BASE",
    "/project=$PROJECT_FILE",
    "/cache=$CACHE_DIR",
    "/temp=$TEMP_DIR",
    "/of=$OUTPUT_FILENAME",
    "/tf=$TARGET_FILE",
    "/device=$DEVICE_NAME",
    "--",
    "Linux",
    "Package"
)

& $IGOR @IgorArgs

if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

docker-compose down builder -v

Write-Host "Build complete! Output at: $TARGET_FILE"