import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/core/models/user';
import { UserService } from 'src/app/core/services/user.service';

@Component({
  selector: 'app-profile-details',
  templateUrl: './profile-details.component.html',
  styleUrls: ['./profile-details.component.css']
})
export class ProfileDetailsComponent implements OnInit {

  userInfo: User | null = null;

  constructor(private userService: UserService) { }

  ngOnInit() {
  }

  getUserById(id: number){
    this.userService.getUserById(id).subscribe(user =>{
      this.userInfo = user;
    });
  }
  updateUser(user: User){
    this.userService.updateUser(user).subscribe(u =>{ });
  }

}
