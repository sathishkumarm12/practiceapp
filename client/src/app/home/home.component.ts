import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  registerMode = false;

  baseUrl = 'https://localhost:5001/api/'

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
  }

  registerToggle() {
    this.registerMode = !this.registerMode;
  }

  RegisterCancel(event: boolean) {
    this.registerMode = event;
  }

}
