@echo off
REM Local CI/CD simulation script for Staff Management (Windows)

echo 🚀 Starting local CI/CD simulation...

REM Check if we're in the right directory
if not exist "staff-management.sln" (
    echo ❌ Please run this script from the staff-management root directory
    exit /b 1
)

REM Step 1: Backend Tests
echo.
echo 📦 Testing Backend...
dotnet clean staff-management.sln >nul 2>&1
dotnet restore staff-management.sln
if %errorlevel% neq 0 (
    echo ❌ Failed to restore dependencies
    exit /b 1
)
echo ✅ Dependencies restored

dotnet build staff-management.sln --no-restore
if %errorlevel% neq 0 (
    echo ❌ Build failed
    exit /b 1
)
echo ✅ Solution built successfully

dotnet test StaffManagement.Tests/StaffManagement.Tests.csproj --no-build --verbosity quiet
if %errorlevel% neq 0 (
    echo ❌ Backend tests failed
    exit /b 1
)
echo ✅ All backend tests passed

REM Step 2: Frontend Tests
echo.
echo 🌐 Testing Frontend...
cd StaffManagement.Web

if not exist "package-lock.json" (
    echo ⚠️ package-lock.json not found, running npm install...
    npm install
)

npm ci >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Failed to install frontend dependencies
    exit /b 1
)
echo ✅ Frontend dependencies installed

echo ⚠️ Skipping frontend tests (not configured yet)

npm run build >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Frontend build failed
    exit /b 1
)
echo ✅ Frontend built successfully

cd ..

REM Step 3: E2E Tests (optional)
echo.
echo 🔄 E2E Tests...
if exist "StaffManagement.E2E\package.json" (
    cd StaffManagement.E2E
    npm ci >nul 2>&1
    echo ✅ E2E dependencies installed
    echo ⚠️ E2E tests available but not running (requires running servers)
    cd ..
) else (
    echo ⚠️ E2E tests not configured
)

REM Step 4: Security Scan (optional)
echo.
echo 🔒 Security...
echo ⚠️ Security scan would run here (Trivy or similar)

REM Step 5: Build for deployment
echo.
echo 📦 Building for deployment...
dotnet publish StaffManagement.Api/StaffManagement.Api.csproj -c Release -o ./publish/backend >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Backend publish failed
    exit /b 1
)
echo ✅ Backend published for deployment

cd StaffManagement.Web
set REACT_APP_API_BASE_URL=/api
npm run build >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Frontend deployment build failed
    exit /b 1
)
echo ✅ Frontend built for deployment
cd ..

REM Create deployment structure
if not exist "deployment" mkdir deployment
if exist "publish\backend" xcopy "publish\backend" "deployment\backend\" /E /I /Q >nul 2>&1
if exist "StaffManagement.Web\build" xcopy "StaffManagement.Web\build" "deployment\frontend\" /E /I /Q >nul 2>&1
echo Deployment created at: %date% %time% > deployment\deployment-info.txt
echo Local simulation run >> deployment\deployment-info.txt

echo ✅ Deployment artifacts created in .\deployment\

echo.
echo 🎉 Local CI/CD simulation completed successfully!
echo.
echo Next steps:
echo • Push to GitHub to trigger the actual CI/CD pipeline
echo • Check the deployment artifacts in .\deployment\
echo • Deploy the artifacts to your hosting platform

REM Cleanup
echo.
echo 🧹 Cleaning up...
if exist "publish" rmdir /s /q "publish" >nul 2>&1
echo ✅ Temporary files cleaned up

pause
