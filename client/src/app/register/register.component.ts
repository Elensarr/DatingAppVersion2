import { error } from '@angular/compiler/src/util';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  @Input() usersFromHomeComponent: any;
  @Output() cancelRegister = new EventEmitter();

  model: any = {};

  constructor(private accountService: AccountService,
    private toastr: ToastrService  ) { }

  ngOnInit(): void {
  }

  register() {
    this.accountService.register(this.model).subscribe(
      response => {
        console.log("Register response ", response);
        this.cancel();
      },
      error => this.toastr.error(error.error)
    )
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
