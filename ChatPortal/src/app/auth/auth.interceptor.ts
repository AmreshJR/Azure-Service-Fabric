import {   HttpRequest, HttpHandler, HttpEvent, HttpInterceptor} from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { AuthenticationService } from '../service/authentication.service';

@Injectable({
  providedIn: 'root'
})
export class TokenIntercepterService implements HttpInterceptor{
  constructor(private authService:AuthenticationService,
    private router: Router) {  }
  intercept(req:any, next:any): Observable<HttpEvent<any>> {
    let token = this.authService.getUserToken();
    let tokenizedReq = req.clone({
        setHeaders: {
          Authorization: token ? `Bearer ${token}` :''
        }
      })
              // return next.handle(authReq);
              return next.handle(tokenizedReq).pipe(
                tap(
                    succ => {
                      console.log(succ);
                     },
                    err => {
                        if (err.status == 401){
                            localStorage.removeItem('token');
                            this.router.navigateByUrl('/login');
                        }else if (err.status == 403){
                            this.router.navigateByUrl('/login');
                        }
                    }
                )
            );
  }
}
