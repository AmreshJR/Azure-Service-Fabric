import { Component, OnInit } from '@angular/core';
import { ChatService } from '../chat.service';
declare function EncryptFieldData(data:any): any;
declare function FrontEndEncryption(data:any): any;
declare function DecryptBackend(data:any): any;

@Component({
  selector: 'app-chat-bot',
  templateUrl: './chat-bot.component.html',
  styleUrls: ['./chat-bot.component.css']
})

export class ChatBotComponent implements OnInit {
  
  messages:any = [];
  newMessage: any;
  constructor(private chatService:ChatService) { }

  ngOnInit(): void {
  }
sendMessage() {
  if (this.newMessage) {
    var obj ={
      question:this.newMessage
    }
    this.messages.push({ text: this.newMessage.replace(/\\r\\n/g, "<br/>").replace(/\n/g, '<br/>'), sender: 'user' });

    var encData = {
      inputString : EncryptFieldData(JSON.stringify(obj))
    }
    this.newMessage = '';
      this.chatService.getGPT(encData).subscribe((res:any)=>{
        this.newMessage = res.answer.replace(/\\r\\n/g, "<br/>").replace(/\n/g, '<br/>');
        this.messages.push({ text: this.newMessage, sender: 'bot' });
        this.newMessage = '';
      })
  }
}
}
