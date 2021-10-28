import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/models/member';
import { IPagination } from 'src/app/models/pagination';
import { User } from 'src/app/models/user';
import { UserParams } from 'src/app/models/userParams';
import { AccountService } from 'src/app/services/account.service';
import { MembersService } from 'src/app/services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {

  //members$: Observable<Member[]>;
  members: Member[];
  paginationData: IPagination;
  userParams: UserParams;
  user: User;
  genderList = [{ value: 'female', display: 'Males' }, { value: 'male', display: 'Females' }];

  constructor(private memberService: MembersService, private accountService: AccountService) {
    accountService.currentUser$.pipe(take(1)).subscribe(userData => {
      this.user = userData;
      this.userParams = new UserParams(userData);
    });
  }

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers() {

    this.memberService.getMembers(this.userParams).subscribe(
      data => {
        this.members = data.result;
        this.paginationData = data.pagination;
      }
    );
  }

  resetFilters() {
    this.userParams = new UserParams(this.user);
    this.loadMembers();

  }

  PageChanged(event: any) {
    this.userParams.pageNumber = event.page;

    this.loadMembers();
  }
}
