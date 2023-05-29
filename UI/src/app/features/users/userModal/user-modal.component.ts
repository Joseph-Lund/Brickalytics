import { Component, Inject, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { GenericType } from 'src/app/core/models/genericType';
import { User } from 'src/app/core/models/user';
import { UserRatesModal } from '../userRatesModal/user-rates-modal.component';
import { UserService } from 'src/app/core/services/user.service';

@Component({
  selector: 'app-user-modal',
  templateUrl: './user-modal.component.html',
  styleUrls: ['./user-modal.component.css']
})
export class UserModal implements OnInit {

  user: User;
  roleList: GenericType[] = [];
  collectionList: GenericType[] = [];
  activeStateList: any[] = [{ id: true, name: "True" }, { id: false, name: "False" }];

  userForm!: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<UserModal>,
    private userService: UserService,
    private fb: FormBuilder,
    private dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.user = data.user;
    this.roleList = data.roleList;
    this.collectionList = data.collectionList;
  }

  ngOnInit() {
    this.createForm();
  }

  cancel() {
    this.dialogRef.close({ data: this.user });
  }

  submit() {
    const creatorName = this.userForm.get('creatorName')?.value;
    const password = this.userForm.get('password')?.value;
    const confirmPassword = this.userForm.get('confirmPassword')?.value;
    if(password == confirmPassword && (password != null || password != "" || confirmPassword != null || confirmPassword != "" )){
      this.userService.updateUserPassword(this.user.id!, password).subscribe(

      );
    }
    const active = this.userForm.get('active')?.value;
    const roleId = this.userForm.get('roleId')?.value;
    const collectionId = this.userForm.get('collectionId')?.value;
    if(password == confirmPassword){
      this.dialogRef.close({ data: new User(this.user.id, active, roleId, collectionId, creatorName, null) });
    }
  }
  openUserModal(){

    const dialogRef = this.dialog.open(UserRatesModal, {
      data: {user: this.user, roleList: this.roleList, collectionList: this.collectionList}
    });

    dialogRef.afterClosed().subscribe(_userRates => {

    });
  }

  private createForm() {
    this.userForm = this.fb.group({
      creatorName: new FormControl(this.user.creatorName, Validators.required),
      password: new FormControl(''),
      confirmPassword: new FormControl(''),
      roleId: new FormControl(this.user.roleId, Validators.required),
      active: new FormControl(this.user.active, Validators.required),
      collectionId: new FormControl(this.user.collectionId, Validators.required)
    }, { validators: [this.checkPasswords, this.checkColldectionId] });

  }

  private checkPasswords: ValidatorFn = (group: AbstractControl): ValidationErrors | null => {
    let pass = group.get('password')?.value;
    let confirmPass = group.get('confirmPassword')?.value;
    if(pass || confirmPass){
    return pass === confirmPass ? null : { notSame: true };
    }else{
      return null;
    }
  }
  private checkColldectionId: ValidatorFn = (group: AbstractControl): ValidationErrors | null => {
    let collectionId = group.get('collectionId')?.value;
    if(collectionId){
      return null;
    }else{
      return collectionId ? null : { collectionNotSelected: true };
    }
  }

}
