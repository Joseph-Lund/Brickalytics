import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from 'src/app/core/services/auth.service';

@Component({
  selector: 'app-profile-details',
  templateUrl: './profile-details.component.html',
  styleUrls: ['./profile-details.component.css']
})
export class ProfileDetailsComponent implements OnInit {

  username: string = "";
  email: string = "";

  constructor(private authService: AuthenticationService) { }

  ngOnInit() {
    this.username = this.authService.getCurrentUser().username;
    this.email = this.authService.getCurrentUser().email;
  }

}
