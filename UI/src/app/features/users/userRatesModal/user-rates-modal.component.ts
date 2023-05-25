import { Component, Inject, OnInit } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { User } from 'src/app/core/models/user';

@Component({
  selector: 'app-user-rates-modal',
  templateUrl: './user-rates-modal.component.html',
  styleUrls: ['./user-rates-modal.component.css']
})
export class UserRatesModal implements OnInit {

  user: User;
  userRatesForm!: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<UserRatesModal>,
    private fb: FormBuilder,
    private dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.user = data.user;
  }

  ngOnInit() {
    this.createForm();
  }

  cancel() {
    this.dialogRef.close({ data: null });
  }

  submit() {
    const creatorName = this.userRatesForm.get('creatorName')?.value;
    const password = this.userRatesForm.get('password')?.value;
    const confirmPassword = this.userRatesForm.get('confirmPassword')?.value;
    const active = this.userRatesForm.get('active')?.value;
    const roleId = this.userRatesForm.get('roleId')?.value;
    const collectionId = this.userRatesForm.get('collectionId')?.value;
    if(password == confirmPassword){
      this.dialogRef.close({ data: new User(this.user.id, active, roleId, collectionId, creatorName, null) });
    }
  }
  private createForm() {
    this.userRatesForm = this.fb.group({
      userRates: this.fb.array([]),
    });
  }

  addUserRate(){
    const creds = this.userRatesForm.controls.credentials as FormArray;
    creds.push(this.fb.group({
      userId: this.user.id,
      productTypeId: 0,
      rate: null,
      percent: null
    }));
  }

  saveUserRate(_rate: any){
    // if(_rate.userId)
  }

}
