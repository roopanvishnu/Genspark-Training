export interface Recipe {
  id: number;
  name: string;
  cuisine: string;
  difficulty: string;
  ingredients: string[];
  instructions: string[];
  prepTimeMinutes: number;
  cookTimeMinutes: number;
  servings: number;
  caloriesPerServing: number;
  rating: number;
  reviewCount: number;
  mealType: string[];
  image: string;
  tags: string[];
  userId: number;
}

export interface RecipeResponse {
  recipes: Recipe[];
  total: number;
  skip: number;
  limit: number;
}
