import type { Page } from '@playwright/test';
import { BasePage } from './base.page';

export class LoginPage extends BasePage {
  private readonly selectors = {
    emailInput: '[data-testid="email-input"]',
    passwordInput: '[data-testid="password-input"]',
    signInButton: '[data-testid="sign-in-button"]',
    signUpLink: '[data-testid="sign-up-link"]',
    errorMessage: '[data-testid="error-message"]',
    emailLabel: 'label:has-text("Email")',
    passwordLabel: 'label:has-text("Password")',
    emailField: 'input[name="email"]',
    passwordField: 'input[name="password"]',
    loginButton: 'button[type="submit"]'
  };

  constructor(page: Page) {
    super(page);
  }

  async navigate() {
    await super.navigate('/login');
  }

  async login(email: string, password: string) {
    await this.fillEmail(email);
    await this.fillPassword(password);
    await this.clickSignIn();
  }

  async fillEmail(email: string) {
    // Try data-testid first, fallback to name attribute
    const emailSelector = await this.page.isVisible(this.selectors.emailInput) 
      ? this.selectors.emailInput 
      : this.selectors.emailField;
    await this.page.fill(emailSelector, email);
  }

  async fillPassword(password: string) {
    // Try data-testid first, fallback to name attribute
    const passwordSelector = await this.page.isVisible(this.selectors.passwordInput) 
      ? this.selectors.passwordInput 
      : this.selectors.passwordField;
    await this.page.fill(passwordSelector, password);
  }

  async clickSignIn() {
    // Try data-testid first, fallback to button type
    const buttonSelector = await this.page.isVisible(this.selectors.signInButton)
      ? this.selectors.signInButton
      : this.selectors.loginButton;
    await this.page.click(buttonSelector);
  }

  async clickSignUp() {
    await this.page.click(this.selectors.signUpLink);
  }

  async getErrorMessage(): Promise<string> {
    await this.page.waitForSelector(this.selectors.errorMessage);
    return await this.page.textContent(this.selectors.errorMessage) || '';
  }

  async isErrorMessageVisible(): Promise<boolean> {
    return await this.page.isVisible(this.selectors.errorMessage);
  }

  async waitForLoginForm() {
    await this.page.waitForSelector(this.selectors.emailLabel);
    await this.page.waitForSelector(this.selectors.passwordLabel);
  }

  async isOnLoginPage(): Promise<boolean> {
    return this.page.url().includes('/login');
  }
}