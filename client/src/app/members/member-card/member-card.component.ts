import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Subject } from 'rxjs';
import { MembersService } from 'src/app/_services/members.service';
import { Member } from '../../_models/member';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {

  @Input() member: Member;
  currentLikes= new Subject<Partial<Member[]>>();

  @Output()
  public memberLikeStatusChanged = new EventEmitter<string>();

  constructor(
    private memberService: MembersService,
    private toastr: ToastrService
  ) { }

  ngOnInit(): void {
  }

  addLike() {
    this.memberService.addLike(this.member.username)
    .subscribe(response => {
      this.memberLikeStatusChanged.emit(response);
    });
  }
}
