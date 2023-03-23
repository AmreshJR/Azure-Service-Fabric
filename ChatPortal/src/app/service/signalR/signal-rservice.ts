import { Observable } from "rxjs";
import {Injectable} from '@angular/core';
import { SignalEventType } from "./signal-event-type";
import { SignalEvent } from "./signal-event";

@Injectable()
export abstract class SignalRService {
  abstract getDataStream<TDataShape>(...filterValues: SignalEventType[]): Observable<SignalEvent<TDataShape>>
}
