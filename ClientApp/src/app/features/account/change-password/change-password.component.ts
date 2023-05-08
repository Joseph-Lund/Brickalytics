import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from 'src/app/core/services/auth.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { SpinnerService } from 'src/app/core/services/spinner.service';


@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent implements OnInit {
  form!:FormGroup
  hideCurrentPassword: boolean;
  hideNewPassword: boolean;
  currentPassword!: string;
  newPassword!: string;
  newPasswordConfirm!: string;
  disableSubmit!: boolean;

  constructor(private authService: AuthenticationService,
    private spinnerService: SpinnerService,
    private notificationService: NotificationService) {

    this.hideCurrentPassword = true;
    this.hideNewPassword = true;
  }

  ngOnInit() {
    this.form = new FormGroup({
      currentPassword: new FormControl('', Validators.required),
      newPassword: new FormControl('', Validators.required),
      newPasswordConfirm: new FormControl('', Validators.required),
    });

    this.form.get('currentPassword')?.valueChanges
      .subscribe((val: string) => { this.currentPassword = val; });

    this.form.get('newPassword')?.valueChanges
      .subscribe((val: string) => { this.newPassword = val; });

    this.form.get('newPasswordConfirm')?.valueChanges
      .subscribe((val: string) => { this.newPasswordConfirm = val; });

    this.spinnerService.visibility.subscribe((value) => {
      this.disableSubmit = value;
    });
  }

  changePassword() {

    if (this.newPassword !== this.newPasswordConfirm) {
      this.notificationService.openSnackBar('New passwords do not match.');
      return;
    }

    const email = this.authService.getCurrentUser().email;

    this.authService.changePassword(email, this.currentPassword, this.newPassword)
      .subscribe(
        data => {
          this.form.reset();
          this.notificationService.openSnackBar('Your password has been changed.');
        },
        error => {
          this.notificationService.openSnackBar(error.error);
        }
      );
  }
}
