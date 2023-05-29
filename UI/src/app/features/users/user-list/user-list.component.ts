import { Component, HostListener, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { Title } from '@angular/platform-browser';
import { GenericType } from 'src/app/core/models/genericType';
import { User } from 'src/app/core/models/user';
import { NotificationService } from 'src/app/core/services/notification.service';
import { ProductTypeService } from 'src/app/core/services/productType.service';
import { UserService } from 'src/app/core/services/user.service';
import { UserModal } from 'src/app/features/users/userModal/user-modal.component';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css']
})
export class UserListComponent implements OnInit {

  userList: User[] = [];
  roleList: GenericType[] = [];
  collectionList: GenericType[] = [];
  productTypeList: GenericType[] = [];
  length = 50;
  pageSize = 10;
  pageIndex = 0;
  pageSizeOptions = [5, 10, 25];
  isLoadingResults: boolean = false;
  showFirstLastButtons: boolean = true;
  pageEvent: PageEvent | null = null;
  displayedColumns: string[] = ['CreatorName', 'Active', 'Role', 'Collection', 'Actions'];
  isColumnsMobile: boolean = false;


  constructor(
    private userService: UserService,
    private productTypeService: ProductTypeService,
    private titleService: Title,
    private notificationService: NotificationService,
    private dialog: MatDialog
  ) { }

  ngOnInit() {
    this.titleService.setTitle('Brickalytics - Users');
    if (window.innerWidth < 992 && !this.isColumnsMobile) {

      this.displayedColumns = ['CreatorName', 'Actions'];
      this.isColumnsMobile = true;

    } else if (window.innerWidth >= 992 && this.isColumnsMobile) {

      this.displayedColumns = ['CreatorName', 'Active', 'Role', 'Collection', 'Actions'];
      this.isColumnsMobile = false;
    }
    this.getUsersList();
    this.getRolesList();
    this.getCollectionsList();
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
  getProductTypeList() {
    this.productTypeService.getProductTypes().subscribe(res => {
      if(res.code == 200){
      this.productTypeList = res.data!;
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

  @HostListener('window:resize', ['$event'])
  onResize(event: any) {

    if (event.target.innerWidth < 992 && !this.isColumnsMobile) {

      this.displayedColumns = ['CreatorName', 'Actions'];
      this.isColumnsMobile = true;

    } else if (event.target.innerWidth >= 992 && this.isColumnsMobile) {

      this.displayedColumns = ['CreatorName', 'Active', 'Role', 'Collection', 'Actions'];
      this.isColumnsMobile = false;
    }

  }


}
