# StaffManager

A Staff Management application built with ASP.NET Web API and React, featuring CRUD operations, advanced search, and Excel/PDF export capabilities.

## Overview

This application provides a robust solution for managing staff records, including:

- **CRUD Operations**: Add, edit, delete, and search staff records.
- **Advanced Search**: Filter staff by ID, gender, and birthday range.
- **Export**: Generate reports in Excel and PDF formats.
- **Testing**: Includes unit tests, integration tests, and end-to-end tests.
- **CI/CD**: Automated builds and tests using GitHub Actions.

## Project Structure

```
StaffManager/
├── StaffManagement.Api/         # ASP.NET Web API backend
├── StaffManagement.Web/         # React frontend
├── StaffManagement.Tests/       # Unit and integration tests
├── StaffManagement.E2E/         # End-to-end tests
├── .github/workflows/           # CI pipeline configuration
├── README.md                    # Project documentation
├── LICENSE                      # MIT License
```

## Setup

### Prerequisites

- .NET 8.0 SDK
- Node.js 18
- Playwright (for E2E tests)

### Backend

1. Navigate to StaffManagement.Api.

2. Run:

   ```bash
   dotnet restore
   dotnet run
   ```

### Frontend

1. Navigate to StaffManagement.Web.

2. Run:

   ```bash
   npm install
   npm start
   ```

### Testing

- **Unit/Integration Tests**: Run dotnet test in StaffManagement.Tests.
- **E2E Tests**: Run npx playwright test in StaffManagement.E2E.

## Dependencies

- **Backend**: ASP.NET Core 8.0, xUnit
- **Frontend**: React 18, Material-UI, XLSX, jsPDF
- **Testing**: xUnit, Playwright

## CI/CD

The repository uses GitHub Actions for continuous integration, running build and test steps on every push or pull request to the main branch. See .github/workflows/ci.yml for details.

## License

This project is licensed under the MIT License. See the LICENSE file for details.

