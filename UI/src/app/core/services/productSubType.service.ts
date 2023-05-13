import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ProductSubType } from '../models/productSubType';

@Injectable({
  providedIn: 'root'
})
export class ProductSubTypeService {
  private readonly apiUrl = environment.apiUrl + '/ProductSubType';

  constructor(private readonly http: HttpClient) {}

  getProductSubTypes(): Observable<ProductSubType[]> {
    return this.http.get<ProductSubType[]>(this.apiUrl);
  }
}
