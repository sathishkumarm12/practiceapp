import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../models/member';
import { PaginatedResult } from '../models/pagination';
import { UserParams } from '../models/userParams';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  members: Member[] = [];
  baseUrl = environment.apiBaseUrl;
  paginatedResult: PaginatedResult<Member[]> = new PaginatedResult<Member[]>();
  memberCache = new Map();

  constructor(private http: HttpClient) { }

  getMembers(userParams: UserParams) {

    var cacheData = this.memberCache.get(Object.values(userParams).join('-'));

    if (cacheData) {
      return of(cacheData);
    };

    let params = new HttpParams();

    params = params.append("pageNumber", userParams.pageNumber);
    params = params.append("pageSize", userParams.pageSize);
    params = params.append("minAge", userParams.minAge);
    params = params.append("maxAge", userParams.maxAge);
    params = params.append("gender", userParams.gender);
    params = params.append("orderBy", userParams.orderBy);

    return this.http.get<Member[]>(this.baseUrl + 'users', { observe: 'response', params })
      .pipe(
        map(response => {
          this.paginatedResult.result = response.body;
          if (response.headers.has("Pagination")) {
            this.paginatedResult.pagination = JSON.parse(response.headers.get("Pagination"));
          }
          return this.paginatedResult;
        }),
        map(
          response => {
            this.memberCache.set(Object.values(userParams).join('-'), { ...response });
            return response;
          }
        )
      );
  }

  // (preValue, curVal) => preValue.concat(curVal.result), []
  getMember(username: string) {

    const member = [...this.memberCache.values()].reduce(
      (preValue, curVal) => preValue.concat(curVal.result), []
    )
      .find((member: Member) => member.userName === username);

    if (member) return of(member);

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

  setMainPhoto(photoId: number) {
    return this.http.put<any>(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete<any>(this.baseUrl + 'users/delete-photo/' + photoId, {});
  }

}
