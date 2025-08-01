import { Component, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatToolbarModule } from '@angular/material/toolbar';
import { ActivatedRoute, Router } from '@angular/router';
import { UserModel } from '../models/user.model';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-navbar',
  imports: [MatToolbarModule, MatIconModule, MatButtonModule,MatMenuModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css'
})
export class Navbar {
	currentUrl = signal("");
	currentUser : UserModel | null = null;
	constructor(private route : ActivatedRoute, private router : Router, private userService : UserService){
		// console.log(this.route.snapshot.url.toString());
		this.currentUrl.set(this.route.snapshot.url.toString());
		this.userService.user$.subscribe({
			next : (data : any) => {
				this.currentUser = data;
			}
		})
	}
	navigate(url : string){
		this.router.navigateByUrl(url);
		this.currentUrl.set(url.substring(1));
		// console.log(this.currentUrl());
	}
	logout(){
		this.userService.logout();	
	}
}
