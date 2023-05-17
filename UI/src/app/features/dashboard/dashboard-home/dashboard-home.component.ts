import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { AuthenticationService } from 'src/app/core/services/auth.service';
import { UserService } from 'src/app/core/services/user.service';
import { CurrentUser } from 'src/app/core/models/currentUser';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DashboardService } from 'src/app/core/services/dashboard.service';
import { ProductsSoldParent } from 'src/app/core/models/productsSoldParent';
import { ProductsSoldChild } from 'src/app/core/models/productsSoldChild';

export interface OrderDetails {
  orderId: number;
  date: string;
  price: number;
  profit: number;
}

const DATA: OrderDetails[] = [
  {orderId: 1, date: 'Hydrogen', price: 2, profit: 1},
  {orderId: 2, date: 'Helium', price: 2, profit: 1},
  {orderId: 3, date: 'Lithium', price: 2, profit: 1},
  {orderId: 4, date: 'Beryllium', price: 2, profit: 1},
  {orderId: 5, date: 'Boron', price: 2, profit: 1},
  {orderId: 6, date: 'Carbon', price: 2, profit: 1},
  {orderId: 7, date: 'Nitrogen', price: 2, profit: 1},
  {orderId: 8, date: 'Oxygen', price: 2, profit: 1},
  {orderId: 9, date: 'Fluorine', price: 2, profit: 1},
  {orderId: 10, date: 'Neon', price: 2, profit: 1},
];
@Component({
  selector: 'app-dashboard-home',
  templateUrl: './dashboard-home.component.html',
  styleUrls: ['./dashboard-home.component.css']
})

export class DashboardHomeComponent implements OnInit {

  currentUser: CurrentUser | null = null;
  rangeForm!: FormGroup;
  productsSold: ProductsSoldParent = new ProductsSoldParent(0, [new ProductsSoldChild('Test', 55, 1),new ProductsSoldChild('Test', 55, 1),new ProductsSoldChild('Test', 55, 1),new ProductsSoldChild('Test', 55, 1),new ProductsSoldChild('Test', 55, 1),new ProductsSoldChild('Test', 55, 1),new ProductsSoldChild('Test', 55, 1),new ProductsSoldChild('Test', 55, 1),new ProductsSoldChild('Test', 55, 1),new ProductsSoldChild('Test', 55, 1),new ProductsSoldChild('Test', 55, 1),new ProductsSoldChild('Test', 55, 1),new ProductsSoldChild('Test', 55, 1),new ProductsSoldChild('Test', 55, 1),new ProductsSoldChild('Test', 55, 1),new ProductsSoldChild('Test', 55, 1),new ProductsSoldChild('Test', 55, 1)]);
  displayedColumns: string[] = ['orderId', 'date', 'price', 'profit'];
  dataSource = DATA;
  constructor(private userService: UserService,
    private authService: AuthenticationService,
    private dashboardService: DashboardService,
    private titleService: Title) {
  }

  ngOnInit() {
    this.currentUser = this.authService.getCurrentUser();
    this.titleService.setTitle('Brickalytics - Dashboard');
    this.createForm();
    this.getProductsSold();
  }

  getUserById(id: number) {
    this.userService.getUserById(id).subscribe();
  }

  getProductsSold(){
    var start = this.rangeForm.get('start')?.value;
    var end = this.rangeForm.get('end')?.value;
    if(!(start instanceof Date)){
      start = start._d;
    }
    if(!(end instanceof Date)){
      end = end._d;
    }
    this.dashboardService.getProductsSold(start, end).subscribe(res => {
      this.productsSold = res;
    })
  }

  private createForm() {
    // default to the last week
    var lastMonday = this.getMonday(this.getLastWeek());
    var sunday = this.getMonday(new Date());
    sunday.setDate(sunday.getDate() - 1);

    this.rangeForm = new FormGroup({
      start: new FormControl(new Date(lastMonday.setHours(0,0,0,0)), Validators.required),
      end: new FormControl(new Date(sunday.setUTCHours(23,59,59,999)), Validators.required)
    });

}
  private getMonday(date: Date) {
    date = new Date(date);
    var day = date.getDay(),
        diff = date.getDate() - day + (day == 0 ? -6:1); // adjust when day is sunday
    return new Date(date.setDate(diff));
  }
  private getLastWeek() {
    var today = new Date();
    var lastWeek = new Date(today.getFullYear(), today.getMonth(), today.getDate() - 7);
    return lastWeek;
  }
}
