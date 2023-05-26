import { Component, Inject, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { UserService } from 'src/app/core/services/user.service';
import { AuthenticationService } from 'src/app/core/services/auth.service';
import { CurrentUser } from 'src/app/core/models/currentUser';

@Component({
  selector: 'app-reset-password-modal',
  templateUrl: './reset-password-modal.component.html',
  styleUrls: ['./reset-password-modal.component.css']
})
export class ResetPasswordModal implements OnInit {

  user: CurrentUser;

  userForm!: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<ResetPasswordModal>,
    private fb: FormBuilder,
    private  authService: AuthenticationService,
    private  userService: UserService
  ) {
    this.user = this.authService.getCurrentUser();
  }

  ngOnInit() {
    this.createForm();
  }

  cancel() {
    this.dialogRef.close();
  }

  submit() {
    //TODO: check if old password is correct in api
    const oldPassword = this.userForm.get('oldPassword')?.value;
    const password = this.userForm.get('password')?.value;
    const confirmPassword = this.userForm.get('confirmPassword')?.value;
    if(password == confirmPassword){
        this.userService.updateUserPassword(this.user.id!, password).subscribe(() =>{
          this.dialogRef.close();
        });
    }
  }

  private createForm() {
    this.userForm = this.fb.group({
      oldPassword: new FormControl('', Validators.required),
      password: new FormControl('', Validators.required),
      confirmPassword: new FormControl('', Validators.required)
    }, { validators: this.checkPasswords });

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

}
