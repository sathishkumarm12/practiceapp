import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryOptions, NgxGalleryImage, NgxGalleryAnimation } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/models/member';
import { Message } from 'src/app/models/message';
import { User } from 'src/app/models/user';
import { AccountService } from 'src/app/services/account.service';
import { MembersService } from 'src/app/services/members.service';
import { MessageService } from 'src/app/services/message.service';
import { PresenceService } from 'src/app/services/presence.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit, OnDestroy {

  member: Member;
  messages: Message[] = [];

  user: User;

  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];

  @ViewChild('memberTabs', { static: true }) memberTabs: TabsetComponent;
  activeTab: TabDirective;

  constructor(public presence: PresenceService, private activeRoute: ActivatedRoute,
    private messageService: MessageService, private accountService: AccountService) {
    accountService.currentUser$.pipe(take(1)).subscribe(data => this.user = data);
  }
  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }

  ngOnInit(): void {
    //const username = this.activeRoute.snapshot.paramMap.get('username');
    //this.loadMember(username);

    this.activeRoute.data.subscribe(data => this.member = data.member);

    this.galleryOptions = [{
      width: '500px',
      height: '500px',
      imagePercent: 100,
      thumbnailsColumns: 4,
      imageAnimation: NgxGalleryAnimation.Slide,
      preview: false
    }];

    this.activeRoute.queryParams.subscribe(params => {
      params?.tab ? this.selectTab(params.tab) : this.selectTab(0);
    });

    this.galleryImages = this.getImages();
  }

  getImages(): NgxGalleryImage[] {
    const imageUrls = [];

    for (const image of this.member.photos) {
      imageUrls.push({
        small: image?.url,
        medium: image?.url,
        big: image?.url
      });
    }

    return imageUrls;
  }

  // loadMember(username: string) {
  //   this.memberService.getMember(username).pipe(take(1)).subscribe(
  //     memberData => { this.member = memberData; }
  //   );
  // }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;

    if (this.activeTab.heading === 'Messages' && this.messages.length === 0) {
      this.messageService.createHubConnection(this.user, this.member.userName);
    }
    else {
      this.messageService.stopHubConnection();
    }
  }

  selectTab(tabId: number) {
    this.memberTabs.tabs[tabId].active = true;
  }

  loadMessages() {

    this.messageService.getMessageThread(this.member.userName).subscribe(
      response => {
        this.messages = response;
      }
    );
  }

}
