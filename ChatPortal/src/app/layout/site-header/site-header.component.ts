import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { ChatService } from 'src/app/chat/chat.service';
import { AuthenticationService } from 'src/app/service/authentication.service';
import { SignalEventType } from 'src/app/service/signalR/signal-event-type';
import { SignalRService } from 'src/app/service/signalR/signal-rservice';

@Component({
  selector: 'app-site-header',
  templateUrl: './site-header.component.html',
  styleUrls: ['./site-header.component.css']
})
export class SiteHeaderComponent implements OnInit {
  private subscription!: Subscription;
  public notificationList: Array<any> = []
  public notificationCount: number = 0;
  public userId:any;
  constructor(
    private authService: AuthenticationService, 
    private router: Router,
    private chatService: ChatService,
    private signal: SignalRService,) {
    this.userId = localStorage.getItem("userId");
     }

  ngOnInit(): void {
    //Subscribe method for reduce notification count
    this.subscription = this.chatService.getDataStream<any>().subscribe(notification => {
      this.notificationList = notification.notificationList;
      this.notificationCount = notification.notificationCount;
    })
    //this.getNotification();
    //SignalR Subscribe method for notificaiton update
    // this.signal.getDataStream<string>(SignalEventType.EVENT_THREE).subscribe(message => {
    //   this.getNotification();
    // })

  }
  logout() {
    this.authService.logout();
    window.location.reload()
    this.router.navigate(['/login']);
  }
  getNotification(){
    this.chatService.getNotificationList(this.userId).subscribe((res:any)=>{
      this.notificationList = res.notificationList;
      this.notificationCount = res.notificationCount
    })
  }
  selectMessage(data: any) {
    this.chatService.updateselectedNotification(data);
  }
}
