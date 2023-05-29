import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';

import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { CurrentUser } from '../models/currentUser';
import { Token } from '../models/token';
import { LoginInfo } from '../models/loginInfo';
import { Router } from '@angular/router';
import { Result } from '../models/result';
import { NotificationService } from './notification.service';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  private currentUser: any;
  private readonly apiUrl = environment.apiUrl + '/Authentication';

  constructor(private router: Router,
    private http: HttpClient,
    private notificationService: NotificationService,
    @Inject('LOCALSTORAGE') private localStorage: Storage) {
    var currentUserJson = this.localStorage.getItem('currentUser');
    if (currentUserJson !== null) {
      this.currentUser = JSON.parse(this.localStorage.getItem('currentUser')!);
    }
  }

  login(creatorName: string, password: string) {
    var loginUrl = this.apiUrl + '/Login';
    var loginModel = new LoginInfo(creatorName, password);

    return this.http.post<Result<CurrentUser>>(loginUrl, loginModel);
  }

  logout(): void {
    var user = this.currentUser;

    var logoutUrl = this.apiUrl + '/Logout';
    var logoutModel = new Token(user.accessToken, user.refreshToken, user.refreshTokenExpiration);

    this.http.put<Result<Token>>(logoutUrl, logoutModel).subscribe(res => {
      if (res.code == 200) {
        this.currentUser = null;
        // Remove the user from local storage to clear the authentication state.
        localStorage.removeItem('currentUser');
        this.router.navigate(['/auth/login']);
      } else {
        this.notificationService.openSnackBar(res.message);
      }
    });
  }

  getCurrentUser(): CurrentUser {
    return this.currentUser;
  }

  setCurrentUser(user: CurrentUser): void {
    this.currentUser = user;
    this.localStorage.setItem('currentUser', JSON.stringify(user));
  }

  refreshToken(): Observable<Result<Token>> {
    var user = this.currentUser;

    var refreshUrl = this.apiUrl + '/Refresh';
    var refreshModel = new Token(user.accessToken, user.refreshToken, user.refreshTokenExpiration);

    return this.http.post<Result<Token>>(refreshUrl, refreshModel).pipe(
      map(token => {
        if (token.code == 200) {
          user.accessToken = token.data!.accessToken;
          user.refreshToken = token.data!.refreshToken;
          user.refreshTokenExpiration = token.data!.refreshTokenExpiration;

          this.setCurrentUser(user);
        } else {
          this.notificationService.openSnackBar(token.message);
          console.error("refreshToken(): ", token.message)
        }
        return token;
      })
    );
  }
}
