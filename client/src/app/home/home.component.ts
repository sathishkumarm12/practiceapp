import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { take } from 'rxjs/operators';
import { AccountService } from '../services/account.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  registerMode = false;

  baseUrl = 'https://localhost:5001/api/'

  constructor(private http: HttpClient, private accountService: AccountService,
    private router: Router) { }

  ngOnInit(): void {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      if (user !== null) {
        this.router.navigateByUrl("/members");
      }
    });
  }

  registerToggle() {
    this.registerMode = !this.registerMode;
  }

  RegisterCancel(event: boolean) {
    this.registerMode = event;
  }

}
