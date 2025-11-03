# Restore and Run API Integration Tests
# This script restores packages and runs all API integration tests

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "  InfraBase API Integration Tests" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

# Navigate to test project directory
$testProject = "Tests\PartnersHub.Services.InfraBase.Apis.Tests"
$projectFile = "$testProject\PartnersHub.Services.InfraBase.Apis.Tests.csproj"

Write-Host "Step 1: Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore $projectFile
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Package restore failed!" -ForegroundColor Red
    Write-Host "If you have network issues, packages might already be cached." -ForegroundColor Yellow
    Write-Host "Continuing with build..." -ForegroundColor Yellow
}
Write-Host ""

Write-Host "Step 2: Building test project..." -ForegroundColor Yellow
dotnet build $projectFile --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Build successful!" -ForegroundColor Green
Write-Host ""

Write-Host "Step 3: Running API integration tests..." -ForegroundColor Yellow
Write-Host "This will test all API endpoints end-to-end with in-memory database" -ForegroundColor Cyan
Write-Host ""

# Run tests with detailed output
dotnet test $projectFile --no-build --logger "console;verbosity=normal"

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "============================================" -ForegroundColor Green
    Write-Host "  ✅ ALL TESTS PASSED!" -ForegroundColor Green
    Write-Host "============================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Coverage includes:" -ForegroundColor Cyan
    Write-Host "  ✓ Request creation and management" -ForegroundColor Green
    Write-Host "  ✓ Complete approval workflow" -ForegroundColor Green
    Write-Host "  ✓ Rejection workflows" -ForegroundColor Green
    Write-Host "  ✓ Item CRUD operations" -ForegroundColor Green
    Write-Host "  ✓ Financial distribution management" -ForegroundColor Green
    Write-Host "  ✓ Attachment management" -ForegroundColor Green
    Write-Host "  ✓ Queries and pagination" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "============================================" -ForegroundColor Red
    Write-Host "  ❌ SOME TESTS FAILED" -ForegroundColor Red
    Write-Host "============================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Check the output above for details." -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "To run specific tests:" -ForegroundColor Cyan
Write-Host "  dotnet test --filter 'CompleteWorkflow'" -ForegroundColor Gray
Write-Host "  dotnet test --filter 'FullyQualifiedName~Item'" -ForegroundColor Gray
Write-Host "  dotnet test --filter 'FullyQualifiedName~Workflow'" -ForegroundColor Gray
