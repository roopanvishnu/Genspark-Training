import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule, MAT_FORM_FIELD_DEFAULT_OPTIONS } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { ColorModel } from '../models/color.model';
import { ColorService } from '../services/color.service';
import { Dialog } from '../dialog/dialog';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-colors',
  imports: [
		FormsModule,
		MatTableModule,
		MatIconModule,
		MatButtonModule,
		MatInputModule,
		MatFormFieldModule
  ],
  templateUrl: './colors.html',
  styleUrl: './colors.css',
  providers :[
    	{provide: MAT_FORM_FIELD_DEFAULT_OPTIONS, useValue: {appearance: 'outline'}}		
	]
})
export class Colors {
  displayedColumns: string[] = ['Id',"Color","Operations"];
    colors : ColorModel[] = []

  constructor(private colorService : ColorService, 	private dialog :MatDialog ){
    this.loadData();
  }
  loadData(){
    this.colorService.getAll().subscribe({
      next: (data : any) => {
        this.colors = data;
        this.colors.sort(c => c.colorId);
        console.log(this.colors);
      }
    })
  }

  onDelete(id : number) {
    this.colorService.delete(id).subscribe({
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
  editingColor: ColorModel | null = null;

  colorFormName : string = "";

  openModal(color?: ColorModel) {
    this.editingColor = color ?? null;
    this.colorFormName = color?.color1 ?? '';
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
    this.editingColor = null;
    this.colorFormName = '';
  }

  onSubmit() {
    if (this.editingColor) {
      const updated = this.colorFormName;
      this.colorService.update(this.editingColor.colorId,updated).subscribe(() => {
        this.closeModal();
        this.loadData();
      });
    } else {
      // Create new color
      this.colorService.create(this.colorFormName).subscribe(() => {
        this.closeModal();
        this.loadData();
      });
    }
  }
}
