import { Component, OnDestroy, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { AccountService } from '../services/account.service';
import { User } from '../models/user';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit, OnDestroy {

  model: any = {};
  currentUser$: Observable<User>;

  constructor(private accountService: AccountService,
    private router: Router,
    private toastr: ToastrService) { }

  ngOnInit(): void {
    this.currentUser$ = this.accountService.currentUser$;
  }

  ngOnDestroy() {

  }

  Login() {

    this.accountService.Login(this.model).subscribe(response => {
      this.router.navigate(['/members']);
    },
      error => {
        console.log(error);
        this.toastr.error(error.error);
      });

  }

  Logout() {
    this.accountService.Logout();
    this.router.navigate(['/home']);
  }

}
