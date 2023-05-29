import { Component, Inject, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { UserService } from 'src/app/core/services/user.service';
import { AuthenticationService } from 'src/app/core/services/auth.service';
import { CurrentUser } from 'src/app/core/models/currentUser';
import { NotificationService } from 'src/app/core/services/notification.service';

@Component({
  selector: 'app-reset-password-modal',
  templateUrl: './reset-password-modal.component.html',
  styleUrls: ['./reset-password-modal.component.css']
})
export class ResetPasswordModal implements OnInit {

  user: CurrentUser;

  userForm!: FormGroup;
  hideNew: boolean = true;
  hideConfirm: boolean = true;
  constructor(
    public dialogRef: MatDialogRef<ResetPasswordModal>,
    private fb: FormBuilder,
    private authService: AuthenticationService,
    private userService: UserService,
    private notificationService: NotificationService
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
    const password = this.userForm.get('password')?.value;
    const confirmPassword = this.userForm.get('confirmPassword')?.value;
    if (password == confirmPassword) {
      this.userService.updateUserPassword(this.user.id!, password).subscribe(res => {
        if (res.code == 200) {
          this.dialogRef.close();
        } else {
          this.notificationService.openSnackBar(res.message);
        }
      });
    }

  }

  private createForm() {
    this.userForm = this.fb.group({
      // oldPassword: new FormControl('', Validators.required),
      password: new FormControl('', Validators.required),
      confirmPassword: new FormControl('', Validators.required)
    }, { validators: this.ConfirmedValidator('password', 'confirmPassword'), });
  }

  private ConfirmedValidator(controlName: string, matchingControlName: string) {
    return (formGroup: FormGroup) => {
      const control = formGroup.controls[controlName];
      const matchingControl = formGroup.controls[matchingControlName];
      if (
        matchingControl.errors &&
        !matchingControl.errors.confirmedValidator
      ) {
        return;
      }
      if (control.value !== matchingControl.value) {
        matchingControl.setErrors({ confirmedValidator: true });
      } else {
        matchingControl.setErrors(null);
      }
    };
  }
  // private checkOldPassword(oldPassword: string): boolean {
  //   //TODO: create API endpoint to check if old password is correct
  //   return true;
  // }
}
