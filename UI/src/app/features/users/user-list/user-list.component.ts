import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { HttpService } from 'src/app/core/services/http.service';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css']
})
export class UserListComponent implements OnInit {

  constructor(
    private httpService: HttpService,
    private titleService: Title,
  ) { }

  ngOnInit() {
    this.titleService.setTitle('Brickalytics - Users');
  }

  getUsersList(){

  }


}
