import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl = environment.apiBaseUrl;

  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) { }

  Login(model: any) {
    return this.http.post(this.baseUrl + 'account/login', model)
      .pipe(
        map(
          (data: User) => {
            if (data) {
              this.setCurrentUser(data);
            }
          }
        )
      );
  }

  setCurrentUser(user: User) {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  Logout() {
    localStorage.clear();
    this.currentUserSource.next(null);
  }

  Register(model: any) {
    return this.http.post(this.baseUrl + 'account/register', model).pipe(
      map(
        (data: User) => {
          if (data) {
            this.setCurrentUser(data);
          }

          return data;
        }
      )
    );
  }
}
