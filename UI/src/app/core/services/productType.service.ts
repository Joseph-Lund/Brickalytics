import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { GenericType } from '../models/genericType';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProductTypeService {

  private readonly apiUrl = environment.apiUrl + '/ProductType';
  constructor(private http: HttpClient) { }

  getProductTypes(): Observable<GenericType[]> {
    return this.http.get<GenericType[]>(this.apiUrl);
  }
}
