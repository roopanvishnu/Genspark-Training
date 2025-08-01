import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";

@Injectable()
export class ColorService {
    constructor( private http : HttpClient){}

    private apiUrl = "http://localhost:5043/api/Color";

    getAll(){
        return this.http.get(this.apiUrl);
    }
    create(name : string){
        return this.http.post(this.apiUrl+'/Create?colorName='+name,{});
    }
    update(id: number, name : string){
        return this.http.post(this.apiUrl+'/Edit/'+id+'?newColorName='+name,{});
    }
    delete(id: number){
        return this.http.post(this.apiUrl+'/Delete/'+id,{});
    }
}