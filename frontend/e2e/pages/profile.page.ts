import type { Page } from '@playwright/test';
import { BasePage } from './base.page';

export class ProfilePage extends BasePage {
  private readonly selectors = {
    generateTokenButton: '[data-testid="generate-token-button"]',
    revokeTokenButton: '[data-testid="revoke-token-button"]',
    tokensList: '[data-testid="tokens-list"]',
    tokenItem: '[data-testid="token-item"]',
    tokenName: '[data-testid="token-name"]',
    tokenValue: '[data-testid="token-value"]',
    tokenModal: '[data-testid="token-modal"]',
    copyTokenButton: '[data-testid="copy-token-button"]',
    closeModalButton: '[data-testid="close-modal-button"]',
    confirmRevokeButton: '[data-testid="confirm-revoke-button"]',
    
    // Fallback selectors
    generateButton: 'button:has-text("Gerar")',
    tokensSection: 'text="Tokens MCP"',
    copyButton: 'button:has-text("Copiar")'
  };

  constructor(page: Page) {
    super(page);
  }

  async navigate() {
    await super.navigate('/profile');
  }

  async generateNewToken() {
    const generateSelector = await this.page.isVisible(this.selectors.generateTokenButton)
      ? this.selectors.generateTokenButton
      : this.selectors.generateButton;
    await this.page.click(generateSelector);
  }

  async copyToken() {
    const copySelector = await this.page.isVisible(this.selectors.copyTokenButton)
      ? this.selectors.copyTokenButton
      : this.selectors.copyButton;
    await this.page.click(copySelector);
  }

  async closeModal() {
    await this.page.click(this.selectors.closeModalButton);
  }

  async revokeToken(tokenName: string) {
    // Find the token row and click its revoke button
    const tokenRow = this.page.locator(`${this.selectors.tokenItem}:has-text("${tokenName}")`);
    await tokenRow.locator(this.selectors.revokeTokenButton).click();
  }

  async confirmRevoke() {
    await this.page.click(this.selectors.confirmRevokeButton);
  }

  async getTokensCount(): Promise<number> {
    return await this.page.locator(this.selectors.tokenItem).count();
  }

  async getTokenNames(): Promise<string[]> {
    const tokenElements = await this.page.locator(this.selectors.tokenName).all();
    const names: string[] = [];
    for (const element of tokenElements) {
      const text = await element.textContent();
      if (text) names.push(text.trim());
    }
    return names;
  }

  async isTokenModalVisible(): Promise<boolean> {
    return await this.page.isVisible(this.selectors.tokenModal);
  }

  async getDisplayedTokenValue(): Promise<string> {
    await this.page.waitForSelector(this.selectors.tokenValue);
    return await this.page.textContent(this.selectors.tokenValue) || '';
  }

  async waitForTokensSection() {
    await this.page.waitForSelector(this.selectors.tokensSection);
  }

  async isOnProfilePage(): Promise<boolean> {
    return this.page.url().includes('/profile');
  }

  async hasToken(tokenName: string): Promise<boolean> {
    const tokenNames = await this.getTokenNames();
    return tokenNames.includes(tokenName);
  }

  async waitForTokenGeneration() {
    await this.page.waitForSelector(this.selectors.tokenModal);
    await this.page.waitForSelector(this.selectors.tokenValue);
  }

  async generateAndCopyToken(): Promise<string> {
    await this.generateNewToken();
    await this.waitForTokenGeneration();
    
    const tokenValue = await this.getDisplayedTokenValue();
    await this.copyToken();
    await this.closeModal();
    
    return tokenValue;
  }
}