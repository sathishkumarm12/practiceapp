import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryOptions, NgxGalleryImage, NgxGalleryAnimation } from '@kolkov/ngx-gallery';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/models/member';
import { MembersService } from 'src/app/services/members.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {

  member: Member;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];

  constructor(private memberService: MembersService, private activeRoute: ActivatedRoute) { }

  ngOnInit(): void {
    const username = this.activeRoute.snapshot.paramMap.get('username');
    this.loadMember(username);

    this.galleryOptions = [{
      width: '500px',
      height: '500px',
      imagePercent: 100,
      thumbnailsColumns: 4,
      imageAnimation: NgxGalleryAnimation.Slide,
      preview: false
    }];


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

  loadMember(username: string) {
    this.memberService.getMember(username).pipe(take(1)).subscribe(
      memberData => { this.member = memberData; this.galleryImages = this.getImages(); }
    );
  }

}
