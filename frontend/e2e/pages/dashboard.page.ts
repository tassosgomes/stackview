import type { Page } from '@playwright/test';
import { BasePage } from './base.page';

export class DashboardPage extends BasePage {
  private readonly selectors = {
    welcomeMessage: '[data-testid="welcome-message"]',
    createStackButton: '[data-testid="create-stack-button"]',
    stacksList: '[data-testid="stacks-list"]',
    stackCard: '[data-testid="stack-card"]',
    emptyState: '[data-testid="empty-state"]',
    profileLink: '[data-testid="profile-link"]',
    logoutButton: '[data-testid="logout-button"]',
    exploreLink: 'a[href="/explore"]',
    createNewStackButton: 'text="Criar Novo Stack"',
    myStacksHeading: 'text="Meus Stacks"',
    mcpTokensLink: 'text="Gerenciar Tokens MCP"'
  };

  constructor(page: Page) {
    super(page);
  }

  async navigate() {
    await super.navigate('/dashboard');
  }

  async clickCreateStack() {
    const createButton = await this.page.isVisible(this.selectors.createStackButton)
      ? this.selectors.createStackButton
      : this.selectors.createNewStackButton;
    await this.page.click(createButton);
  }

  async clickProfile() {
    await this.page.click(this.selectors.profileLink);
  }

  async clickMcpTokens() {
    await this.page.click(this.selectors.mcpTokensLink);
  }

  async clickExplore() {
    await this.page.click(this.selectors.exploreLink);
  }

  async logout() {
    await this.page.click(this.selectors.logoutButton);
  }

  async getWelcomeMessage(): Promise<string> {
    return await this.page.textContent(this.selectors.welcomeMessage) || '';
  }

  async getStacksCount(): Promise<number> {
    const stacks = await this.page.locator(this.selectors.stackCard).count();
    return stacks;
  }

  async isEmptyStateVisible(): Promise<boolean> {
    return await this.page.isVisible(this.selectors.emptyState);
  }

  async waitForDashboard() {
    await this.page.waitForSelector(this.selectors.myStacksHeading);
  }

  async isOnDashboard(): Promise<boolean> {
    return this.page.url().includes('/dashboard');
  }

  async getStackNames(): Promise<string[]> {
    const stackElements = await this.page.locator('[data-testid="stack-card"] h3').all();
    const names: string[] = [];
    for (const element of stackElements) {
      const text = await element.textContent();
      if (text) names.push(text);
    }
    return names;
  }

  async clickStackByName(name: string) {
    await this.page.click(`[data-testid="stack-card"]:has-text("${name}")`);
  }
}