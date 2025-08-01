import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogActions, MatDialogClose, MatDialogContent, MatDialogRef, MatDialogTitle } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-dialog',
  imports: [MatFormFieldModule,
    MatInputModule,
    FormsModule,
    MatButtonModule,
    MatDialogTitle,
    MatDialogContent,
    MatDialogActions
  ],
  templateUrl: './dialog.html',
  styleUrl: './dialog.css'
})
export class Dialog {
  dialogRef = inject(MatDialogRef<Dialog>);
  data = inject(MAT_DIALOG_DATA);
  
  onNoClick(){
    // console.log("No");
    this.dialogRef.close();
  }
  onYesClick(){
    // console.log("Yes");
    this.data.onAccept();
    this.dialogRef.close();
    
  }
}
