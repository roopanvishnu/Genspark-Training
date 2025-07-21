import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Recipe, RecipeResponse } from '../models/recipe';

@Injectable({
  providedIn: 'root',
})
export class RecipeService {
  private _recipes = signal<Recipe[]>([]);
  private readonly API_URL = 'https://dummyjson.com/recipes';

  constructor(private http: HttpClient) {
    this.fetchRecipes();
  }

  get recipes() {
    return this._recipes.asReadonly();
  }

  fetchRecipes(): void {
    this.http.get<RecipeResponse>(this.API_URL).subscribe({
      next: (res) =>{
        console.log(res),
        this._recipes.set(res.recipes)
      },
      error: (err) => {
        console.error('Failed to fetch recipes:', err);
        this._recipes.set([]);
      },
    });
    console.log("completed fetching")
  }

  clearRecipes(): void {
    this._recipes.set([]);
  }
}


