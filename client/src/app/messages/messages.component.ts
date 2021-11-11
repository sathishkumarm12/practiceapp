import { Component, OnInit } from '@angular/core';
import { Message } from '../models/message';
import { IPagination } from '../models/pagination';
import { MessageService } from '../services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages: Message[] = [];
  paginationData: IPagination;
  container = 'Unread';
  pageNumber = 1;
  pageSize = 5;
  loadingData = false;

  constructor(private messageService: MessageService) { }

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages() {
    this.loadingData = true;
    this.messageService.getMessages(this.pageNumber, this.pageSize, this.container).subscribe(
      response => {
        this.messages = response.result;
        this.paginationData = response.pagination;
        this.loadingData = false;
      }
    );
  }

  deleteMessage(id: number) {
    this.messageService.deleteMessage(id).subscribe(() => {
      this.messages.splice(this.messages.findIndex(m => m.id == id), 1);
    });
  }

  PageChanged(event: any) {
    if (this.pageNumber != event.page) {
      this.pageNumber = event.page;
      this.loadMessages();
    }
  }
}
