import { test, expect } from '@playwright/test'

function uniqueEmail() {
  return `e2e.${Date.now()}@example.com`
}

test.describe('Auth flow and Select validation', () => {
  test('explore page has no Select.Item value error and register/login persist token', async ({ page }) => {
    // 1) Check /explore for Select item console error
    const consoleMessages: string[] = []
    page.on('console', (msg) => {
      if (msg.type() === 'error') consoleMessages.push(msg.text())
    })

    await page.goto('http://localhost:3000/explore')
    // give some time for potential client-side errors to surface
    await page.waitForTimeout(500)

    // Ensure the Radix Select error isn't present
    const joined = consoleMessages.join('\n')
    expect(joined).not.toContain('A <Select.Item /> must have a value prop')

    // 2) Register a new user
    const email = uniqueEmail()
    const password = 'S3cureP@ssw0rd'

    await page.goto('http://localhost:3000/register')
    await page.getByRole('textbox', { name: 'Full Name' }).fill('Playwright E2E')
    await page.getByRole('textbox', { name: 'Email' }).fill(email)
    await page.getByRole('textbox', { name: 'Password', exact: true }).fill(password)
    await page.getByRole('textbox', { name: 'Confirm Password' }).fill(password)
    await page.getByRole('button', { name: 'Create Account' }).click()

    // 3) After register we land in login. Try to login and assert storage
    await page.waitForURL('**/login')
    await page.getByRole('textbox', { name: 'Email' }).fill(email)
    await page.getByRole('textbox', { name: 'Password', exact: true }).fill(password)
    await page.getByRole('button', { name: 'Sign In' }).click()

    // Wait for network to settle and app to store token
    await page.waitForTimeout(500)

    const stored = await page.evaluate(() => ({
      authToken: (globalThis as any).localStorage.getItem('authToken'),
      authUser: (globalThis as any).localStorage.getItem('authUser')
    }))

    // Token should be present if backend returns it
    expect(stored.authToken).toBeTruthy()

    // If user is returned, it should be valid JSON
    if (stored.authUser) {
      expect(() => JSON.parse(stored.authUser as string)).not.toThrow()
    }

    // Cleanup localStorage for test isolation
    await page.evaluate(() => {
      localStorage.removeItem('authToken')
      localStorage.removeItem('authUser')
    })
  })
})
