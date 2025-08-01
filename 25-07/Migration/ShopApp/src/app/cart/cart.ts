import { Component } from '@angular/core';
import { CartService } from '../services/cart.service';
import { ProductService } from '../services/product.service';
import { CartDisplayModel } from '../models/cartdisplay.model';
import { CategoryService } from '../services/category.service';
import { ColorService } from '../services/color.service';
import { ColorModel } from '../models/color.model';
import { CategoryModel } from '../models/category.model';
import { MatTableModule } from '@angular/material/table';
import { MatFormField, MatInputModule } from '@angular/material/input';
import { MAT_FORM_FIELD_DEFAULT_OPTIONS, MatFormFieldControl, MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { CustomerDataModel } from '../models/customerdata.model';

@Component({
    selector: 'app-cart',
    imports: [
        CommonModule,
        FormsModule,
        MatTableModule,
        MatInputModule,
        MatFormFieldModule,
        MatButtonModule
    ],
    templateUrl: './cart.html',
    styleUrl: './cart.css',
    providers : [
        {provide: MAT_FORM_FIELD_DEFAULT_OPTIONS, useValue: {appearance: 'outline'}}
    ]
})
export class Cart {
    cartItems: CartDisplayModel[] = [];
    colors: ColorModel[] = [];
    categories: CategoryModel[] = [];
    displayedColumns: string[] = ['productName', 'category', 'color', 'price', 'quantity', 'total'];

    customer = new CustomerDataModel();

    constructor(
        private cartService: CartService,
        private productService: ProductService,
        private categoryService: CategoryService,
        private colorService: ColorService,
    ) {
        this.loadData();
    }

    // loadData(){

    //     this.categoryService.getAll().subscribe({
    //         next: (data : any ) => {
    //             this.categories = data;
    //         }
    //     })
    //     this.colorService.getAll().subscribe({
    //         next: (data : any ) => {
    //             this.colors = data;
    //         }
    //     })
    //     // this.productService.
    //     this.cartItems = [];
    //     this.cartService.getCart().forEach((value,key,map) =>{
    //         let item = new CartDisplayModel(key,value);
    //         this.productService.getDetailsById(key).subscribe({
    //             next : (data : any) => {
    //                 item.productName = data.productName;
    //                 item.price = data.price;
    //                 item.categoryId = data.categoryId;
    //                 item.colorId = data.colorId;

    //                 this.cartItems.push(item);
    //                 console.log(this.cartItems);
    //             }
    //         })
    //     })
    // }

    async loadData() {
        const categoriesPromise = this.categoryService.getAll().toPromise();
        const colorsPromise = this.colorService.getAll().toPromise();

        const [categoriesData, colorsData] = await Promise.all([categoriesPromise, colorsPromise]);

        this.categories = categoriesData as CategoryModel[];
        this.colors = colorsData as ColorModel[];

        const cartMap = this.cartService.getCart(); // Map<number, number>

        const cartItemPromises: Promise<CartDisplayModel>[] = [];

        cartMap.forEach((quantity, productId) => {
            const item = new CartDisplayModel(productId, quantity);
            const promise = this.productService.getDetailsById(productId).toPromise().then((data: any) => {
                item.productName = data.productName;
                item.price = data.price;
                item.categoryId = data.categoryId;
                item.colorId = data.colorId;
                return item;
            });
            cartItemPromises.push(promise);
        });

        this.cartItems = await Promise.all(cartItemPromises);
        console.log(this.cartItems);
    }


    getColorName(colorId: number | null): string {
        const color = this.colors.find(c => c.colorId === colorId);
        return color ? color.color1 : '-';
    }

    getCategoryName(categoryId: number | null): string {
        const category = this.categories.find(c => c.categoryId === categoryId);
        return category ? category.name : '-';
    }
    checkout() {
        this.cartService.checkout(this.customer).subscribe({
            next : (data : any) => {
                alert("Order placed successfully");
                this.cartService.clearCart();
            }
        })
     }
}
