import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../models/user';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl = environment.apiBaseUrl;

  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient, private presenceService: PresenceService) { }

  Login(model: any) {
    return this.http.post(this.baseUrl + 'account/login', model)
      .pipe(
        map(
          (data: User) => {
            if (data) {
              this.setCurrentUser(data);
              this.presenceService.createHubConnection(data);
            }
          }
        )
      );
  }

  setCurrentUser(user: User) {
    user.roles = [];
    const roles = this.getDecodedToken(user.token).role;
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  Logout() {
    localStorage.clear();
    this.currentUserSource.next(null);
    this.presenceService.stopHubConnection();
  }

  Register(model: any) {
    return this.http.post(this.baseUrl + 'account/register', model).pipe(
      map(
        (data: User) => {
          if (data) {
            this.setCurrentUser(data);
            this.presenceService.createHubConnection(data);
          }

          return data;
        }
      )
    );
  }

  getDecodedToken(token) {
    return JSON.parse(atob(token.split('.')[1]));
  }
}
