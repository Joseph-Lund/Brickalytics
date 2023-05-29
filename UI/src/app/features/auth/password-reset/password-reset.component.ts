import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { AuthenticationService } from 'src/app/core/services/auth.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { UserService } from 'src/app/core/services/user.service';

@Component({
  selector: 'app-password-reset',
  templateUrl: './password-reset.component.html',
  styleUrls: ['./password-reset.component.css']
})
export class PasswordResetComponent implements OnInit {

  private token!: string;
  email!: string;
  form!: FormGroup;
  loading!: boolean;
  hideNewPassword: boolean;
  hideNewPasswordConfirm: boolean;

  constructor(private activeRoute: ActivatedRoute,
    private router: Router,
    private authenticationService: AuthenticationService,
    private userService: UserService,
    private notificationService: NotificationService,
    private titleService: Title) {

    this.titleService.setTitle('Brickalytics - Password Reset');
    this.hideNewPassword = true;
    this.hideNewPasswordConfirm = true;
  }

  ngOnInit() {
    this.activeRoute.queryParamMap.subscribe((params: ParamMap) => {
      this.token = params.get('token') + '';
      this.email = params.get('email') + '';

      if (!this.token || !this.email) {
        this.router.navigate(['/']);
      }
    });

    this.form = new FormGroup({
      newPassword: new FormControl('', Validators.required),
      newPasswordConfirm: new FormControl('', Validators.required)
    });
  }

  resetPassword() {

    const password = this.form.get('newPassword')?.value;
    const passwordConfirm = this.form.get('newPasswordConfirm')?.value;

    if (password !== passwordConfirm) {
      this.notificationService.openSnackBar('Passwords do not match');
      return;
    }

    this.loading = true;

    this.userService.updateUserPassword(this.authenticationService.getCurrentUser().id, password)
      .subscribe(
        res => {
          if (res.code == 200) {
            this.notificationService.openSnackBar('Your password has been changed.');
            this.router.navigate(['/auth/login']);
          } else {
            this.loading = false;
            this.notificationService.openSnackBar(res.message);
          }
        }, err=>{
            this.loading = false;
            console.error(err);
        });
  }

  cancel() {
    this.router.navigate(['/']);
  }
}
