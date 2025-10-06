import type { Page } from '@playwright/test';
import { BasePage } from './base.page';

export class StackCreatePage extends BasePage {
  private readonly selectors = {
    nameInput: '[data-testid="stack-name-input"]',
    typeSelect: '[data-testid="stack-type-select"]',
    descriptionTextarea: '[data-testid="stack-description-textarea"]',
    technologyInput: '[data-testid="technology-input"]',
    technologySuggestion: '[data-testid="technology-suggestion"]',
    selectedTechnology: '[data-testid="selected-technology"]',
    isPublicCheckbox: '[data-testid="is-public-checkbox"]',
    saveButton: '[data-testid="save-stack-button"]',
    cancelButton: '[data-testid="cancel-button"]',
    
    // Fallback selectors based on common patterns
    nameField: 'input[name="name"]',
    typeField: 'select[name="type"]',
    descriptionField: 'textarea[name="description"]',
    submitButton: 'button[type="submit"]'
  };

  constructor(page: Page) {
    super(page);
  }

  async navigate() {
    await super.navigate('/stacks/create');
  }

  async fillName(name: string) {
    const nameSelector = await this.page.isVisible(this.selectors.nameInput)
      ? this.selectors.nameInput
      : this.selectors.nameField;
    await this.page.fill(nameSelector, name);
  }

  async selectType(type: string) {
    const typeSelector = await this.page.isVisible(this.selectors.typeSelect)
      ? this.selectors.typeSelect
      : this.selectors.typeField;
    await this.page.selectOption(typeSelector, type);
  }

  async fillDescription(description: string) {
    const descriptionSelector = await this.page.isVisible(this.selectors.descriptionTextarea)
      ? this.selectors.descriptionTextarea
      : this.selectors.descriptionField;
    await this.page.fill(descriptionSelector, description);
  }

  async addTechnology(technology: string) {
    await this.page.fill(this.selectors.technologyInput, technology);
    
    // Wait for suggestions to appear and click the first one
    try {
      await this.page.waitForSelector(this.selectors.technologySuggestion, { timeout: 2000 });
      await this.page.click(`${this.selectors.technologySuggestion}:first-child`);
    } catch {
      // If no suggestions, just press Enter to add as new technology
      await this.page.press(this.selectors.technologyInput, 'Enter');
    }
  }

  async setPublic(isPublic: boolean) {
    const checkbox = this.page.locator(this.selectors.isPublicCheckbox);
    const isChecked = await checkbox.isChecked();
    if (isChecked !== isPublic) {
      await checkbox.click();
    }
  }

  async save() {
    const saveSelector = await this.page.isVisible(this.selectors.saveButton)
      ? this.selectors.saveButton
      : this.selectors.submitButton;
    await this.page.click(saveSelector);
  }

  async cancel() {
    await this.page.click(this.selectors.cancelButton);
  }

  async createStack(stackData: {
    name: string;
    type: string;
    description: string;
    technologies: string[];
    isPublic?: boolean;
  }) {
    await this.fillName(stackData.name);
    await this.selectType(stackData.type);
    await this.fillDescription(stackData.description);
    
    for (const tech of stackData.technologies) {
      await this.addTechnology(tech);
    }
    
    if (stackData.isPublic !== undefined) {
      await this.setPublic(stackData.isPublic);
    }
    
    await this.save();
  }

  async waitForForm() {
    await this.page.waitForSelector(this.selectors.nameField);
  }

  async isOnCreatePage(): Promise<boolean> {
    return this.page.url().includes('/stacks/create');
  }

  async getSelectedTechnologies(): Promise<string[]> {
    const techElements = await this.page.locator(this.selectors.selectedTechnology).all();
    const technologies: string[] = [];
    for (const element of techElements) {
      const text = await element.textContent();
      if (text) technologies.push(text.trim());
    }
    return technologies;
  }
}