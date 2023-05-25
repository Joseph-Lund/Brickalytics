import { Component, OnInit } from '@angular/core';
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
  isRateLimitReached: boolean = false;
  showFirstLastButtons: boolean = true;
  pageEvent: PageEvent | null = null;
  displayedColumns: string[] = ['CreatorName', 'Active', 'Role', 'Collection', 'Actions'];

  constructor(
    private userService: UserService,
    private productTypeService: ProductTypeService,
    private titleService: Title,
    private notificationService: NotificationService,
    private dialog: MatDialog
  ) { }

  ngOnInit() {
    this.titleService.setTitle('Brickalytics - Users');
    this.getUsersList();
    this.getRolesList();
    this.getCollectionsList();
  }

  getUsersList(){
    this.userService.getUsers().subscribe(users =>{
      this.userList = users;
    },
    err =>{
      console.error('Error getting user list: ', err)
      this.notificationService.openSnackBar('Error getting user list');
    });
  }
  getRolesList(){
    this.userService.getRoles().subscribe(roles =>{
      this.roleList = roles;
    },
    err =>{
      console.error('Error getting role list: ', err)
      this.notificationService.openSnackBar('Error getting role list');
    });
  }
  getCollectionsList(){
    this.userService.getCollections().subscribe(collections =>{
      this.collectionList = collections;
    },
    err =>{
      console.error('Error getting role list: ', err)
      this.notificationService.openSnackBar('Error getting role list');
    });
  }
  getProductTypeList(){
    this.productTypeService.getProductTypes().subscribe(productTypes =>{
      this.productTypeList = productTypes;
    },
    err =>{
      console.error('Error getting product type list: ', err)
      this.notificationService.openSnackBar('Error getting product type list');
    });
  }
  addUser(user: User){
    this.userService.addUser(user).subscribe(userId =>{
      user.id = userId;
      this.userList.push(user);
    },
    err =>{
      console.error('Error adding user: ', err)
      this.notificationService.openSnackBar('Error adding user');
    });
  }
  updateUser(user: User){
    this.userService.updateUser(user).subscribe(userId =>{
      this.getUsersList();
    },
    err =>{
      console.error('Error updating user: ', err)
      this.notificationService.openSnackBar('Error updating user');
    });
  }
  updateUserPassword(user: User, password: string){
    this.userService.updateUserPassword(user.id!, password).subscribe(userId =>{},
    err =>{
      console.error('Error updating user password: ', err)
      this.notificationService.openSnackBar('Error updating user password');
    });
  }
  getRole(id:number | null){
    for(var role of this.roleList){
      if(role.id == id){
        return role.name;
      }
    }
    return "None";
  }
  getCollection(id:number){
    if(id){
      for(var collection of this.collectionList){
        if(collection.id == id){
          return collection.name;
        }
      }
    }
    return "None";
  }
  openUserModal(user: User | null = null){

    if(!user){
      user = new User(null, true, 2, 0, '', null);
    }

    const dialogRef = this.dialog.open(UserModal, {
      data: {user: user, roleList: this.roleList, collectionList: this.collectionList}
    });

    dialogRef.afterClosed().subscribe(_user => {
       //TODO: Make it so I dont need to call the api again to refresh the results.
       //      If new user added, add to the list and sort, if it already exists
       //      then update the item in the list.

       this.getUsersList();
    });
  }

  // handlePageEvent(e: PageEvent) {
  //   this.pageEvent = e;
  //   this.length = e.length;
  //   this.pageSize = e.pageSize;
  //   this.pageIndex = e.pageIndex;
  // }

  // setPageSizeOptions(setPageSizeOptionsInput: string) {
  //   if (setPageSizeOptionsInput) {
  //     this.pageSizeOptions = setPageSizeOptionsInput.split(',').map(str => +str);
  //   }
  // }


}
