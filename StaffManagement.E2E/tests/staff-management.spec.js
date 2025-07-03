import { test, expect } from '@playwright/test';

test.describe('Staff Management System', () => {
  test('should load the main page', async ({ page }) => {
    await page.goto('/');
    await expect(page).toHaveTitle(/Staff Management System/);
    await expect(page.locator('h6')).toContainText('Staff Management System');
  });

  test('should display staff list', async ({ page }) => {
    await page.goto('/');
    
    // Wait for the staff list to load
    await page.waitForSelector('[data-testid="staff-list"]', { timeout: 10000 });
    
    // Check if staff members are displayed
    const staffRows = page.locator('[data-testid="staff-row"]');
    await expect(staffRows).toHaveCountGreaterThan(0);
  });

  test('should open add staff form', async ({ page }) => {
    await page.goto('/');
    
    // Click on Add Staff button
    await page.click('[data-testid="add-staff-button"]');
    
    // Check if form is displayed
    await expect(page.locator('[data-testid="staff-form"]')).toBeVisible();
    await expect(page.locator('input[name="firstName"]')).toBeVisible();
  });

  test('should search for staff', async ({ page }) => {
    await page.goto('/');
    
    // Wait for the search input to be available
    await page.waitForSelector('[data-testid="search-input"]');
    
    // Enter search term
    await page.fill('[data-testid="search-input"]', 'John');
    await page.click('[data-testid="search-button"]');
    
    // Wait for results
    await page.waitForTimeout(1000);
    
    // Check if search results are filtered
    const staffRows = page.locator('[data-testid="staff-row"]');
    await expect(staffRows).toHaveCountGreaterThan(0);
  });

  test('should export staff data', async ({ page }) => {
    await page.goto('/');
    
    // Wait for export button to be available
    await page.waitForSelector('[data-testid="export-csv-button"]');
    
    // Start waiting for download before clicking
    const downloadPromise = page.waitForEvent('download');
    
    // Click export button
    await page.click('[data-testid="export-csv-button"]');
    
    // Wait for download to complete
    const download = await downloadPromise;
    expect(download.suggestedFilename()).toMatch(/staff_export_.*\.csv/);
  });
});
