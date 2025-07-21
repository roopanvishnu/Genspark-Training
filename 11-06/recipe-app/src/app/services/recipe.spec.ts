import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { RecipeService } from './recipe';
import { RecipeResponse } from '../models/recipe';

describe('RecipeService', () => {
  let service: RecipeService;
  let httpMock: HttpTestingController;

  const mockResponse: RecipeResponse = {
    recipes: [
      {
        id: 1,
        name: 'Pasta',
        cuisine: 'Italian',
        difficulty: 'Easy',
        ingredients: ['noodles', 'sauce'],
        instructions: ['Boil water', 'Add pasta', 'Stir sauce'],
        prepTimeMinutes: 10,
        cookTimeMinutes: 15,
        servings: 2,
        caloriesPerServing: 300,
        rating: 4.5,
        reviewCount: 10,
        mealType: ['Lunch'],
        image: 'image-url',
        tags: ['vegetarian'],
        userId: 1,
      },
    ],
    total: 1,
    skip: 0,
    limit: 30,
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [RecipeService],
    });

    service = TestBed.inject(RecipeService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch recipes and update signal', () => {
    const req = httpMock.expectOne('https://dummyjson.com/recipes');
    expect(req.request.method).toBe('GET');

    req.flush(mockResponse);

    const recipes = service.recipes();
    expect(recipes.length).toBe(1);
    expect(recipes[0].name).toBe('Pasta');
  });

  it('should handle error and set recipes to empty array', () => {
    const req = httpMock.expectOne('https://dummyjson.com/recipes');
    req.error(new ErrorEvent('Network error'));

    const recipes = service.recipes();
    expect(recipes).toEqual([]);
  });

  it('should clear recipes', () => {
    // Manually set initial recipes
    (service as any)._recipes.set(mockResponse.recipes);

    expect(service.recipes().length).toBe(1);
    service.clearRecipes();
    expect(service.recipes()).toEqual([]);
  });
});
