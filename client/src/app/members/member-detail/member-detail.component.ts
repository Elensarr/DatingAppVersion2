import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_services/message.service';
import { Member } from '../../_models/member';
import { MembersService } from '../../_services/members.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  
  @ViewChild('memberTabs', {static:true}) memberTabs: TabsetComponent;
  activeTab: TabDirective;
  member: Member;

  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  messages: Message[]=[];

  constructor(
    private memberService: MembersService,
    private route: ActivatedRoute,
    private messageService: MessageService
  ) { }

  ngOnInit(): void {

    this.route.data.subscribe(
      data => {
        this.member = data.member;
      }
    )

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
    this.galleryImages = this.getImages();

    this.route.queryParams.subscribe(
      params => {
        params.tab ? this.selectTab(params.tab) : this.selectTab(0);
      }
    );
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

    onTabActivated(data: TabDirective) {
      this.activeTab = data;
      if (this.activeTab.heading==='Messages' && this.messages.length===0) {
        this.loadMessages();
      }
    }

    loadMessages() {
      this.messageService.getMessageThread(this.member.username)
        .subscribe(
          messages => {
            this.messages = messages;
          }
        );
    }

    selectTab(tabId: number) {
      this.memberTabs.tabs[tabId].active=true;
    }
  }

