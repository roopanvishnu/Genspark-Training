import { Component, signal } from '@angular/core';
import { ProductModel } from '../models/product.model';
import { ProductSearchModel } from '../models/productsearch.model';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { BehaviorSubject, catchError, debounceTime, of, switchMap, tap } from 'rxjs';
import { ProductService } from '../services/product.service';
import { CategoryService } from '../services/category.service';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { AsyncPipe, DatePipe, JsonPipe } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatCardModule } from '@angular/material/card';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MAT_FORM_FIELD_DEFAULT_OPTIONS, MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatTabsModule } from '@angular/material/tabs';
import { UserModel } from '../models/user.model';
import { Navbar } from "../navbar/navbar";
import { CartService } from '../services/cart.service';
import { P } from '@angular/cdk/keycodes';
import { Router } from '@angular/router';
import { UserService } from '../services/user.service';
import { provideNativeDateAdapter } from '@angular/material/core';
import { ProductAddDTO } from '../models/productadddto';

interface selectInterface {
    value : any,
    view: string
}

@Component({
	selector: 'app-products',
	imports: [
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatIconModule,
    MatDatepickerModule,
    MatInputModule,
    MatButtonModule,
    MatButtonToggleModule,
    MatSelectModule,
    MatAutocompleteModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    MatCardModule,
    MatTabsModule,
    MatPaginatorModule,
],
	templateUrl: './products.html',
	styleUrl: './products.css',
	providers: [
		provideNativeDateAdapter(),
		{provide: MAT_FORM_FIELD_DEFAULT_OPTIONS, useValue: {appearance: 'outline'}},
	]
})
export class Products {
	errorMessage: string = "";
	products: ProductModel[] = [];
	currentUser: UserModel | null = null;
	productsearch: ProductSearchModel = new ProductSearchModel(1);
	private snackbar = new MatSnackBar();
	categoryList: selectInterface[] = [];


	productSearchSubject = new BehaviorSubject<ProductSearchModel>(this.productsearch);


	constructor(
		private productService: ProductService,
		private categoryService: CategoryService,
		private cartService: CartService,
		private userService: UserService,
		private router : Router
	) 
	{
		this.userService.user$.subscribe({
			next : (data : any) =>{
				this.currentUser = data;
			}
		})
		this.loadData();
		this.productSearchSubject.next(this.productsearch);
	}

	loadData() {
		this.categoryService.getAll().subscribe({
			next : (data : any) => {
				this.categoryList = [];
				this.categoryList.push({value: null, view : "All"});
				console.log(data);
				data.forEach((c : any) => {
					this.categoryList.push({value: c.categoryId, view: c.name});
				});
			}
		})
	}

	onValueChange() {
		this.productSearchSubject.next(this.productsearch);
		console.log(this.productsearch);
	}

	total = 0;
	handlePageEvent(e: PageEvent) {

		// this.productsearch.pageSize = e.pageSize;
		this.productsearch.page = e.pageIndex + 1;
		this.productSearchSubject.next(this.productsearch);
	}
		
	addToCart(productId : number){
		this.cartService.addToCart(productId);
		alert("Added to the Cart");
		console.log(this.cartService.getCart());
	}

	buyNow(productId : number){
		this.cartService.addToCart(productId);
		this.router.navigateByUrl("/cart");
	}

	ngOnInit() {

		this.productSearchSubject.pipe(
			debounceTime(500),
			tap(() => { console.log("API Called") }),
			switchMap((query: ProductSearchModel) => this.productService.getProductsByPage( query).pipe(
				catchError((err) => {
					console.error('API error:', err);
					if (err) {
						this.errorMessage = err.message;
						this.snackbar.open(err.message, undefined, { duration: 2000 })
					}
					// Return empty result or fallback

					return of({ data: { $values: [] }});
				})
			))
		).subscribe({
			next: (res: any) => {
				// console.log(res);
				this.products = res as ProductModel[];
				this.total = res?.pagination?.totalRecords ?? 0;
				console.log(this.products);
			},
			error: (err) => {
				console.log(err);
			}
		})
	}




	showModal = false;
selectedProduct: ProductModel | null = null;

productFormModel: ProductModel = new ProductModel();

openModal(product?: ProductModel) {
  this.selectedProduct = product ?? null;

  if (product) {
    this.productFormModel = product;
  }
  this.showModal = true;
}

closeModal() {
  this.showModal = false;
  this.selectedProduct = null;
}

saveProduct() {
	// console.log(this.productFormModel);
  if (this.selectedProduct) {
    this.productService.update(this.selectedProduct.productId, ProductAddDTO.fromModel(this.productFormModel),this.currentUser as UserModel).subscribe(() => {
      this.snackbar.open("Product updated!", "", { duration: 2000 });
      this.closeModal();
      this.productSearchSubject.next(this.productsearch);
    });
  } else {
    this.productService.create(ProductAddDTO.fromModel(this.productFormModel), this.currentUser as UserModel).subscribe(() => {
      this.snackbar.open("Product created!", "", { duration: 2000 });
      this.closeModal();
      this.productSearchSubject.next(this.productsearch);
    });
  }
}


}
