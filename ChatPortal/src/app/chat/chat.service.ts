import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable,map, Subject } from 'rxjs';
declare function EncryptFieldData(data:any): any;
declare function FrontEndEncryption(data:any): any;
declare function DecryptBackend(data:any): any;

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private BaseURL!:string;
  private NotificationList!: Subject<any>;
  private selectedNotification!: Subject<any>;
  constructor(private http: HttpClient) {
    this.NotificationList = new Subject<any>();
    this.selectedNotification = new Subject<any>();
    this.BaseURL =  environment.IsProd ? environment.BaseURL +'ChatService/' :environment.BaseURL;
   }

  getAllUsers(): Observable<any> {
    return this.http.get(this.BaseURL + 'api/Chat/GetAllUsers', {responseType:'text'}).pipe(map(res => {
      var data = JSON.parse(res)
     return data;
   }));;
  }
  getGPT(data:any): Observable<any> {
    return this.http.post(this.BaseURL + 'api/ChatGPT/GetGPTResul', data,{responseType:'text'}).pipe(map(res => {
      var data = JSON.parse(res)
     return data;
   }));;
  }
  postMessage(data:any){
    return this.http.post(this.BaseURL + 'api/Chat/PostMessage',data,{responseType:'text'}).pipe(map(res => {
      var data = JSON.parse(res)
     return data;
   }));
  }
  getUserChatList(userId:number): Observable<any> {
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json'}) };
    return this.http.get(this.BaseURL + `api/Chat/GetUserChatList?userId=${userId}`, {responseType:'text'}).pipe(map(res => {
      //var data:Array<object> = res;
      var data = JSON.parse(res)
      return data;
    }));
  }
  readMessage(data:any){
    return this.http.post(this.BaseURL + 'api/Chat/ReadMessage',data,{responseType:'text'}).pipe(map(res => {
      var data = JSON.parse(res)
     return data;
   }));
  }
  getNotificationList(userId:number): Observable<string> {
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json'}) };
    return this.http.get(this.BaseURL + `api/Chat/GetNotificationList?userId=${userId}`, {responseType:'text'}).pipe(map(res => {
    var data = JSON.parse(res)
     return data;
   }));;
  }
  readNotification(notificationId:number): Observable<any> {
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json'}) };
    return this.http.get(this.BaseURL + `api/Chat/ReadNotification?notificationId=${notificationId}`, {responseType:'text'});
  }
  getMessage(data:any){
    return this.http.post(this.BaseURL + 'api/Chat/GetMessage',data,{responseType:'text'}).pipe(map(res => {
      var data = JSON.parse(res);
     return data;
   }));
  }
  uploadAttacments(data:any){
    return this.http.post(this.BaseURL + 'api/Upload/Upload',data).pipe(map(res => {
     return res;
   }));
  }
  RemoveAttchmetns(data:any){
    return this.http.post(this.BaseURL + 'api/Upload/RemoveAttachment',data).pipe(map(res => {
     return res;
   }));
  }


  getDataStream<TDataShape>(): Observable<TDataShape> {
    return this.NotificationList.asObservable();
  }
  updateDataStream<TDataShape>(payload: TDataShape){
    this.NotificationList.next(payload);
  }
  getselectedNotification<TDataShape>(): Observable<TDataShape> {
    return this.selectedNotification.asObservable();
  }
  updateselectedNotification<TDataShape>(payload: TDataShape){
    this.selectedNotification.next(payload);
  }
}
