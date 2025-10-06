import { test, expect } from '@playwright/test';
import { LoginPage } from './pages/login.page';
import { DashboardPage } from './pages/dashboard.page';
import { StackCreatePage } from './pages/stack-create.page';
import { TEST_USERS, TEST_STACKS, TestDataHelper } from './fixtures/test-data';

test.describe('Login and Stack Creation Flow', () => {
  let loginPage: LoginPage;
  let dashboardPage: DashboardPage;
  let stackCreatePage: StackCreatePage;

  test.beforeEach(async ({ page }) => {
    loginPage = new LoginPage(page);
    dashboardPage = new DashboardPage(page);
    stackCreatePage = new StackCreatePage(page);
  });

  test('should successfully login and create a new stack', async () => {
    // Step 1: Navigate to login page
    await loginPage.navigate();
    await loginPage.waitForLoginForm();
    
    // Step 2: Perform login
    await loginPage.login(TEST_USERS.validUser.email, TEST_USERS.validUser.password);
    
    // Step 3: Verify successful login and dashboard access
    await dashboardPage.waitForDashboard();
    expect(await dashboardPage.isOnDashboard()).toBe(true);
    
    const welcomeMessage = await dashboardPage.getWelcomeMessage();
    expect(welcomeMessage.toLowerCase()).toContain('bem-vindo');

    // Step 4: Navigate to stack creation
    await dashboardPage.clickCreateStack();
    
    // Step 5: Fill stack creation form
    await stackCreatePage.waitForForm();
    expect(await stackCreatePage.isOnCreatePage()).toBe(true);
    
    const stackData = {
      ...TEST_STACKS.frontendStack,
      name: TestDataHelper.generateRandomStackName()
    };
    
    await stackCreatePage.createStack(stackData);
    
    // Step 6: Verify redirect back to dashboard
    await dashboardPage.waitForDashboard();
    expect(await dashboardPage.isOnDashboard()).toBe(true);
    
    // Step 7: Verify the new stack appears in the dashboard
    const stackNames = await dashboardPage.getStackNames();
    expect(stackNames).toContain(stackData.name);
  });

  test('should show validation errors for empty stack form', async () => {
    // Login first
    await loginPage.navigate();
    await loginPage.login(TEST_USERS.validUser.email, TEST_USERS.validUser.password);
    await dashboardPage.waitForDashboard();
    
    // Navigate to create stack
    await dashboardPage.clickCreateStack();
    await stackCreatePage.waitForForm();
    
    // Try to save without filling required fields
    await stackCreatePage.save();
    
    // Should remain on create page due to validation errors
    expect(await stackCreatePage.isOnCreatePage()).toBe(true);
  });

  test('should successfully create stack with markdown content', async () => {
    // Login
    await loginPage.navigate();
    await loginPage.login(TEST_USERS.validUser.email, TEST_USERS.validUser.password);
    await dashboardPage.waitForDashboard();
    
    // Create stack with markdown content
    await dashboardPage.clickCreateStack();
    await stackCreatePage.waitForForm();
    
    const stackData = {
      name: TestDataHelper.generateRandomStackName(),
      type: 'Backend',
      description: `# API Backend Stack

## Overview
This is a comprehensive backend stack for modern web applications.

### Key Features
- RESTful API design
- Database integration
- Authentication & Authorization
- Rate limiting
- Logging & Monitoring

### Technologies Used
- Node.js runtime
- Express.js framework
- PostgreSQL database
- Redis for caching

\`\`\`javascript
// Example endpoint
app.get('/api/stacks', async (req, res) => {
  const stacks = await stackService.getAll();
  res.json(stacks);
});
\`\`\`

### Database Schema
| Table | Description |
|-------|-------------|
| stacks | Main stack entities |
| technologies | Available technologies |
| users | User accounts |
`,
      technologies: ['Node.js', 'Express.js', 'PostgreSQL', 'Redis'],
      isPublic: true
    };
    
    await stackCreatePage.createStack(stackData);
    
    // Verify creation
    await dashboardPage.waitForDashboard();
    const stackNames = await dashboardPage.getStackNames();
    expect(stackNames).toContain(stackData.name);
  });

  test('should handle login failure with invalid credentials', async () => {
    await loginPage.navigate();
    await loginPage.waitForLoginForm();
    
    // Try to login with invalid credentials
    await loginPage.login(TEST_USERS.invalidUser.email, TEST_USERS.invalidUser.password);
    
    // Should remain on login page
    expect(await loginPage.isOnLoginPage()).toBe(true);
    
    // Should show error message
    const isErrorVisible = await loginPage.isErrorMessageVisible();
    if (isErrorVisible) {
      const errorMessage = await loginPage.getErrorMessage();
      expect(errorMessage.toLowerCase()).toContain('invalid');
    }
  });

  test('should create private stack', async () => {
    // Login
    await loginPage.navigate();
    await loginPage.login(TEST_USERS.validUser.email, TEST_USERS.validUser.password);
    await dashboardPage.waitForDashboard();
    
    // Create private stack
    await dashboardPage.clickCreateStack();
    await stackCreatePage.waitForForm();
    
    const stackData = {
      name: TestDataHelper.generateRandomStackName(),
      type: 'Mobile',
      description: 'Private mobile stack for internal projects.',
      technologies: ['Flutter', 'Dart', 'Firebase'],
      isPublic: false
    };
    
    await stackCreatePage.createStack(stackData);
    
    // Verify creation
    await dashboardPage.waitForDashboard();
    const stackNames = await dashboardPage.getStackNames();
    expect(stackNames).toContain(stackData.name);
  });

  test('should add multiple technologies to stack', async () => {
    // Login
    await loginPage.navigate();
    await loginPage.login(TEST_USERS.validUser.email, TEST_USERS.validUser.password);
    await dashboardPage.waitForDashboard();
    
    // Create stack with multiple technologies
    await dashboardPage.clickCreateStack();
    await stackCreatePage.waitForForm();
    
    await stackCreatePage.fillName(TestDataHelper.generateRandomStackName());
    await stackCreatePage.selectType('Frontend');
    await stackCreatePage.fillDescription('Frontend stack with multiple technologies');
    
    // Add multiple technologies
    const technologies = TestDataHelper.getRandomTechnologies(4);
    for (const tech of technologies) {
      await stackCreatePage.addTechnology(tech);
    }
    
    await stackCreatePage.setPublic(true);
    await stackCreatePage.save();
    
    // Verify creation
    await dashboardPage.waitForDashboard();
  });
});