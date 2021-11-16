import { Component, OnInit } from '@angular/core';
import { AccountService } from './services/account.service';
import { User } from './models/user'
import { PresenceService } from './services/presence.service';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Practice app';
  users: any;

  constructor(private accountService: AccountService, private presence: PresenceService) { }

  ngOnInit() {
    this.setCurrentUser();
  }

  setCurrentUser() {
    const user: User = JSON.parse(localStorage.getItem('user'));
    if (user) {
      this.accountService.setCurrentUser(user);
      this.presence.createHubConnection(user);
    }
  }
}
