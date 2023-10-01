import { Component, HostListener, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { AuthenticationService } from 'src/app/core/services/auth.service';
import { UserService } from 'src/app/core/services/user.service';
import { CurrentUser } from 'src/app/core/models/currentUser';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DashboardService } from 'src/app/core/services/dashboard.service';
import { ProductsSoldParent } from 'src/app/core/models/productsSoldParent';
import { GenericType } from 'src/app/core/models/genericType';
import { Payment } from 'src/app/core/models/payment';
import { NotificationService } from 'src/app/core/services/notification.service';
import { User } from 'src/app/core/models/user';
import { UserModal } from '../../users/userModal/user-modal.component';
import { MatDialog } from '@angular/material/dialog';

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

  userList: User[] = [];
  owed: Payment | undefined;
  roleList: GenericType[] = [];
  collectionList: GenericType[] = [];
  isColumnsMobile: boolean = false;
  isLoadingResults: boolean = false;
  payments: Payment[] = [];
  productsSold: ProductsSoldParent = new ProductsSoldParent(0, 0, []);
  isAdmin = false;
  selectedTab: string = 'Analytics'
  breakpoint = 0;
  displayedColumns: string[] = [];

  constructor(private userService: UserService,
    private authService: AuthenticationService,
    private dashboardService: DashboardService,
    private notificationService: NotificationService,
    private titleService: Title,
    private dialog: MatDialog) {
    this.rangeForm = new FormGroup({
      start: new FormControl(new Date(), Validators.required),
      end: new FormControl(new Date(), Validators.required)
    });
    this.creatorForm = new FormGroup({
      id: new FormControl(this.creators![0]?.id ? this.creators![0].id : 0, Validators.required)
    });
  }

  ngOnInit() {
    this.currentUser = this.authService.getCurrentUser();
    this.isAdmin = this.currentUser.isAdmin;
    this.titleService.setTitle('Brickalytics - Dashboard');

    this.onResizeInit();
    this.getUsersList();
    this.getRolesList();
    this.getCreators();
    this.getCollectionsList();
    this.toggleTab('Analytics');
  }

  getUserById(id: number) {
    this.userService.getUserById(id).subscribe(res => {
      if(res.code != 200){
        this.notificationService.openSnackBar(res.message)
      }
    }
    );
  }

  getCreators() {
    if (this.currentUser?.isAdmin) {
      this.userService.getCreatorNames().subscribe(res => {
        if (res.code == 200) {
          this.creators = res.data!;
          this.createForms();
        } else {
          this.notificationService.openSnackBar(res.message);
        }
      })
    } else {
      this.getPayments();
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
      this.dashboardService.getProductsSoldAdmin(start, end, id).subscribe(res => {
        if (res.code == 200) {
          for (var i = 0; i < res.data!.items.length; i++) {
            res.data!.items[i].total = (Math.round(parseFloat(res.data!.items[i].total) * 100) / 100).toFixed(2).toString()
          }
          this.productsSold = res.data!;
        } else {
          this.notificationService.openSnackBar(res.message);
        }
      });

    } else {
      this.dashboardService.getProductsSold(start, end).subscribe(res => {
        if (res.code == 200) {
          this.productsSold = res.data!;
        } else {
          this.notificationService.openSnackBar(res.message);
        }
      });
    }
  }
  getPayments() {
    var id = this.currentUser?.isAdmin ? this.creatorForm.get('id')?.value : 0;
    this.dashboardService.getPayments(id).subscribe(res => {
      if (res.code == 200) {
        this.owed = res.data!.shift();
        this.payments = res.data!;
      } else {
        this.notificationService.openSnackBar(res.message);
      }
    }, ()=>{},()=>{
      this.onResizeInit();
    });
  }

  logout(): void {
    this.authService.logout();
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
      if (res.code == 200) {
        this.productsSold = res.data!;
      } else {
        this.notificationService.openSnackBar(res.message);
      }
    })
  }
  addPayment(){
    var id = this.creatorForm.get('id')?.value;
    var amount: number = this.owed!.paymentAmount
    this.dashboardService.addPayment(id, amount).subscribe(()=>{
      this.getPayments();
    })
  }
  toggleTab(tabName: string){
    this.selectedTab = tabName;
    switch(tabName){
      case 'Analytics':
        break;
      case 'Payments':
        break;
      case 'Users':
        break;
      case 'Settings':
        break;
      default:
        break;
    }
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
      id: new FormControl(this.creators![0]?.id ? this.creators![0].id : 0, Validators.required)
    });

    this.getProductsSold();
    this.getPayments();
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


  getUsersList() {
    this.userService.getUsers().subscribe(res => {
      if(res.code == 200){
      this.userList = res.data!;
      } else {
      this.notificationService.openSnackBar(res.message);
      }
    });
  }

  addUser(user: User) {
    this.userService.addUser(user).subscribe(res => {
      if (res.code == 200) {
        user.id = res.data;
        this.userList!.push(user);
      } else {
        this.notificationService.openSnackBar(res.message);
      }
    });
  }
  updateUser(user: User) {
    this.userService.updateUser(user).subscribe(res => {
      if (res.code == 200) {
        this.getUsersList();
      } else {
        this.notificationService.openSnackBar(res.message)
      }
    });
  }
  updateUserPassword(user: User, password: string) {
    this.userService.updateUserPassword(user.id!, password).subscribe(res => {
      if (res.code != 200) {
        this.notificationService.openSnackBar(res.message);
      }
    });
  }
  getRole(id: number | null) {
    for (var role of this.roleList!) {
      if (role.id == id) {
        return role.name;
      }
    }
    return "None";
  }
  getCollection(id: number) {
    if (id) {
      for (var collection of this.collectionList!) {
        if (collection.id == id) {
          return collection.name;
        }
      }
    }
    return "None";
  }
  openUserModal(user: User | null = null) {

    if (!user) {
      user = new User(null, true, 2, 0, '', null);
    }

    const dialogRef = this.dialog.open(UserModal, {
      data: { user: user, roleList: this.roleList, collectionList: this.collectionList }
    });

    dialogRef.afterClosed().subscribe(_user => {
      this.getUsersList();
    });
  }
  getRolesList() {
    this.userService.getRoles().subscribe(res => {
      if(res.code == 200){
      this.roleList = res.data!;
      } else {

      console.error(res.code, res.message);
      this.notificationService.openSnackBar(res.message);
      }
    });
  }
  getCollectionsList() {
    this.userService.getCollections().subscribe(res => {
      if(res.code == 200){
      this.collectionList = res.data!;
      } else {
      this.notificationService.openSnackBar(res.message);
      }
    });
  }

  // @HostListener('window:resize', ['$event'])
  onResize(event: any) {

    if (event.target.innerWidth < 992 && !this.isColumnsMobile) {

      this.displayedColumns = ['CreatorName', 'Actions'];
      this.isColumnsMobile = true;

    } else if (event.target.innerWidth >= 992 && this.isColumnsMobile) {

      this.displayedColumns = ['CreatorName', 'Active', 'Role', 'Collection', 'Actions'];
      this.isColumnsMobile = false;
    }
    if (event.target.innerWidth >= 1400) { this.breakpoint = 5 };
    if (event.target.innerWidth < 1400) { this.breakpoint = 4 };
    if (event.target.innerWidth < 1200) { this.breakpoint = 3 };
    if (event.target.innerWidth < 992) { this.breakpoint = 2 };
    if (event.target.innerWidth < 550) { this.breakpoint = 1 };

  }

  onResizeInit() {
    if (window.innerWidth >= 1400) { this.breakpoint = 5 };
    if (window.innerWidth < 1400) { this.breakpoint = 4 }
    if (window.innerWidth < 1200) { this.breakpoint = 3 }
    if (window.innerWidth < 992) { this.breakpoint = 2 }
    if (window.innerWidth < 550) { this.breakpoint = 1 }
  }

}
