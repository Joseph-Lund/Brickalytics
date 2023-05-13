import { Router } from '@angular/router';
import { BehaviorSubject, EMPTY, Observable, throwError } from 'rxjs';
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpErrorResponse } from '@angular/common/http';
import { HttpRequest } from '@angular/common/http';
import { HttpHandler } from '@angular/common/http';
import { HttpEvent } from '@angular/common/http';
import { catchError, filter, finalize, switchMap, take } from 'rxjs/operators';
import { AuthenticationService } from '../services/auth.service';
import { MatDialog } from '@angular/material/dialog';
import { Token } from '../models/token';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);


    constructor(private authService: AuthenticationService,
        private router: Router,
        private dialog: MatDialog) { }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

        const user = this.authService.getCurrentUser();

        if (user && user.accessToken) {
          req = this.addToken(req, user.accessToken);
        }
        return next.handle(req).pipe(
          catchError((error: { status: number; }) => {
            if (error instanceof HttpErrorResponse && error.status === 401) {
              return this.handle401Error(req, next);
            } else {
              return throwError(error);
            }
          })
        );
    }

    private addToken(request: HttpRequest<any>, token: string) {
      return request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`,
        },
      });
    }
    private handle401Error(
      request: HttpRequest<any>,
      next: HttpHandler
    ): Observable<HttpEvent<any>> {
      if (!this.isRefreshing) {
        this.isRefreshing = true;
        this.refreshTokenSubject.next(null);
        this.authService
        return this.authService.refreshToken().pipe(
          switchMap((response: Token) => {
            if (response && response.accessToken) {
              this.refreshTokenSubject.next(response.accessToken);
              return next.handle(this.addToken(request, response.accessToken));
            }
            return this.redirectToLogin();
          }),
          catchError((error: any) => {
            return this.redirectToLogin();
          }),
          finalize(() => {
            this.isRefreshing = false;
          })
        );
      } else {
        return this.refreshTokenSubject.pipe(
          filter((token) => token != null),
          take(1),
          switchMap((token: string) => {
            return next.handle(this.addToken(request, token));
          })
        );
      }
    }
    private redirectToLogin(): Observable<any> {
      this.dialog.closeAll();
      this.router.navigate(['/auth/login']);
      return EMPTY;
    }
}
