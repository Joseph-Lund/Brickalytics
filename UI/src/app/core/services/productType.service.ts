import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { GenericType } from '../models/genericType';
import { environment } from 'src/environments/environment';
import { Result } from '../models/result';

@Injectable({
  providedIn: 'root'
})
export class ProductTypeService {

  private readonly apiUrl = environment.apiUrl + '/ProductType';
  constructor(private http: HttpClient) { }

  getProductTypes(): Observable<Result<GenericType[]>> {
    return this.http.get<Result<GenericType[]>>(this.apiUrl);
  }
}
