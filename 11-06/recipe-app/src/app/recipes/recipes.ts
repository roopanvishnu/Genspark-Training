import { Component, Signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RecipeService } from '../services/recipe';
import { Recipe } from '../models/recipe';

@Component({
  selector: 'app-recipes',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './recipes.html',
  styleUrls: ['./recipes.css'],
})
export class RecipesComponent {
  recipes: Signal<readonly Recipe[]>;

  constructor(private recipeService: RecipeService) {
    this.recipes = this.recipeService.recipes;
  }

  clearAll() {
    this.recipeService.clearRecipes();
  }
}
