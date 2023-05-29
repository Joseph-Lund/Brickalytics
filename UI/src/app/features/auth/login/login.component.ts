import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { AuthenticationService } from 'src/app/core/services/auth.service';
import { NotificationService } from 'src/app/core/services/notification.service';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

    loginForm!: FormGroup;
    loading!: boolean;

    constructor(private router: Router,
      private titleService: Title,
        private authenticationService: AuthenticationService,
        private notificationService: NotificationService) {
    }

    ngOnInit() {
        this.titleService.setTitle('Brickalytics - Login');
        this.createForm();
    }

    private createForm() {

        this.loginForm = new FormGroup({
            username: new FormControl('', Validators.required),
            password: new FormControl('', Validators.required)
        });
    }

    login() {
        const username = this.loginForm.get('username')?.value;
        const password = this.loginForm.get('password')?.value;

        this.loading = true;
        this.authenticationService.login(username.toLowerCase(), password)
        .subscribe(response => {
          this.authenticationService.setCurrentUser(response);
          this.router.navigate(['/dashboard']);
        },
        err => {
          this.loading = false;
          if(err.error.includes("System.UnauthorizedAccessException")){
            this.notificationService.openSnackBar("Incorrect Username or Password");
          } else {
            this.notificationService.openSnackBar("An Uknown Error Has Occured");
          }
        });
    }
}
