import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { CurrentUser } from '../models/currentUser';
import { Router } from '@angular/router';
import { Dates } from '../models/dates';
import { ProductsSoldParent } from '../models/productsSoldParent';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  private readonly apiUrl = environment.apiUrl + '/Analytics';

  constructor(private http: HttpClient) { }

  getProductsSold(start: Date, end: Date) {
    var getProductsSoldUrl = this.apiUrl + '/ProductsSold';
    var getProductsSoldModel = new Dates(start, end);
    return this.http.post<ProductsSoldParent>(getProductsSoldUrl, getProductsSoldModel);
  }

  getProductsSoldAdmin(start: Date, end: Date, userId: number) {
    var getProductsSoldUrl = this.apiUrl + '/ProductsSoldAdmin';
    var getProductsSoldModel = new Dates(start, end, userId);
    return this.http.post<ProductsSoldParent>(getProductsSoldUrl, getProductsSoldModel);
  }
}
