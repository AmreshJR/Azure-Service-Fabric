import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthenticationService } from '../service/authentication.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(
    private route: Router,
    private authService: AuthenticationService
  ) {}
  canActivate(): boolean {
    const token = this.authService.isLogged();

    if (token)
      return true;
    else {
      this.route.navigate(['/login']);
      return false;
    }
  }
}
