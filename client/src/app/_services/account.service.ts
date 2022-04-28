import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl = environment.apiUrl;
  // ReplaySubject - an observable, works as a buffer. When subscribed on, emmits last value (specified in ())
  // (1) = how many users to keep in the buffer
  private currentUserSource = new ReplaySubject<User>(1);

  //asObservable - prevents from gettiong next value
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) { }

  public login(model: any) {
    return this.http.post(this.baseUrl + 'account/login', model)
      .pipe(map((response: User) => {
          const user = response;
        if (user) {
          this.setCurrentUser(user);
          }
        })
      )
  }

  setCurrentUser(user: User) {
    user.roles =[];
    const roles = this.getDecodedToken(user.token).role;    
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);// check if roles is an [] or a string
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }

  register(model: any) {
    return this.http.post(this.baseUrl + 'account/register', model)
      .pipe(
        map((user:User) => {
          if (user) {
            this.setCurrentUser(user);
          }
         return user;
        })
      )
  }

  // gets info from token
  getDecodedToken(token) {
    return JSON.parse(atob(token.split('.')[1]));
  }


}
