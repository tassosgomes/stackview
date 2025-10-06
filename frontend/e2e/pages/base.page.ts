import type { Page } from '@playwright/test';

export abstract class BasePage {
  protected readonly page: Page;

  constructor(page: Page) {
    this.page = page;
  }

  async navigate(path: string = '/') {
    await this.page.goto(path);
  }

  async waitForPage() {
    await this.page.waitForLoadState('networkidle');
  }

  async fillForm(selector: string, value: string) {
    await this.page.fill(selector, value);
  }

  async clickButton(selector: string) {
    await this.page.click(selector);
  }

  async getText(selector: string): Promise<string> {
    return await this.page.textContent(selector) || '';
  }

  async isVisible(selector: string): Promise<boolean> {
    return await this.page.isVisible(selector);
  }

  async waitForSelector(selector: string) {
    await this.page.waitForSelector(selector);
  }

  async screenshot(name: string) {
    await this.page.screenshot({ path: `e2e/screenshots/${name}.png` });
  }

  async getCurrentUrl(): Promise<string> {
    return this.page.url();
  }

  async waitForUrlPattern(pattern: RegExp) {
    await this.page.waitForURL(pattern);
  }

  async evaluateInPage<T>(fn: () => T): Promise<T> {
    return await this.page.evaluate(fn);
  }
}