import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";

@Injectable()
export class OrderDetailService {
    constructor(
        private http : HttpClient
    ){}

    private apiUrl = 'http://localhost:5043/api/OrderDetail'
    getAll(){
        return this.http.get(this.apiUrl);
    }
}