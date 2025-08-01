import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";

@Injectable()
export class CategoryService {
    constructor( private http : HttpClient){}

    private apiUrl = "http://localhost:5043/api/Category";

    getAll(){
        return this.http.get(this.apiUrl+'/all');
    }
    create(name : string){
        return this.http.post(this.apiUrl+'/Create?name='+name,{});
    }
    update(id: number, name : string){
        return this.http.post(this.apiUrl+'/Edit/'+id+'?name='+name,{});
    }
    delete(id: number){
        return this.http.post(this.apiUrl+'/Delete/'+id,{});
    }
}