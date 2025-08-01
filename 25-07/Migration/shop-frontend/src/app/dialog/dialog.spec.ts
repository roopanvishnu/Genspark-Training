import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Dialog } from './dialog';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogTitle, MatDialogContent, MatDialogActions, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

describe('DeleteDocumentDialog', () => {
  let component: Dialog;
  let fixture: ComponentFixture<Dialog>;
  let dialogRefSpy :any;
  let matDialogDataStub : any;

  beforeEach(async () => {
    dialogRefSpy = jasmine.createSpyObj('MatDialogRef', ['close']);
    matDialogDataStub = {
      message: "Want to delete",
      onAccept: jasmine.createSpy('onAccept')
    };

    await TestBed.configureTestingModule({
      imports: [
        Dialog,
        MatFormFieldModule,
        MatInputModule,
        FormsModule,
        MatButtonModule,
        MatDialogTitle,
        MatDialogContent,
        MatDialogActions
      ],
      providers: [
        { provide: MatDialogRef, useValue: dialogRefSpy },
        { provide: MAT_DIALOG_DATA, useValue: matDialogDataStub },

      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Dialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should call onAccept when Yes is clicked', () => {
    component.onYesClick();
    expect(matDialogDataStub.onAccept).toHaveBeenCalled();
    expect(dialogRefSpy.close).toHaveBeenCalled();
  });
});
