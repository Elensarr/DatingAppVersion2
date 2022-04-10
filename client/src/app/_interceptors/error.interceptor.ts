import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, pipe, throwError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(
    private router: Router, // to redirect to error page
    private toastr: ToastrService // show some errors
  ) { }

  // intercept request or response (in next)
  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request) // want to catch errors
      .pipe(
        catchError(error => {
          if (error) {
            switch (error.status) {
              case 400:
                if (error.error.errors) {
                  const modalStateErrors = [];
                  for (const key in error.error.errors) {
                    if (error.error.errors[key]) {
                      modalStateErrors.push(error.error.errors[key]);// flattening array of errors
                    }
                  }
                  throw modalStateErrors.flat();// flattening array of errors
                } else {
                  this.toastr.error(error.statusText, error.status);
                }
                break;
              case 401:
                this.toastr.error(error.statusText, error.status);
                break;
              case 404:
                this.router.navigateByUrl('/not-found');
                break;
              case 500:
                const navigationExtras: NavigationExtras = {
                  state: { error: error.error }
                }
                this.router.navigateByUrl('/server-error', navigationExtras);
                break;
              default:
                this.toastr.error('Something went wrong');
                console.log(error);
                break;
            }
          }
          return throwError(error);
        })
      );
  }
}
