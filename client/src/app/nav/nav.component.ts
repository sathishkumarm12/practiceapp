import { Component, OnDestroy, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { AccountService } from '../Services/account.service';
import { User } from '../models/user';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit, OnDestroy {

  model: any = {};
  currentUser$: Observable<User>;

  constructor(private accountService: AccountService) { }

  ngOnInit(): void {
    this.currentUser$ = this.accountService.currentUser$;
  }

  ngOnDestroy() {

  }

  Login() {

    this.accountService.Login(this.model).subscribe(response => {

    },
      error => {
        console.log(error);

      });

  }

  Logout() {
    this.accountService.Logout();

  }

}
