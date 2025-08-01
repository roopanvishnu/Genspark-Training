import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs";
import { UserModel } from "../models/user.model";
import { Router } from "@angular/router";

@Injectable()
export class UserService {
    constructor(private http : HttpClient, private router : Router){}

    private apiUserUrl = 'http://localhost:5043/api/User';
    private apiAuthUrl = 'http://localhost:5043/api/Auth';

    private userSubject = new BehaviorSubject<UserModel | null>(null);
    public user$  = this.userSubject.asObservable();

    login(username : string, password : string){
        this.http.post(this.apiAuthUrl+'/Login',{username, password}).subscribe({
            next : (data : any) => {
                this.userSubject.next(data as UserModel);
                this.router.navigateByUrl("/");
            }
        })
    }
    signup(username : string, password : string){
        this.http.post(this.apiUserUrl+'/Create',{username, password}).subscribe({
            next : (data : any) => {
                alert("User Added Successfully");
                this.router.navigateByUrl("/login");
            }
        })
    }
    logout(){
        this.userSubject.next(null);
    }

}