import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { delay, map } from 'rxjs/operators';
import * as moment from 'moment';

import { environment } from '../../../environments/environment';
import { Observable, of } from 'rxjs';
import { CurrentUser } from '../models/currentUser';
import { Token } from '../models/token';
import { LoginInfo } from '../models/loginInfo';
import { LoginResponse } from '../models/loginResponse';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  private currentUser: any;
  private readonly apiUrl = environment.apiUrl + '/Authentication';

  constructor(private http: HttpClient,
    @Inject('LOCALSTORAGE') private localStorage: Storage) {
    var currentUserJson = this.localStorage.getItem('currentUser');
    if (currentUserJson !== null) {
      this.currentUser = JSON.parse(this.localStorage.getItem('currentUser')!);
    }
  }

  login(creatorName: string, password: string) {
    var loginUrl = this.apiUrl + '/Login';
    var loginModel = new LoginInfo(creatorName, password);

    this.http.post<LoginResponse>(loginUrl, loginModel).subscribe(response => {
      this.setCurrentUser(new CurrentUser(response.id, response.creatorName, response.email, response.accessToken, response.refreshToken, response.refreshTokenExpiration))
    });
  }

  logout(): void {
    var user = this.currentUser;

    var logoutUrl = this.apiUrl + '/Logout';
    var logoutModel = new Token(user.accessToken, user.refreshToken, user.refreshTokenExpiration);

    this.http.put<Token>(logoutUrl, logoutModel).subscribe(token => {
      this.currentUser = null;
      // Remove the user from local storage to clear the authentication state.
      localStorage.removeItem('currentUser');
    });
  }

  getCurrentUser(): CurrentUser {
    return this.currentUser;
  }

  setCurrentUser(user: CurrentUser): void {
    this.currentUser = user;
    this.localStorage.setItem('currentUser', JSON.stringify(user));
  }

  refreshToken(): Observable<Token> {
    var user = this.currentUser;

    var refreshUrl = this.apiUrl + 'Refresh';
    var refreshModel = new Token(user.accessToken, user.refreshToken, user.refreshTokenExpiration);

    return this.http.post<Token>(refreshUrl, refreshModel).pipe(
      map(token => {
        user.accessToken = token.accessToken;
        user.refreshToken = token.refreshToken;
        user.refreshTokenExpiration = token.refreshTokenExpiration;

        this.setCurrentUser(user);

        return token;
      })
    );
  }
}
