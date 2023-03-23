import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { map, Observable } from 'rxjs';
declare function DecryptBackend(data:any): any;
@Injectable({
  providedIn: 'root'
})
export class UserService {
  private BaseURL!:string;

  constructor(private http: HttpClient) {
    this.BaseURL =  environment.IsProd ? environment.BaseURL +'Authentication/' :environment.BaseURL;

  }
  register(encData:any){
    return this.http.post(this.BaseURL + `api/Account/Register`,encData,{responseType:'text'}).pipe(map(res => {
      var data = JSON.parse(res)
     return data;
   }));
  }
  login(encData:object){

    return this.http.post(this.BaseURL + `api/Account/Login`,encData,{responseType:'text'}).pipe(map(res => {
      var data = JSON.parse(res)
      return data;
   }));
  }
  oAuthLogin(encData:object){
    return this.http.post(this.BaseURL + `api/Account/OAuthLogin`,encData,{responseType:'text'}).pipe(map(res => {
      var data = JSON.parse(res)
      JSON.parse(res)
      return data;
    }));
  }
}
