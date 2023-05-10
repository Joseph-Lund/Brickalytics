import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class HttpService {

  constructor(private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: Storage) {
  }

  get(endpoint: string) {
    this.http.get(this.baseUrl + endpoint).subscribe(result => {
      return result;
    }, error => console.error(error));
  }
  post(endpoint: string, item: any) {
    this.http.post(this.baseUrl + endpoint, item).subscribe(result => {
      return result;
    }, error => console.error(error));
  }
  put(endpoint: string, item: any) {
    this.http.put(this.baseUrl + endpoint, item).subscribe(result => {
      return result;
    }, error => console.error(error));
  }
  delete(endpoint: string) {
    this.http.delete(this.baseUrl + endpoint).subscribe(result => {
      return result;
    }, error => console.error(error));
  }
}
