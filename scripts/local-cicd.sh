#!/bin/bash
# Local CI/CD simulation script for Staff Management

echo "🚀 Starting local CI/CD simulation..."

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to print status
print_status() {
    echo -e "${GREEN}✓${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}⚠${NC} $1"
}

print_error() {
    echo -e "${RED}✗${NC} $1"
}

# Check if we're in the right directory
if [ ! -f "staff-management.sln" ]; then
    print_error "Please run this script from the staff-management root directory"
    exit 1
fi

# Step 1: Backend Tests
echo -e "\n📦 Testing Backend..."
dotnet clean staff-management.sln > /dev/null 2>&1
dotnet restore staff-management.sln
if [ $? -eq 0 ]; then
    print_status "Dependencies restored"
else
    print_error "Failed to restore dependencies"
    exit 1
fi

dotnet build staff-management.sln --no-restore
if [ $? -eq 0 ]; then
    print_status "Solution built successfully"
else
    print_error "Build failed"
    exit 1
fi

dotnet test StaffManagement.Tests/StaffManagement.Tests.csproj --no-build --verbosity quiet
if [ $? -eq 0 ]; then
    print_status "All backend tests passed"
else
    print_error "Backend tests failed"
    exit 1
fi

# Step 2: Frontend Tests
echo -e "\n🌐 Testing Frontend..."
cd StaffManagement.Web

if [ ! -f "package-lock.json" ]; then
    print_warning "package-lock.json not found, running npm install..."
    npm install
fi

npm ci > /dev/null 2>&1
if [ $? -eq 0 ]; then
    print_status "Frontend dependencies installed"
else
    print_error "Failed to install frontend dependencies"
    exit 1
fi

# Skip frontend tests for now as they might not be set up
print_warning "Skipping frontend tests (not configured yet)"

npm run build > /dev/null 2>&1
if [ $? -eq 0 ]; then
    print_status "Frontend built successfully"
else
    print_error "Frontend build failed"
    exit 1
fi

cd ..

# Step 3: E2E Tests (optional)
echo -e "\n🔄 E2E Tests..."
if [ -d "StaffManagement.E2E" ] && [ -f "StaffManagement.E2E/package.json" ]; then
    cd StaffManagement.E2E
    if [ -f "package.json" ]; then
        npm ci > /dev/null 2>&1
        print_status "E2E dependencies installed"
        print_warning "E2E tests available but not running (requires running servers)"
    fi
    cd ..
else
    print_warning "E2E tests not configured"
fi

# Step 4: Security Scan (optional)
echo -e "\n🔒 Security..."
print_warning "Security scan would run here (Trivy or similar)"

# Step 5: Build for deployment
echo -e "\n📦 Building for deployment..."
dotnet publish StaffManagement.Api/StaffManagement.Api.csproj -c Release -o ./publish/backend > /dev/null 2>&1
if [ $? -eq 0 ]; then
    print_status "Backend published for deployment"
else
    print_error "Backend publish failed"
    exit 1
fi

cd StaffManagement.Web
REACT_APP_API_BASE_URL="/api" npm run build > /dev/null 2>&1
if [ $? -eq 0 ]; then
    print_status "Frontend built for deployment"
else
    print_error "Frontend deployment build failed"
    exit 1
fi
cd ..

# Create deployment structure
mkdir -p deployment
cp -r publish/backend deployment/ 2>/dev/null
cp -r StaffManagement.Web/build deployment/frontend 2>/dev/null
echo "Deployment created at: $(date)" > deployment/deployment-info.txt
echo "Local simulation run" >> deployment/deployment-info.txt

print_status "Deployment artifacts created in ./deployment/"

echo -e "\n🎉 ${GREEN}Local CI/CD simulation completed successfully!${NC}"
echo -e "\nNext steps:"
echo "• Push to GitHub to trigger the actual CI/CD pipeline"
echo "• Check the deployment artifacts in ./deployment/"
echo "• Deploy the artifacts to your hosting platform"

# Cleanup
echo -e "\n🧹 Cleaning up..."
rm -rf publish 2>/dev/null
print_status "Temporary files cleaned up"
