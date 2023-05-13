import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { AuthenticationService } from 'src/app/core/services/auth.service';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

    loginForm!: FormGroup;
    loading!: boolean;

    constructor(private titleService: Title,
        private authenticationService: AuthenticationService) {
    }

    ngOnInit() {
        this.titleService.setTitle('Brickalytics - Login');
        this.createForm();
    }

    private createForm() {

        this.loginForm = new FormGroup({
            username: new FormControl('', [Validators.required]),
            password: new FormControl('', Validators.required)
        });
    }

    login() {
        const username = this.loginForm.get('username')?.value;
        const password = this.loginForm.get('password')?.value;

        this.loading = true;
        this.authenticationService.login(username.toLowerCase(), password);
    }

    // resetPassword() {
    //     this.router.navigate(['/auth/password-reset-request']);
    // }
}
