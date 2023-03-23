import { DatePipe, formatDate } from '@angular/common';
import { Pipe, PipeTransform } from '@angular/core';
declare function EncryptFieldData(data:any): any;
declare function FrontEndDecryptionMessage(data:any): any;
@Pipe({ name: 'chatDate' })
export class ChatDatePipe implements PipeTransform{
  constructor(private datePipe:DatePipe){}
  transform(dateTime:Date) {

    const date = new Date(dateTime);
    const today = new Date();

    //Convert to local time
    const eDate = new Date(date.getTime() - date.getTimezoneOffset() * 60 * 1000);

    const formattedTime = this.datePipe.transform(eDate.toString(),'h:mma');
    const formattedDate = this.datePipe.transform(eDate.toString(),'MMM d');
    const formattedDay = this.datePipe.transform(eDate.toString(),'EEEE');

    const isToday = eDate.toDateString() === today.toDateString();
    const isYesterday = eDate.toDateString() === new Date(today.getTime() - 24 * 60 * 60 * 1000).toDateString();
    const isWithin7Days = (today.getTime() - eDate.getTime()) <= (7 * 24 * 60 * 60 * 1000);

    return isToday ? formattedTime : (isYesterday ? 'Yesterday' : (isWithin7Days ? formattedDay : formattedDate) );

}
}
@Pipe({name:'ellipse'})
export class EllipsePipe implements PipeTransform{
  constructor(){}
  transform(input:string) {
    return (input.length > 20) ? input.substring(0,20) + '...' : input;
  }
}

@Pipe({name:'decryptMessage'})
export class DecryptMessagePipe implements PipeTransform{
  constructor(){}
  transform(input:string) {
      return FrontEndDecryptionMessage(input);
  }
}