import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AuthGuard } from './auth/auth.guard';
import { LoginComponent } from './user/login/login/login.component';
import { SignupComponent } from './user/signup/signup/signup.component';
import { ChatComponent } from './chat/chat.component';
import { ToastrModule } from 'ngx-toastr';
import { TokenIntercepterService } from './auth/auth.interceptor';
import { PickerModule } from '@ctrl/ngx-emoji-mart';
import { IsLoggedGuard } from './user/is-logged.guard';
import { NgxUiLoaderHttpModule, NgxUiLoaderModule, NgxUiLoaderRouterModule } from 'ngx-ui-loader';
import { ChatDatePipe, DecryptMessagePipe, EllipsePipe } from './customPipe/custom.pipe';
import { DatePipe } from '@angular/common';
import { SignalRService } from './service/signalR/signal-rservice';
import { PrivateSignalRService } from './service/signalR/private-signalr.service';
import { SiteLayoutComponent } from './layout/site-layout/site-layout.component';
import { SiteFooterComponent } from './layout/site-footer/site-footer.component';
import { SiteHeaderComponent } from './layout/site-header/site-header.component';
import { FacebookLoginProvider, GoogleLoginProvider, SocialAuthServiceConfig, SocialLoginModule } from '@abacritt/angularx-social-login';
import { environment } from 'src/environments/environment';
import { ChatBotComponent } from './chat/chat-bot/chat-bot.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    SignupComponent,
    ChatComponent,
    ChatDatePipe,
    EllipsePipe,
    DecryptMessagePipe,
    SiteLayoutComponent,
    SiteFooterComponent,
    SiteHeaderComponent,
    ChatBotComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    BrowserAnimationsModule,
    ToastrModule.forRoot({
      //timeOut: 10000,
      //positionClass: 'toast-top-right',
      progressBar: true,
      maxOpened:1,
      preventDuplicates: true
    }),
    PickerModule,
    SocialLoginModule,
    NgxUiLoaderModule.forRoot({
      "bgsColor": "#007bff",
      "bgsOpacity": 0.5,
      "bgsPosition": "bottom-right",
      "bgsSize": 60,
      "bgsType": "square-jelly-box",
      "blur": 5,
      "delay": 0,
      "fastFadeOut": true,
      "fgsColor": "#007bff",
      "fgsPosition": "center-center",
      "fgsSize": 60,
      "fgsType": "ball-spin-clockwise",
      "gap": 24,
      "masterLoaderId": "master",
      "overlayBorderRadius": "0",
      "overlayColor": "rgba(40, 40, 40, 0.8)",
      "hasProgressBar": false,
      "text":"Fetching Data.."
    }),
    NgxUiLoaderRouterModule.forRoot({
      "showForeground": false
    }),
    NgxUiLoaderHttpModule.forRoot({
      "showForeground":true,
      excludeRegexp:["Message","Notification","Notification","ChatGPT"]
    })
  ],
  providers: [AuthGuard,IsLoggedGuard,DatePipe,
    { provide: HTTP_INTERCEPTORS, useClass: TokenIntercepterService, multi: true },
    { provide: SignalRService,useClass: PrivateSignalRService},
    {
      provide: 'SocialAuthServiceConfig',
      useValue: {
        autoLogin: false,
        providers: [
          {
            id: GoogleLoginProvider.PROVIDER_ID,
            provider: new GoogleLoginProvider(environment.GoogleClientId)
          }
          // ,
          // {
          //   id: FacebookLoginProvider.PROVIDER_ID,
          //   provider: new FacebookLoginProvider(environment.GoogleClientId)
          // }
        ],
        onError: (err:any) => {
          console.error(err);
        }
      } as SocialAuthServiceConfig,
    }  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
