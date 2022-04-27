import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { of } from 'rxjs';
import { map, switchMap, take, tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { User } from '../_models/user';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';


@Injectable({
  providedIn: 'root'
})
export class MembersService {
  
  members: Member[] = [];
  baseUrl = environment.apiUrl;
  memberCashe = new Map(); // Map object similar to dic, stores info in key-value format
  user: User;
  userParams: UserParams;


  constructor(private http: HttpClient,
    private accountService: AccountService,
    private toastr: ToastrService) {

    this.accountService.currentUser$.pipe(
      take(1))
      .subscribe(
        user => {
          this.user = user;
          this.userParams = new UserParams(user);
        }
      )

  } // for being able to store userParams

  getUserParams() {
    return this.userParams;
  }

  setUserParams(params: UserParams) {
    this.userParams = params;
  }

  resetUserParams() {
    this.userParams = new UserParams(this.user);
    return this.userParams;
}

  getMembers(userParams: UserParams) {
    // cashing
    // Object.values(userParams).join("-")) - used as a key //18-99-1-5-created-female
    var response = this.memberCashe.get(Object.values(userParams).join("-"));
    // if we have response for this key, take it from cache
    if (response) {
      return of(response);
    }

    let params = getPaginationHeaders(userParams.pageNumber, userParams.pageSize); // to serialize params
        
    // params to add to http request
    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);    

    
    return getPaginatedResult<Member[]>(this.baseUrl + 'users', params, this.http)
      .pipe(map(response => {
        this.memberCashe.set(Object.values(userParams).join("-"), response);
        return response;
      }));
  }

  

  getMember(username: string) {
    //const member = this.members.find(
    //  x => x.username === username
    //);
    //if (member !== undefined) {
    //  return of(member);
    //}

    const member = [...this.memberCashe.values()]
      .reduce((arr, elem) => arr.concat(elem.result), []) // reduces array
      .find((member: Member) => member.username === username);

    return this.http.get<Member>(this.baseUrl + 'users/'+ username);
  }
  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member)
      .pipe(
        map(
          () => {
            const index = this.members.indexOf(member);
            this.members[index] = member;
          }
        )
    )
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + "users/set-main-photo/" + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + "users/delete-photo/" + photoId);
  }

  //likes
  addLike(username:string) {
    let url_string=this.baseUrl + "likes/"+ username;    
    return this.http.post(this.baseUrl + "likes/"+ username, null, { responseType: "text" })// responseType specified as expect Json forma of response
    .pipe(
      tap(
        response=> {
          if (response=="liked") 
        {
          this.toastr.success("You have liked " + username);
        }
        else if (response=="disliked") {
          this.toastr.error("You have disliked " + username);
        }
        }
      )      
    ); 
    
  }

  getLikes(predicate:string, pageNumber: number,  pageSize: number) {    
    let params =  getPaginationHeaders(pageNumber, pageSize);
    params = params.append('predicate', predicate);
    return  getPaginatedResult<Partial<Member[]>>(this.baseUrl + "likes", params, this.http);
  }

  
}
