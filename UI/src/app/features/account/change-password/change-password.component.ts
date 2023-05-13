import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Component, OnInit } from '@angular/core';
import { NotificationService } from 'src/app/core/services/notification.service';
import { SpinnerService } from 'src/app/core/services/spinner.service';
import { User } from 'src/app/core/models/user';
import { UserService } from 'src/app/core/services/user.service';


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

  constructor(private userService: UserService,
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


  updatePassword(user: User, password: string){
    this.userService.updateUserPassword(user.id, password).subscribe(u =>{ });
  }
}
