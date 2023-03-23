import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './auth/auth.guard';
import { ChatComponent } from './chat/chat.component';
import { SiteLayoutComponent } from './layout/site-layout/site-layout.component';
import { IsLoggedGuard } from './user/is-logged.guard';
import { LoginComponent } from './user/login/login/login.component';
import { SignupComponent } from './user/signup/signup/signup.component';

const routes: Routes = [

  { path: 'login', component: LoginComponent,canActivate: [IsLoggedGuard],},
  { path: 'signup', component: SignupComponent },
  {
    path: '',
    component: SiteLayoutComponent,
    canActivate: [AuthGuard],
    children: [ 
      { path: '', component: ChatComponent},
      { path: 'chat', component: ChatComponent},
    ],
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
