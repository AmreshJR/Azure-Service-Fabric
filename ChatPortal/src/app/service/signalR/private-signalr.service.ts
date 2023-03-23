import { Injectable } from '@angular/core';
import { HttpTransportType, HubConnection, HubConnectionBuilder, IHttpConnectionOptions, LogLevel } from '@microsoft/signalr';
import { filter, Observable, Subject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { SignalEvent } from './signal-event';
import { SignalEventType } from './signal-event-type';
import { SignalRService } from './signal-rservice';

@Injectable({
  providedIn: 'root'
})
export class PrivateSignalRService extends SignalRService {
  private _signalEvent: Subject<any>;
  private _openConnection: boolean = false;
  private _isInitializing: boolean = false;
  private _hubConnection!: HubConnection;

  constructor() {
    super();
    this._signalEvent = new Subject<any>();
    this._isInitializing = true;
    this._initializeSignalR();
  }
  getDataStream<TDataShape>(...filterValues: SignalEventType[]): Observable<SignalEvent<TDataShape>> {
    this._ensureConnection();
    return this._signalEvent.asObservable().pipe(filter(event => filterValues.some(f => f === event.type)));
  }
  private _ensureConnection() {
    if (this._openConnection || this._isInitializing)
     return;

    this._initializeSignalR();
  }

  private _initializeSignalR() {
    var jwtToken:any = localStorage.getItem("token");
    const options: IHttpConnectionOptions = {
      accessTokenFactory: () => {
        return jwtToken;
      },
      withCredentials:false,
    };
     this._hubConnection = new HubConnectionBuilder()
    .configureLogging(LogLevel.Error)
    .withUrl(environment.signalRBaseURL+'chat',options)
    .build();
    this._hubConnection.start()
      .then(_ => {
        this._openConnection = true;
        this._isInitializing = false;
        this._setupSignalREvents()
      })
      .catch(error => {
        console.warn(error);
        this._hubConnection.stop().then(_ => {
          this._openConnection = false;
        })
      });

  }

  private _setupSignalREvents() {
    this._hubConnection.on('updateChatList', (data) => {
      // map or transform your data as appropriate here:
      this._onMessage({type: SignalEventType.EVENT_ONE, data})
    })
    this._hubConnection.on('updateNewChat', (data) => {
      // map or transform your data as appropriate here:
      this._onMessage({type: SignalEventType.EVENT_TWO, data})
    })
    this._hubConnection.on('inAppNotification',(data)=> {
      this._onMessage({type:SignalEventType.EVENT_THREE, data})
    })
    this._hubConnection.onclose(() => {
      return this._openConnection = false;
    });
  }

  private _onMessage<TDataShape>(payload: SignalEvent<TDataShape>) {
    this._signalEvent.next(payload);
  }
}
