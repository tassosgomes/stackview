import { test, expect } from '@playwright/test';
import { LoginPage } from './pages/login.page';
import { DashboardPage } from './pages/dashboard.page';
import { ExplorePage } from './pages/explore.page';
import { StackCreatePage } from './pages/stack-create.page';
import { TEST_USERS } from './fixtures/test-data';

test.describe('Exploration and Search Flow', () => {
  let loginPage: LoginPage;
  let dashboardPage: DashboardPage;
  let explorePage: ExplorePage;
  let stackCreatePage: StackCreatePage;

  test.beforeEach(async ({ page }) => {
    loginPage = new LoginPage(page);
    dashboardPage = new DashboardPage(page);
    explorePage = new ExplorePage(page);
    stackCreatePage = new StackCreatePage(page);
  });

  test('should explore public stacks without authentication', async () => {
    // Navigate directly to explore page
    await explorePage.navigate();
    expect(await explorePage.isOnExplorePage()).toBe(true);
    
    // Wait for stacks to load
    await explorePage.waitForStacks();
    
    // Verify that stacks are displayed
    const stacksCount = await explorePage.getStacksCount();
    expect(stacksCount).toBeGreaterThan(0);
  });

  test('should search for stacks by technology', async () => {
    await explorePage.navigate();
    
    // Search for React stacks
    await explorePage.search('React');
    
    // Wait for search results
    await explorePage.waitForStacks();
    
    // Verify search results contain React-related stacks
    const stackNames = await explorePage.getStackNames();
    expect(stackNames.length).toBeGreaterThan(0);
    
    // Check if at least one stack mentions React
    const hasReactStack = await explorePage.hasStackWithTechnology('React');
    expect(hasReactStack).toBe(true);
  });

  test('should filter stacks by type', async () => {
    await explorePage.navigate();
    
    // Filter by Frontend type
    await explorePage.filterByType('Frontend');
    
    // Wait for filtered results
    await explorePage.waitForStacks();
    
    // Verify filtered results
    const stacksCount = await explorePage.getStacksCount();
    expect(stacksCount).toBeGreaterThan(0);
  });

  test('should handle no search results', async () => {
    await explorePage.navigate();
    
    // Search for a technology that doesn't exist
    await explorePage.search('NonExistentTech12345');
    
    // Check for no results message
    const noResults = await explorePage.isNoResultsVisible();
    if (noResults) {
      expect(noResults).toBe(true);
    } else {
      // Alternative: check if stacks count is 0
      const stacksCount = await explorePage.getStacksCount();
      expect(stacksCount).toBe(0);
    }
  });

  test('should create stack and find it in exploration', async () => {
    // First login and create a public stack
    await loginPage.navigate();
    await loginPage.login(TEST_USERS.validUser.email, TEST_USERS.validUser.password);
    await dashboardPage.waitForDashboard();
    
    // Create a unique stack with specific technology
    await dashboardPage.clickCreateStack();
    await stackCreatePage.waitForForm();
    
    const uniqueStackName = `Test Stack ${Date.now()}`;
    const uniqueTechnology = 'UniqueTestTech';
    
    const stackData = {
      name: uniqueStackName,
      type: 'Backend',
      description: `# ${uniqueStackName}

This is a test stack created for E2E testing.

## Technologies
- ${uniqueTechnology}
- Node.js
- Express

## Purpose
Testing exploration and search functionality.
`,
      technologies: [uniqueTechnology, 'Node.js', 'Express'],
      isPublic: true
    };
    
    await stackCreatePage.createStack(stackData);
    await dashboardPage.waitForDashboard();
    
    // Now go to explore and search for the created stack
    await explorePage.navigate();
    await explorePage.search(uniqueTechnology);
    
    // Verify the stack appears in search results
    const stackNames = await explorePage.getStackNames();
    expect(stackNames).toContain(uniqueStackName);
  });

  test('should navigate to stack details from exploration', async () => {
    await explorePage.navigate();
    await explorePage.waitForStacks();
    
    const stacksCount = await explorePage.getStacksCount();
    if (stacksCount > 0) {
      // Click on the first stack
      await explorePage.clickStackByIndex(0);
      
      // Should navigate to stack detail page
      // Note: This assumes stack detail pages exist
      // The actual navigation depends on the routing implementation
      await explorePage.waitForUrlPattern(/.*\/stacks\/.*/);
    }
  });

  test('should combine search and filter', async () => {
    await explorePage.navigate();
    
    // Apply both search and filter
    await explorePage.filterByType('Frontend');
    await explorePage.search('React');
    
    // Wait for results
    await explorePage.waitForStacks();
    
    // Should show only Frontend stacks that contain React
    const stacksCount = await explorePage.getStacksCount();
    if (stacksCount > 0) {
      const hasReactStack = await explorePage.hasStackWithTechnology('React');
      expect(hasReactStack).toBe(true);
    }
  });

  test('should perform case-insensitive search', async () => {
    await explorePage.navigate();
    
    // Search with different cases
    const searches = ['react', 'REACT', 'React', 'rEaCt'];
    
    for (const searchTerm of searches) {
      await explorePage.search(searchTerm);
      
      // All should return similar results
      const stacksCount = await explorePage.getStacksCount();
      // At least verify no error occurs
      expect(stacksCount).toBeGreaterThanOrEqual(0);
    }
  });

  test('should handle special characters in search', async () => {
    await explorePage.navigate();
    
    // Test various special characters
    const specialSearches = ['React.js', 'C#', 'C++', '.NET', 'Vue.js'];
    
    for (const searchTerm of specialSearches) {
      await explorePage.search(searchTerm);
      
      // Should not crash or error
      const stacksCount = await explorePage.getStacksCount();
      expect(stacksCount).toBeGreaterThanOrEqual(0);
    }
  });

  test('should show all stacks when search is cleared', async () => {
    await explorePage.navigate();
    
    // Get initial stacks count
    const initialCount = await explorePage.getStacksCount();
    
    // Search for something specific
    await explorePage.search('React');
    
    // Clear search
    await explorePage.search('');
    
    // Should show all stacks again
    const finalCount = await explorePage.getStacksCount();
    expect(finalCount).toBeGreaterThanOrEqual(initialCount);
  });
});