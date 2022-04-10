import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-server-error',
  templateUrl: './server-error.component.html',
  styleUrls: ['./server-error.component.css']
})
export class ServerErrorComponent implements OnInit {

  error: any;

  constructor(private router: Router) {
    // only have access to currentNav here and have it only at the first load
    const navigation = this.router.getCurrentNavigation();
    this.error = navigation?.extras?.state?.error; //? optional
  }

  ngOnInit(): void {
  }

}
