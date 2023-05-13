import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { AuthenticationService } from 'src/app/core/services/auth.service';
import { UserService } from 'src/app/core/services/user.service';
import { CurrentUser } from 'src/app/core/models/currentUser';

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




  tiles: Tile[] = [
    { text: 'Traffic', cols: 1, rows: 1, color: 'lightblue' },
    { text: 'Products Sold', cols: 1, rows: 1, color: 'lightblue' },
    { text: 'Date Picker', cols: 1, rows: 1, color: 'lightgreen' },
    { text: 'Details [prin', cols: 3, rows: 1, color: '#DDBDF1' },
  ];

  constructor(private userService: UserService,
    private authService: AuthenticationService,
    private titleService: Title) {
  }

  ngOnInit() {
    this.currentUser = this.authService.getCurrentUser();
    this.titleService.setTitle('Brickalytics - Dashboard');
    this.getUserById(this.currentUser.id);
  }
  getUserById(id: number) {
    this.userService.getUserById(id).subscribe();
  }
}
