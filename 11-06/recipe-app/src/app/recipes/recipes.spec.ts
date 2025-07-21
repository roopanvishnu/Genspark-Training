import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RecipesComponent } from './recipes';
import { RecipeService } from '../services/recipe';
import { Recipe } from '../models/recipe';
import { signal } from '@angular/core';

class MockRecipeService {
  recipes = signal<Recipe[]>([
    {
      id: 1,
      name: 'Pasta',
      cuisine: 'Italian',
      difficulty: 'Easy',
      ingredients: ['noodles', 'sauce'],
      instructions: ['Boil', 'Cook', 'Serve'],
      prepTimeMinutes: 10,
      cookTimeMinutes: 20,
      servings: 2,
      caloriesPerServing: 300,
      rating: 4.5,
      reviewCount: 12,
      mealType: ['Lunch'],
      image: '',
      tags: ['vegetarian'],
      userId: 1,
    },
  ]);

  clearRecipes = jasmine.createSpy('clearRecipes');
}

describe('RecipesComponent', () => {
  let component: RecipesComponent;
  let fixture: ComponentFixture<RecipesComponent>;
  let mockService: MockRecipeService;

  beforeEach(async () => {
    mockService = new MockRecipeService();

    await TestBed.configureTestingModule({
      imports: [RecipesComponent],
      providers: [{ provide: RecipeService, useValue: mockService }],
    }).compileComponents();

    fixture = TestBed.createComponent(RecipesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges(); // trigger ngOnInit/constructor logic
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should get recipes from the service', () => {
    const recipes = component.recipes();
    expect(recipes.length).toBe(1);
    expect(recipes[0].name).toBe('Pasta');
  });

  it('should call clearRecipes on the service when clearAll() is called', () => {
    component.clearAll();
    expect(mockService.clearRecipes).toHaveBeenCalled();
  });
});
