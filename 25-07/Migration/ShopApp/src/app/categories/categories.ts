import { Component } from '@angular/core';
import { CategoryModel } from '../models/category.model';
import { CategoryService } from '../services/category.service';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MAT_FORM_FIELD_DEFAULT_OPTIONS, MatFormFieldModule } from '@angular/material/form-field';
import { Dialog } from '../dialog/dialog';
import { MatDialog } from '@angular/material/dialog';

@Component({
	selector: 'app-categories',
	imports: [
		FormsModule,
		MatTableModule,
		MatIconModule,
		MatButtonModule,
		MatInputModule,
		MatFormFieldModule
	],
	templateUrl: './categories.html',
	styleUrl: './categories.css',
	providers :[
    	{provide: MAT_FORM_FIELD_DEFAULT_OPTIONS, useValue: {appearance: 'outline'}}		
	]
})
export class Categories {
	displayedColumns: string[] = ['Id',"Name","Operations"];
    categories : CategoryModel[] = []

	constructor(private categoryService : CategoryService, private dialog : MatDialog){
		this.loadData();
	}
	loadData(){
		this.categoryService.getAll().subscribe({
			next: (data : any) => {
				this.categories = data;
				this.categories.sort(c => c.categoryId).reverse();
				console.log(this.categories);
			}
		})
	}

	  onDelete(id : number) {
		this.categoryService.delete(id).subscribe({
		  next : () =>{
			this.loadData();
		  }
		})
	  }
	  openDeleteDialog(message : string, id : number){
		this.dialog.open(Dialog,{
			data : {
				message : `Want to delete ${message}`, 
				onAccept : ()=>{
					this.onDelete(id);
					alert(`Document ${message} deleted successfully!`);
			}
		  }
		})
	  }

	showModal = false;
	editingCategory: CategoryModel | null = null;

	categoryFormName : string = "";

	openModal(category?: CategoryModel) {
		this.editingCategory = category ?? null;
		this.categoryFormName = category?.name ?? '';
		this.showModal = true;
	}

	closeModal() {
		this.showModal = false;
		this.editingCategory = null;
		this.categoryFormName = '';
	}

	onSubmit() {
		if (this.editingCategory) {
			const updated = this.categoryFormName;
			this.categoryService.update(this.editingCategory.categoryId,updated).subscribe(() => {
				this.closeModal();
				this.loadData();
			});
		} else {
			// Create new category
			this.categoryService.create(this.categoryFormName).subscribe(() => {
				this.closeModal();
				this.loadData();
			});
		}
	}
}
