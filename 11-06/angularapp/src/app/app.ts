import { Component } from '@angular/core';
import { Product } from './product/product'; // ✅ Import your Product component

@Component({
  selector: 'app-root',
  standalone: true, // ✅ Required for standalone components
  imports: [Product], // ✅ Declare imported components here
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class App {
  protected title = 'angularapp';
}
