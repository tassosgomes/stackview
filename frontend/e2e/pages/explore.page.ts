import type { Page } from '@playwright/test';
import { BasePage } from './base.page';

export class ExplorePage extends BasePage {
  private readonly selectors = {
    searchInput: '[data-testid="search-input"]',
    typeFilter: '[data-testid="type-filter"]',
    technologyFilter: '[data-testid="technology-filter"]',
    stackCard: '[data-testid="stack-card"]',
    stackTitle: '[data-testid="stack-title"]',
    stackType: '[data-testid="stack-type"]',
    stackTechnologies: '[data-testid="stack-technologies"]',
    loadMoreButton: '[data-testid="load-more-button"]',
    noResultsMessage: '[data-testid="no-results-message"]',
    
    // Fallback selectors
    searchField: 'input[placeholder*="Buscar"]',
    filterButton: 'button:has-text("Filtrar")'
  };

  constructor(page: Page) {
    super(page);
  }

  async navigate() {
    await super.navigate('/explore');
  }

  async search(query: string) {
    const searchSelector = await this.page.isVisible(this.selectors.searchInput)
      ? this.selectors.searchInput
      : this.selectors.searchField;
    await this.page.fill(searchSelector, query);
    await this.page.press(searchSelector, 'Enter');
  }

  async filterByType(type: string) {
    await this.page.selectOption(this.selectors.typeFilter, type);
  }

  async filterByTechnology(technology: string) {
    await this.page.fill(this.selectors.technologyFilter, technology);
    await this.page.press(this.selectors.technologyFilter, 'Enter');
  }

  async getStacksCount(): Promise<number> {
    return await this.page.locator(this.selectors.stackCard).count();
  }

  async getStackNames(): Promise<string[]> {
    const stackElements = await this.page.locator(this.selectors.stackTitle).all();
    const names: string[] = [];
    for (const element of stackElements) {
      const text = await element.textContent();
      if (text) names.push(text.trim());
    }
    return names;
  }

  async clickStackByIndex(index: number) {
    await this.page.click(`${this.selectors.stackCard}:nth-child(${index + 1})`);
  }

  async clickStackByName(name: string) {
    await this.page.click(`${this.selectors.stackCard}:has-text("${name}")`);
  }

  async loadMore() {
    await this.page.click(this.selectors.loadMoreButton);
  }

  async isNoResultsVisible(): Promise<boolean> {
    return await this.page.isVisible(this.selectors.noResultsMessage);
  }

  async waitForStacks() {
    await this.page.waitForSelector(this.selectors.stackCard);
  }

  async isOnExplorePage(): Promise<boolean> {
    return this.page.url().includes('/explore');
  }

  async getStackTechnologies(stackIndex: number): Promise<string[]> {
    const techElement = this.page.locator(`${this.selectors.stackCard}:nth-child(${stackIndex + 1}) ${this.selectors.stackTechnologies}`);
    const text = await techElement.textContent();
    if (!text) return [];
    
    // Assuming technologies are comma-separated or in tags
    return text.split(',').map(t => t.trim()).filter(Boolean);
  }

  async hasStackWithTechnology(technology: string): Promise<boolean> {
    const stacks = await this.page.locator(this.selectors.stackCard).all();
    for (const stack of stacks) {
      const techText = await stack.locator(this.selectors.stackTechnologies).textContent();
      if (techText && techText.toLowerCase().includes(technology.toLowerCase())) {
        return true;
      }
    }
    return false;
  }
}