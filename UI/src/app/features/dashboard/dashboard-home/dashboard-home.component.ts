import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { AuthenticationService } from 'src/app/core/services/auth.service';
import { UserService } from 'src/app/core/services/user.service';
import { CurrentUser } from 'src/app/core/models/currentUser';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DashboardService } from 'src/app/core/services/dashboard.service';
import { ProductsSoldParent } from 'src/app/core/models/productsSoldParent';

export interface Tile {
  color: string;
  cols: number;
  rows: number;
  text: string;
}
@Component({
  selector: 'app-dashboard-home',
  templateUrl: './dashboard-home.component.html',
  styleUrls: ['./dashboard-home.component.css']
})

export class DashboardHomeComponent implements OnInit {

  currentUser: CurrentUser | null = null;
  rangeForm!: FormGroup;
  productsSold!: ProductsSoldParent;

  tiles: Tile[] = [
    { text: 'Traffic', cols: 1, rows: 1, color: 'lightblue' },
    { text: 'Products Sold', cols: 1, rows: 1, color: 'lightblue' },
    { text: 'Date Picker', cols: 1, rows: 1, color: 'lightgreen' },
    { text: 'Details [prin', cols: 3, rows: 1, color: '#DDBDF1' },
  ];

  constructor(private userService: UserService,
    private authService: AuthenticationService,
    private dashboardService: DashboardService,
    private titleService: Title) {
  }

  ngOnInit() {
    this.currentUser = this.authService.getCurrentUser();
    this.titleService.setTitle('Brickalytics - Dashboard');
    this.createForm();
    // this.getProductsSold();
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
    var monday = this.getMonday(new Date());

    this.rangeForm = new FormGroup({
      start: new FormControl(new Date(lastMonday.setHours(0,0,0,0)), Validators.required),
      end: new FormControl(new Date(monday.setUTCHours(23,59,59,999)), Validators.required)
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
