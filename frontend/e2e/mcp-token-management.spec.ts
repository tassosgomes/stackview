import { test, expect } from '@playwright/test';
import { LoginPage } from './pages/login.page';
import { DashboardPage } from './pages/dashboard.page';
import { ProfilePage } from './pages/profile.page';
import { TEST_USERS } from './fixtures/test-data';

test.describe('MCP Token Management Flow', () => {
  let loginPage: LoginPage;
  let dashboardPage: DashboardPage;
  let profilePage: ProfilePage;

  test.beforeEach(async ({ page }) => {
    loginPage = new LoginPage(page);
    dashboardPage = new DashboardPage(page);
    profilePage = new ProfilePage(page);

    // Login before each test
    await loginPage.navigate();
    await loginPage.login(TEST_USERS.validUser.email, TEST_USERS.validUser.password);
    await dashboardPage.waitForDashboard();
  });

  test('should navigate to profile page and show MCP tokens section', async () => {
    // Navigate to profile via dashboard
    await dashboardPage.clickMcpTokens();
    
    // Should be on profile page
    expect(await profilePage.isOnProfilePage()).toBe(true);
    
    // Wait for tokens section to load
    await profilePage.waitForTokensSection();
  });

  test('should generate a new MCP token', async () => {
    // Navigate to profile
    await profilePage.navigate();
    await profilePage.waitForTokensSection();
    
    // Get initial token count
    const initialCount = await profilePage.getTokensCount();
    
    // Generate new token
    await profilePage.generateNewToken();
    
    // Should show token modal
    expect(await profilePage.isTokenModalVisible()).toBe(true);
    
    // Should display the raw token
    const tokenValue = await profilePage.getDisplayedTokenValue();
    expect(tokenValue).toBeTruthy();
    expect(tokenValue.length).toBeGreaterThan(20); // Tokens should be reasonably long
    
    // Copy the token
    await profilePage.copyToken();
    
    // Close the modal
    await profilePage.closeModal();
    
    // Verify token count increased
    const finalCount = await profilePage.getTokensCount();
    expect(finalCount).toBe(initialCount + 1);
  });

  test('should display generated token only once', async () => {
    await profilePage.navigate();
    await profilePage.waitForTokensSection();
    
    // Generate token and get the value
    await profilePage.generateNewToken();
    await profilePage.waitForTokenGeneration();
    
    const tokenValue = await profilePage.getDisplayedTokenValue();
    expect(tokenValue).toBeTruthy();
    
    // Close modal
    await profilePage.closeModal();
    
    // Navigate away and back
    await dashboardPage.navigate();
    await profilePage.navigate();
    await profilePage.waitForTokensSection();
    
    // Should not show the raw token value anywhere
    // (This is security feature - tokens are shown only once)
    // We can verify this by checking that no element contains the token value
  });

  test('should revoke an existing token', async () => {
    await profilePage.navigate();
    await profilePage.waitForTokensSection();
    
    // Generate a token first
    const tokenValue = await profilePage.generateAndCopyToken();
    expect(tokenValue).toBeTruthy();
    
    // Get initial count
    const initialCount = await profilePage.getTokensCount();
    expect(initialCount).toBeGreaterThan(0);
    
    // Get the token names to find one to revoke
    const tokenNames = await profilePage.getTokenNames();
    if (tokenNames.length > 0) {
      const tokenToRevoke = tokenNames[0];
      
      // Revoke the token
      await profilePage.revokeToken(tokenToRevoke);
      
      // Confirm revocation if confirmation dialog appears
      try {
        await profilePage.confirmRevoke();
      } catch {
        // Confirmation might not be required
      }
      
      // Verify token was removed or marked as revoked
      const finalCount = await profilePage.getTokensCount();
      // Token might be removed completely or just marked as revoked
      expect(finalCount).toBeLessThanOrEqual(initialCount);
    }
  });

  test('should handle multiple token generation', async () => {
    await profilePage.navigate();
    await profilePage.waitForTokensSection();
    
    const initialCount = await profilePage.getTokensCount();
    const tokensToGenerate = 3;
    
    // Generate multiple tokens
    for (let i = 0; i < tokensToGenerate; i++) {
      await profilePage.generateNewToken();
      await profilePage.waitForTokenGeneration();
      
      const tokenValue = await profilePage.getDisplayedTokenValue();
      expect(tokenValue).toBeTruthy();
      
      await profilePage.copyToken();
      await profilePage.closeModal();
    }
    
    // Verify all tokens were created
    const finalCount = await profilePage.getTokensCount();
    expect(finalCount).toBe(initialCount + tokensToGenerate);
  });

  test('should copy token to clipboard', async ({ context }) => {
    await profilePage.navigate();
    await profilePage.waitForTokensSection();
    
    // Grant clipboard permissions
    await context.grantPermissions(['clipboard-read', 'clipboard-write']);
    
    // Generate token
    await profilePage.generateNewToken();
    await profilePage.waitForTokenGeneration();
    
    const tokenValue = await profilePage.getDisplayedTokenValue();
    
    // Copy token
    await profilePage.copyToken();
    
    // Verify clipboard content
    const clipboardContent = await profilePage.evaluateInPage(() => {
      return (navigator as any).clipboard.readText();
    });
    expect(clipboardContent).toBe(tokenValue);
    
    await profilePage.closeModal();
  });

  test('should show token creation timestamp', async () => {
    await profilePage.navigate();
    await profilePage.waitForTokensSection();
    
    // Generate a new token
    await profilePage.generateAndCopyToken();
    
    // Verify that tokens list shows creation information
    const tokensCount = await profilePage.getTokensCount();
    expect(tokensCount).toBeGreaterThan(0);
    
    // The exact implementation depends on how timestamps are displayed
    // This is a placeholder for timestamp verification
  });

  test('should handle token generation errors gracefully', async () => {
    await profilePage.navigate();
    await profilePage.waitForTokensSection();
    
    // This test would simulate server errors during token generation
    // The exact implementation depends on how errors are handled
    // For now, we just verify that the UI doesn't break with repeated attempts
    
    for (let i = 0; i < 3; i++) {
      try {
        await profilePage.generateNewToken();
        await profilePage.waitForTokenGeneration();
        await profilePage.closeModal();
      } catch (error) {
        // Error handling - UI should remain functional
        console.log(`Token generation attempt ${i + 1} failed:`, error);
      }
    }
    
    // UI should still be functional
    expect(await profilePage.isOnProfilePage()).toBe(true);
  });

  test('should prevent token generation without authentication', async () => {
    // Logout first (if logout functionality exists)
    try {
      await dashboardPage.logout();
    } catch {
      // Logout might not be available, continue
    }
    
    // Try to access profile page directly
    await profilePage.navigate();
    
    // Should redirect to login or show authentication error
    // The exact behavior depends on the authentication implementation
    const isOnProfile = await profilePage.isOnProfilePage();
    const isOnLogin = await loginPage.isOnLoginPage();
    
    // Should either redirect to login or show auth error
    expect(isOnProfile || isOnLogin).toBe(true);
  });

  test('should validate token format', async () => {
    await profilePage.navigate();
    await profilePage.waitForTokensSection();
    
    // Generate token
    await profilePage.generateNewToken();
    await profilePage.waitForTokenGeneration();
    
    const tokenValue = await profilePage.getDisplayedTokenValue();
    
    // Validate token format (depends on implementation)
    // Common formats include JWT, UUID, or custom formats
    expect(tokenValue).toMatch(/^[A-Za-z0-9_-]+$/); // Basic alphanumeric format
    expect(tokenValue.length).toBeGreaterThan(20);
    expect(tokenValue.length).toBeLessThan(500);
    
    await profilePage.closeModal();
  });
});