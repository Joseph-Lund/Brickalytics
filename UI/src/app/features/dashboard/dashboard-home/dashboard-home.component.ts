import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { AuthenticationService } from 'src/app/core/services/auth.service';
import { UserService } from 'src/app/core/services/user.service';
import { CurrentUser } from 'src/app/core/models/currentUser';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DashboardService } from 'src/app/core/services/dashboard.service';
import { ProductsSoldParent } from 'src/app/core/models/productsSoldParent';
import { ProductsSoldChild } from 'src/app/core/models/productsSoldChild';
import { GenericType } from 'src/app/core/models/genericType';
import { User } from 'src/app/core/models/user';

@Component({
  selector: 'app-dashboard-home',
  templateUrl: './dashboard-home.component.html',
  styleUrls: ['./dashboard-home.component.css']
})

export class DashboardHomeComponent implements OnInit {

  currentUser: CurrentUser | null = null;
  rangeForm!: FormGroup;
  creatorForm!: FormGroup;
  creators: GenericType[] = [];
  productsSold: ProductsSoldParent = new ProductsSoldParent(0, 0, []);
  isAdmin = false;

  constructor(private userService: UserService,
    private authService: AuthenticationService,
    private dashboardService: DashboardService,
    private titleService: Title) {
      this.rangeForm = new FormGroup({
        start: new FormControl(new Date(), Validators.required),
        end: new FormControl(new Date(), Validators.required)
      });
      this.creatorForm = new FormGroup({
        id: new FormControl(this.creators[0]?.id ? this.creators[0].id : 0, Validators.required)
      });
  }

  ngOnInit() {
    this.currentUser = this.authService.getCurrentUser();
    this.isAdmin = this.currentUser.isAdmin;
    this.titleService.setTitle('Brickalytics - Dashboard');
    console.log("this.currentUser", this.currentUser);
    console.log("this.isAdmin", this.isAdmin);
    this.getCreators();
  }

  getUserById(id: number) {
    this.userService.getUserById(id).subscribe();
  }

  getCreators() {
    console.log("getCreators in");
    if (this.currentUser?.isAdmin) {
      this.userService.getCreatorNames().subscribe(res => {

    console.log("getCreatorNames admin", res);
        this.creators = res;
        this.createForms();
      })
    } else {
      console.log("getCreatorNames user");

      this.createForms();
    }
  }

  getProductsSold() {
    var start = this.rangeForm.get('start')?.value;
    var end = this.rangeForm.get('end')?.value;
    if (!(start instanceof Date)) {
      start = start._d;
    }
    if (!(end instanceof Date)) {
      end = end._d;
    }
    if (this.currentUser?.isAdmin) {
      var id = this.creatorForm.get('id')?.value;

      console.log("this.creatorForm.get('id')?.value", id);
      this.dashboardService.getProductsSoldAdmin(start, end, id).subscribe(res => {
        this.productsSold = res;
      });

    } else {
      this.dashboardService.getProductsSold(start, end).subscribe(res => {
        this.productsSold = res;
      });
    }
  }

  getProductsSoldAdmin() {
    var start = this.rangeForm.get('start')?.value;
    var end = this.rangeForm.get('end')?.value;

    var id = this.creatorForm.get('id')?.value;

    if (!(start instanceof Date)) {
      start = start._d;
    }
    if (!(end instanceof Date)) {
      end = end._d;
    }

    this.dashboardService.getProductsSoldAdmin(start, end, id).subscribe(res => {
      this.productsSold = res;
    })
  }

  private createForms() {
    // default to the last week
    var lastMonday = this.getMonday(this.getLastWeek());
    var sunday = this.getMonday(new Date());
    sunday.setDate(sunday.getDate() - 1);

    this.rangeForm = new FormGroup({
      start: new FormControl(new Date(lastMonday.setHours(0, 0, 0, 0)), Validators.required),
      end: new FormControl(new Date(sunday.setUTCHours(23, 59, 59, 999)), Validators.required)
    });
    this.creatorForm = new FormGroup({
      id: new FormControl(this.creators[0]?.id ? this.creators[0].id : 0, Validators.required)
    });

    this.getProductsSold();
  }

  private getMonday(date: Date) {
    date = new Date(date);
    var day = date.getDay(),
      diff = date.getDate() - day + (day == 0 ? -6 : 1); // adjust when day is sunday
    return new Date(date.setDate(diff));
  }
  private getLastWeek() {
    var today = new Date();
    var lastWeek = new Date(today.getFullYear(), today.getMonth(), today.getDate() - 7);
    return lastWeek;
  }
}
