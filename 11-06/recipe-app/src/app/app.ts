import { Component } from '@angular/core';
import { RecipesComponent } from './recipes/recipes';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RecipesComponent],
  template: `<app-recipes />`,
})
export class App {}
