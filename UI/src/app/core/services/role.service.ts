import { Injectable } from '@angular/core';
import { GenericType } from '../models/genericType';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Result } from '../models/result';


@Injectable({
  providedIn: 'root'
})
export class RoleService {

  private readonly apiUrl = environment.apiUrl + '/Role';
  constructor(private http: HttpClient) { }

  getRoles(): Observable<Result<GenericType[]>> {
    return this.http.get<Result<GenericType[]>>(this.apiUrl);
  }
}
