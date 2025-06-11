import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";

@Injectable()
export class ProductService{
    private http = inject(HttpClient);

    getProduct(id:number=1){
        return this.http.get('https://dummyjson.com/products/'+id)
    }
}