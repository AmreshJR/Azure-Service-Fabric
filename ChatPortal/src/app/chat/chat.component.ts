import {ChangeDetectorRef, Component, ElementRef, OnDestroy, OnInit, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { AbstractControl, UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { EmojiService } from '@ctrl/ngx-emoji-mart/ngx-emoji';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { ChatService } from './chat.service';
import { forkJoin, map, Subscription } from 'rxjs';
import { SignalRService } from '../service/signalR/signal-rservice';
import { SignalEventType } from '../service/signalR/signal-event-type';
import { Event, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { dtoCurrentChat } from '../dto/dtoChat';
import { AuthenticationService } from '../service/authentication.service';
declare function EncryptFieldData(data:any): any;
declare function FrontEndEncryption(data:any): any;
declare function FrontEndDecryptionMessage(data:any): any;
declare function DecryptBackend(data:any): any;

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit, OnDestroy{
@ViewChild('scrollMe') private myScrollContainer!: ElementRef;
@ViewChildren('messages') messages!: QueryList<any>;
@ViewChild('emojiInput') emojiInput!: ElementRef;
 messageForm: UntypedFormGroup = new UntypedFormGroup({});

  // Subscribe for data change when new message send or receiver
    private subscription!: Subscription;
    private notificationSubscribe!: Subscription;
    attachmentIds: any;
    public userId:any;
    public messageText!:string;
    public currentChat:dtoCurrentChat =
    {
      chatIndex:null,
      message:[],
      receiverId:0,
      userName:'',    
      profilePicture:''
    };
    public chatList:Array<any> = [];
    public userList:Array<any> = [];
    public userSearchInput!:string;
    public userSearchList:Array<any> = [];
    public chatSearchList:Array<any> = [];
    public chatSearchInput!:string;
    public tempChatIndex!:number;
    public listChanged:boolean = false;
    public previousScrollHeightMinusTop:any;
    public previousIndex:number = 0;
    public isLazyLoadingCompleted = true;
    public fileSize:boolean = false;
    public fileType:boolean = false;
    public uploadedAttachments: any;
    public previousCarotIndex:number = 0;
    public showUsers:boolean = false;
    public showEmoji:boolean = false;
    public showChatBot:boolean = false;
    public InitialLoad:boolean = true;
    constructor(
      private fb: UntypedFormBuilder,
      private ngxService: NgxUiLoaderService,
      private chatService:ChatService,
      private signal: SignalRService,
      private cdref: ChangeDetectorRef,
      private authService: AuthenticationService,
      private router: Router,
      private toastr: ToastrService) {
        this.userId = localStorage.getItem("userId");
      }
    ngOnInit() {
    this.initializeForm();
    this.getAllUsers();
    this.chatSearchList = this.chatList;

    this.subscription = this.signal.getDataStream<string>(SignalEventType.EVENT_ONE).subscribe(message => {
      this.updateChatList(message.data);
    })

    this.subscription = this.signal.getDataStream<string>(SignalEventType.EVENT_TWO).subscribe(message => {
      this.updateNewChat(message.data);
    })

    // this.notificationSubscribe = this.chatService.getselectedNotification<any>().subscribe(notification => {
    //   this.selectUserNotify(notification)
    // })

    this.scrollToBottom()
  }
  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  initializeForm():void {
    this.messageForm = this.fb.group({
      message: [
        '',
        [
          Validators.required,
        ],
      ]
    });
  }
  
  addEmoji(event:any,emojiInputField:HTMLInputElement): void{
    const { selectionStart } = emojiInputField;
    const value = this.Message?.value;
    if (!value) {
      this.Message?.patchValue(event.emoji.native);
    } else {
      this.Message?.patchValue(`${value.substring(0, selectionStart)}${event.emoji.native}${value.substring(selectionStart)}`);
    }
    setTimeout(() => emojiInputField.selectionStart = selectionStart+event.emoji.native.length);
  }

  onSubmit():void{
    const { userId, currentChat } = this;

    if(!userId || !this.Message?.value || !currentChat.receiverId){
      return;
    }
    const messageContent = FrontEndEncryption(this.Message?.value);

    const data ={
      senderId: this.userId,
      receiverId: this.currentChat.receiverId.toString(),
      messageContent: messageContent
    };
    const encData = {
      inputString : EncryptFieldData(JSON.stringify(data))
    }
    this.chatService.postMessage(encData).subscribe(()=>{})
    this.messageForm.reset();
  }
  getAllUsers(): void {
    const allUsers$ = this.chatService.getAllUsers().pipe(map((response:any) => response.filter((x:any) => x.userId !== this.userId)));

    const userChatList$ = this.chatService.getUserChatList(this.userId);

    forkJoin({ allUsers$, userChatList$ }).subscribe({
      next: ({ allUsers$, userChatList$ }) => {
        this.userList = allUsers$;
        this.userSearchList = this.userList.slice();
        this.chatList = userChatList$;
        this.chatSearchList = this.chatList;
        //this.updateCurrentChat(userChatList$[0], 0);
      },
      error: (error) => {
        console.log(error)
      }
    });
  }

  //Method to update currentChat Object.
  updateCurrentChat(chat: any, index: number): void {
    this.InitialLoad = false;
    this.currentChat = {
      message: chat.message,
      receiverId: chat.receiverId,
      userName: chat.receiverName,
      profilePicture: chat.receiverProfilePicture,
      chatIndex: index
    };
  }


  //searching in all user tab.
  searchUserList() : void {
    if(!this.userSearchInput)
    {
      this.userSearchList = [];
      this.userSearchList = this.userList.filter(x => x.userId !== this.userId);
    }
    else{
      this.userSearchList = [];
      const input = this.userSearchInput.toLowerCase();
      this.userSearchList = this.userList.filter(x => x.userName.toLowerCase().includes(input) && x.userId !== this.userId);
    }
  }

  //searching in recent chat
  searchChatList() : void {
    if(!this.chatSearchInput)
    {
      this.chatSearchList = [...this.chatList];
      this.currentChat.chatIndex = this.tempChatIndex;
    }
    else{
      if(this.currentChat.chatIndex !== -1){
        this.tempChatIndex = this.currentChat.chatIndex
      }

      this.currentChat.chatIndex = -1;
      this.chatSearchList = [];
      const input = this.chatSearchInput.toLowerCase();
      for (const x of this.chatList) {
        const userName = x.receiverName?.toLowerCase();
        if (userName?.includes(input) && !this.chatSearchList.includes(x)) {
          this.chatSearchList.push(x);
        }
      }
    }
  }

  //Show or Add New Conversation in recent chat tab.
  startNewChat(userDetail:any) : void {
    //If Chat Already exist in Recent Chat.
    var existingChat  = this.chatSearchList.find(x=>x.receiverId == userDetail.userId);
    this.removeNewChat();
    if(existingChat){
      var index = this.chatSearchList.indexOf(existingChat);
      this.updateCurrentChat(existingChat, index);
    } 
    //For new Chat
    else{
      var newChatObject = {
        message:[],
        unreadMessageCount: 0,
        senderId:this.userId,
        senderName:'',
        receiverName:userDetail.userName,
        receiverProfilePicture:userDetail.profilePicture,
        receiverId:userDetail.userId,
        lastMessage:'Draft',
        lastMessageTime:'',
      }
      this.chatSearchList.push(newChatObject);
      var index = this.chatSearchList.indexOf(newChatObject);
      this.currentChat.message = [],
      this.updateCurrentChat(newChatObject,index);
      this.messageForm.reset();
    }
    this.showUsers = !this.showUsers;
    this.userSearchInput = '';
    this.searchUserList();
  }

  //Selecting user for futher message posting.
  changeSelectedUser(index:number,chat:any) : void{
    if(index == this.currentChat.chatIndex){
      return;
    }

    this.updateCurrentChat(chat,index);
    this.removeNewChat();
    this.messageForm.reset();
    this.chatSearchInput = '';
    this.searchChatList();
    var newIndex = this.chatList.indexOf(this.chatList.find(x=>x.chatId == chat.chatId));
    this.currentChat.chatIndex = newIndex;
    this.scrollToBottom();
  }

  removeNewChat() :void{
    const newChatIndex = this.chatSearchList.findIndex(chat => chat.message.length === 0);
    if (newChatIndex !== -1) {
      this.chatSearchList.splice(newChatIndex, 1);
    }

  }

  hideEmojiPopup(): void{
   this.showEmoji = false;
  }

  //update chat new message for both sender and receiver
  updateChatList(message:any) : void{                                           
    const chatIndex  = this.chatList.findIndex(x => x.chatId === message.chatId);
    const chat = this.chatList[chatIndex];
      this.chatList.splice(chatIndex,1);
      const messageIndex = chat.message.indexOf(message);
      if(messageIndex === -1 && message.senderId !== this.userId)
      {
        chat.unreadMessageCount +=1
      }
      chat.message.push(message);
      chat.lastMessage = message.messageContent;
      chat.messageTime = message.createdOn;
      this.chatList.unshift(chat);
      if(this.currentChat.chatIndex == chatIndex)
      {
        this.updateCurrentChat(chat,chatIndex);
      }
      else
      {
        const oldChatIndex = this.chatList.findIndex(x => x.chatId === message.chatId);
        this.currentChat.chatIndex = oldChatIndex;
      }
      this.chatSearchList = [];
      this.chatSearchList = [...this.chatList];
      this.scrollToBottom();
  }

  //Updating newChat Conversation for sendeer & receiver.
  updateNewChat(chat:any) : void{
    this.removeNewChat();
    if(this.chatList.length <= 0){
      this.chatList.unshift(chat);
      this.updateCurrentChat(chat,0);
    }
    //This else block define newChat is updated for receiver
    else if(chat.message[chat.message.length-1].senderId != this.userId){
        this.chatList.unshift(chat);
        this.currentChat.chatIndex += 1;
    }
      //This else block define newChat is updated for sender
    else if(chat.message[chat.message.length-1].senderId == this.userId){
      this.chatList.unshift(chat);
      this.updateCurrentChat(chat,0);
    }
    this.chatSearchList = [];
    this.chatSearchList = this.chatList.filter(x=>x);
  }

  readMessage() : void{
    var chat = this.chatList.filter(x=>x.receiverId == this.currentChat.receiverId)[0];
    if(chat?.unreadMessageCount>0){
      var data = {
        senderId : this.userId.toString(),
        receiverId: this.currentChat.receiverId.toString()
      }
      this.chatService.readMessage(data).subscribe(response=>{
            chat.unreadMessageCount = 0;
      })
    }
  }
  get Message(): AbstractControl  {
    return this.messageForm.get('message') as AbstractControl;
  }


getNotificationList(): void{
  this.chatService.getNotificationList(this.userId).subscribe((response=>{
    this.chatService.updateDataStream(response);
  }))
}

selectUserNotify(data:any) : void
{
  var chat = this.chatList.filter(x=>x.receiverId == data.userId)[0];
  var index = this.chatList.indexOf(chat);
  this.changeSelectedUser(index,chat);
  this.chatService.readNotification(data.notificationId).subscribe(response=>{
    this.getNotificationList();
  })
}

//Get message when scroll.
pagenateMessage(event:any) : void{
  const target = event.target;
  const { scrollTop } = target;
  if(scrollTop !== 0 || !this.isLazyLoadingCompleted) {
    return;
  }
  this.isLazyLoadingCompleted = false;
  const chatId = this.currentChat.message[0].chatId;
  const chat = this.chatList.find(c => c.message[0].chatId === chatId);

  if(!chat || chat.isCompletedChat) {
    this.isLazyLoadingCompleted = true;
    return;
  }

  const data = {
    oldListCount:this.currentChat.message.length,
    noOfData:10,
    chatId
  }
  if(!chat.isCompletedChat){
   this.ngxService.startBackground();
   this.prepareFor();
    this.chatService.getMessage(data).subscribe((res:any)=>{
      if(res.messageList){
       res.messageList.map((x:any)=>{
         if(chat.message.indexOf(x) == -1){
          chat.message.unshift(x);
          this.cdref.detectChanges();
          setTimeout(() => {
            this.restore();
          });
         }
       })
      }
      chat.isCompletedChat = res.isListCompleted;
      this.isLazyLoadingCompleted = true;
     this.ngxService.stopBackground();
    })
  }
  else
  this.isLazyLoadingCompleted = true;
 
}

scrollToBottom(): void {
  if(this.myScrollContainer){
      this.previousScrollHeightMinusTop = 0;
      this.myScrollContainer.nativeElement.scrollTop = this.myScrollContainer.nativeElement.scrollHeight;
      this.cdref.detectChanges();
  }

}

prepareFor():void {
  this.myScrollContainer.nativeElement.scrollTop = !this.myScrollContainer.nativeElement.scrollTop // check for scrollTop is zero or not
    ? this.myScrollContainer.nativeElement.scrollTop + 10
    : this.myScrollContainer.nativeElement.scrollTop;
   this.previousScrollHeightMinusTop = this.myScrollContainer.nativeElement.scrollHeight - this.myScrollContainer.nativeElement.scrollTop;
    // the current position is stored before new messages are loaded
}

restore():void {
      this.myScrollContainer.nativeElement.scrollTop = this.myScrollContainer.nativeElement.scrollHeight - this.previousScrollHeightMinusTop;
        // restoring the scroll position to the one stored earlier

}
  @ViewChild('attachments') fileUpload!: ElementRef;
  uploadAttachments(event: any): void {
    this.fileSize = false;
    this.fileType = false;
    const files = event.target.files;
    const allowedTypes = [
      'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
      'application/msword',
      'application/vnd.ms-excel',
      'image/png',
      'application/pdf',
      'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
      'image/jpeg',
      'video/mp4',
      'image/gif',
      'application/vnd.ms-powerpoint',
      'audio/mpeg',
      'video/x-matroska',
      'video/3gpp'
    ];
    const formData = new FormData();
    this.ngxService.startBackground();
    for (const file of files) {
      const { size, type, name } = file;
      if (!allowedTypes.includes(type)) {
        this.fileType = true;
      } else if (size > 5020905) {
        this.fileSize = true;
      } else {
        formData.append('file', file, name);
      }
    }
    if (formData.has('file')) {
      this.chatService.uploadAttacments(formData).subscribe(
        (res: any) => {
          this.uploadedAttachments = res.attachmentList;
          this.attachmentIds = res.attachmentIds;
          this.fileUpload.nativeElement.value = null;
          this.ngxService.stopBackground();
        },
        (err) => {
          this.ngxService.stopBackground();
          this.toastr.error(err, 'Error');
        }
      );
    } else {
      this.ngxService.stopBackground();
    }
  }
  
  deletedoc(documentId: any): void {
    const data = {
      DocumentId: documentId,
      DocumentIds: this.attachmentIds
    };
    this.chatService.RemoveAttchmetns(data).subscribe(
      (result: any) => {
        this.uploadedAttachments = result.attachmentList;
        this.attachmentIds = result.attachmentIds;
        this.uploadedAttachments = this.uploadedAttachments.filter((x: any) => x.documentId !== documentId);
      },
      (error) => {
        console.log(error.error);
      }
    );
  }
openChatbot(){
  this.showChatBot = !this.showChatBot;
}
saveMessage(message:any){
message.messageContent = FrontEndEncryption(message.editableContent);
message.editableContent = '';
message.editMode = false;
}
showCancelEdit(message:any){
  this.prepareFor();
message.editMode = !message.editMode
message.editableContent =FrontEndDecryptionMessage(message.messageContent)
this.cdref.detectChanges();
setTimeout(() => {
  this.restore();
});
}
logout() {
  this.authService.logout();
  window.location.reload()
  this.router.navigate(['/login']);
}
}


