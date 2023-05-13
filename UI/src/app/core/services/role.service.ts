import { Component, OnInit } from '@angular/core';
import { GenericType } from '../models/genericType';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-role',
  templateUrl: './role.component.html',
  styleUrls: ['./role.component.css']
})
export class RoleService {

  private readonly apiUrl = environment.apiUrl + '/Role';
  constructor(private http: HttpClient) { }

  getRoles(): Observable<GenericType[]> {
    return this.http.get<GenericType[]>(this.apiUrl);
  }
}
