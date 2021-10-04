import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Practice app';
  users: any;

  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.getUsers();
  }

  getUsers() {
    this.http.get<[{ id: number, username: string }]>("https://localhost:5001/api/users")
      .subscribe(
        response => {
          this.users = response;
        },
        error => {
          console.log(error);
        }
      )
  }

}
