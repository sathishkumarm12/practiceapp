import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../models/member';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  members: Member[] = [];
  baseUrl = environment.apiBaseUrl;
  constructor(private http: HttpClient) { }

  getMembers() {
    if (this.members.length > 0)
      return of(this.members);

    return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
      map(membersData => {
        this.members = membersData;
        return this.members;
      })
    );
  }

  getMember(username: string) {
    const member = this.members.find(x => x.userName === username);

    if (member !== undefined) {
      return of(member);
    }

    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member) {
    return this.http.put<any>(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    )
  }
}
