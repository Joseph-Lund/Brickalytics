import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { User } from '../models/user';
import { Observable } from 'rxjs';
import { UserRate } from '../models/userRate';
import { GenericType } from '../models/genericType';
import { Result } from '../models/result';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly apiUrl = environment.apiUrl + '/User';
  constructor(private http: HttpClient) { }

  getUsers(): Observable<Result<User[]>> {
    return this.http.get<Result<User[]>>(this.apiUrl);
  }
  getCreatorNames(): Observable<Result<GenericType[]>> {
    const url = `${this.apiUrl}/Names`;
    return this.http.get<Result<GenericType[]>>(url);
  }
  getRoles(): Observable<Result<GenericType[]>> {
    const url = `${this.apiUrl}/Roles`;
    return this.http.get<Result<GenericType[]>>(url);
  }
  getCollections(): Observable<Result<GenericType[]>> {
    const url = `${this.apiUrl}/Collections`;
    return this.http.get<Result<GenericType[]>>(url);
  }

  getUserById(id: number): Observable<Result<User>> {
    const url = `${this.apiUrl}/${id}`;
    return this.http.get<Result<User>>(url);
  }

  addUser(user: User): Observable<Result<number>> {
    return this.http.post<Result<number>>(this.apiUrl, user);
  }

  updateUser(user: User): Observable<Result> {
    return this.http.put<Result>(this.apiUrl, user);
  }

  updateUserPassword(id: number, password: string): Observable<Result> {
    const url = `${this.apiUrl}/Password`;
    const body = { id: id, password: password };
    return this.http.put<Result>(url, body);
  }

  addUserRate(userRate: UserRate): Observable<Result> {
    const url = `${this.apiUrl}/Rate`;
    return this.http.put<Result>(url, userRate);
  }

  deleteUserRate(userRate: UserRate): Observable<Result> {
    const url = `${this.apiUrl}/Rate`;
    return this.http.delete<Result>(url, { body: userRate });
  }
}
