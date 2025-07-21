import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { Product } from "../models/product";


@Injectable()
export class ProductService{
    private http = inject(HttpClient)

    getproduct(id:number = 1){
        return this.http.get<Product>("https://dummyjson.com/products/" + id);
    }
}