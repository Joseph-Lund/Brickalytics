import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { GenericType } from 'src/app/core/models/genericType';
import { ProductSubType } from 'src/app/core/models/productSubType';
import { User } from 'src/app/core/models/user';
import { NotificationService } from 'src/app/core/services/notification.service';
import { ProductSubTypeService } from 'src/app/core/services/productSubType.service';
import { ProductTypeService } from 'src/app/core/services/productType.service';
import { RoleService } from 'src/app/core/services/role.service';
import { UserService } from 'src/app/core/services/user.service';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css']
})
export class UserListComponent implements OnInit {

  userList: User[] = [];
  roleList: GenericType[] = [];
  productTypeList: GenericType[] = [];
  productSubTypeList: ProductSubType[] = [];

  constructor(
    private userService: UserService,
    private roleService: RoleService,
    private productTypeService: ProductTypeService,
    private productSubTypeService: ProductSubTypeService,
    private titleService: Title,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.titleService.setTitle('Brickalytics - Users');
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
    this.roleService.getRoles().subscribe(roles =>{
      this.roleList = roles;
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
  getProductSubTypeList(){
    this.productSubTypeService.getProductSubTypes().subscribe(productSubTypes =>{
      this.productSubTypeList = productSubTypes;
    },
    err =>{
      console.error('Error getting product sub type list: ', err)
      this.notificationService.openSnackBar('Error getting product sub type list');
    });
  }
  getCollectionsList(){
    this.userService.getUsers().subscribe(users =>{
      this.userList = users;
    },
    err =>{
      console.error('Error getting collections list: ', err)
      this.notificationService.openSnackBar('Error getting collections list');
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
    this.userService.updateUserPassword(user.id, password).subscribe(userId =>{},
    err =>{
      console.error('Error updating user password: ', err)
      this.notificationService.openSnackBar('Error updating user password');
    });
  }

  // getUserById(id: number): User{
  //   var userToReturn;
  //   this.userService.getUserById(id).subscribe(user =>{
  //     userToReturn = user;
  //   });
  // }


}
