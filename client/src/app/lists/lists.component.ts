import { Component, OnInit } from '@angular/core';
import { LikeParams } from '../models/likeParams';
import { Member } from '../models/member';
import { IPagination } from '../models/pagination';
import { MembersService } from '../services/members.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {

  members: Partial<Member[]>;
  likeParams: LikeParams;
  paginationData: IPagination;

  constructor(private memberService: MembersService) {
    this.likeParams = new LikeParams();
  }

  ngOnInit(): void {

    this.getLikes();
  }

  getLikes() {
    this.memberService.getLikes(this.likeParams).subscribe(memberList => {
      this.members = memberList.result;
      this.paginationData = memberList.pagination;
    });
  }

  PageChanged(event: any) {
    this.likeParams.pageNumber = event.page;
    this.getLikes();
  }

}
