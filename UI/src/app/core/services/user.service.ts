import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { User } from '../models/user';
import { Observable } from 'rxjs';
import { UserRate } from '../models/userRate';
import { GenericType } from '../models/genericType';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly apiUrl = environment.apiUrl + '/User';
  constructor(private http: HttpClient) { }

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.apiUrl);
  }
  getRoles(): Observable<GenericType[]> {
    const url = `${this.apiUrl}/Roles`;
    return this.http.get<GenericType[]>(url);
  }
  getCollections(): Observable<GenericType[]> {
    const url = `${this.apiUrl}/Collections`;
    return this.http.get<GenericType[]>(url);
  }

  getUserById(id: number): Observable<User> {
    const url = `${this.apiUrl}/${id}`;
    return this.http.get<User>(url);
  }

  addUser(user: User): Observable<number> {
    return this.http.post<number>(this.apiUrl, user);
  }

  updateUser(user: User): Observable<void> {
    return this.http.put<void>(this.apiUrl, user);
  }

  updateUserPassword(id: number, password: string): Observable<void> {
    const url = `${this.apiUrl}/Password`;
    const body = { id: id, password: password };
    return this.http.put<void>(url, body);
  }

  addUserRate(userRate: UserRate): Observable<void> {
    const url = `${this.apiUrl}/Rate`;
    return this.http.put<void>(url, userRate);
  }

  deleteUserRate(userRate: UserRate): Observable<void> {
    const url = `${this.apiUrl}/Rate`;
    return this.http.delete<void>(url, { body: userRate });
  }
}
