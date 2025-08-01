import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ProductSearchModel } from "../models/productsearch.model";
import { ProductModel } from "../models/product.model";
import { UserModel } from "../models/user.model";
import { ProductAddDTO } from "../models/productadddto";

@Injectable()
export class ProductService {
    constructor( private http : HttpClient){

    }
    private apiUrl = 'http://localhost:5043/api/Product'

    getProductsByPage(query : ProductSearchModel){
        if(query.category!=null)
            return this.http.get(this.apiUrl+`?page=${query.page}&category=${query.category}`);
        else
            return this.http.get(this.apiUrl+`?page=${query.page}`);
    }
    getAll(){
        return this.http.get(this.apiUrl+`/all`);
    }
    getDetailsById(id : number){
        return this.http.get(this.apiUrl+`/Details/${id}`);
    }

    create(product : ProductAddDTO, user : UserModel){
        console.log(product);
        console.log(user);
        return this.http.post(this.apiUrl+'/Create', product, {
            headers : {
                "Authorization" : `Bearer ${user.accessToken}`
            }
        });
    }
    update(productId: number, product : ProductAddDTO, user : UserModel){
        return this.http.post(this.apiUrl+'/Edit/'+productId, product, {
            headers :{
                "Authorization" : `Bearer ${user.accessToken}`
            }
        });
    }
}