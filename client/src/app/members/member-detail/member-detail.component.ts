import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { NgxGalleryImageComponent } from '@kolkov/ngx-gallery/lib/ngx-gallery-image/ngx-gallery-image.component';
import { Member } from '../../_models/member';
import { Photo } from '../../_models/photo';
import { MembersService } from '../../_services/members.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  member: Member;

  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];

  constructor(
    private memberService: MembersService,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    this.loadMember();

    this.galleryOptions = [
      {
        width: '500px',
        height: '400px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ]
  }
    
  getImages(): NgxGalleryImage[] {
    const imageUrls = [];
    this.member.photos.forEach(
      photo => {        
        imageUrls.push({
          small: photo?.url,
          medium: photo?.url,
          big: photo?.url
        })
      }
    )
    return imageUrls;
  }

  loadMember() {
    this.memberService.getMember(this.route.snapshot.paramMap.get('username'))
      .subscribe(
        member => {
          this.member = member;
          this.galleryImages = this.getImages();
        }
    );
   
      
    }
  }

