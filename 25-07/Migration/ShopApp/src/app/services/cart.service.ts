import { Inject, Injectable } from "@angular/core";
import { CartModel } from "../models/cart.model";
import { CustomerDataModel } from "../models/customerdata.model";
import { HttpClient } from "@angular/common/http";

@Injectable()
export class CartService {
    private cart: Map<number, number> = new Map<number,number>();

    constructor(private http: HttpClient){}

    addToCart(productId : number){
        let count = this.cart.get(productId);
        if(count == 0 || count == undefined || count == null){
            this.cart.set(productId,1);
        }else{

            this.cart.set(productId,count+1);
        }
    }

    getCart(){
        return this.cart;
    }
    clearCart(){
        this.cart.clear();
    }

    private apiShoppingCartUrl = 'http://localhost:5043/api/ShoppingCart'
    checkout(customer : CustomerDataModel){
        let checkoutItems: CartModel[] = []
        this.cart.forEach((value,key) => {
            checkoutItems.push(new CartModel(key,value));
        })
        return this.http.post(this.apiShoppingCartUrl+`/Checkout`,{...customer,cartItems : checkoutItems});
    }
}