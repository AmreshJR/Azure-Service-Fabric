import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { SocialAuthService } from '@abacritt/angularx-social-login';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  constructor(private http: HttpClient,private oAuthService: SocialAuthService) { }

  getUserToken(){
    var token = localStorage.getItem("token");
    return token;
  }
  isLogged():boolean{
    var token = localStorage.getItem("token");
    return !!token;
  }
  logout(){
    this.signOut();
    localStorage.clear();
  }
  signOut(): void {
    this.oAuthService.signOut();
  }
}
