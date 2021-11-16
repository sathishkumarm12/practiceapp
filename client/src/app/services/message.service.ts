import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Message } from '../models/message';
import { PaginatedResult } from '../models/pagination';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  baseUrl = environment.apiBaseUrl;
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();

  paginatedResult: PaginatedResult<Message[]> = new PaginatedResult<Message[]>();

  constructor(private http: HttpClient) { }

  createHubConnection(user: User, otherUserName: string) {
    this.hubConnection = new HubConnectionBuilder().withUrl(this.hubUrl + 'message?user=' + otherUserName, {
      accessTokenFactory: () => user.token
    })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch(error => console.log(error));

    this.hubConnection.on('ReceiveMessageThread', (message: Message[]) => {
      this.messageThreadSource.next(message);
    });

    this.hubConnection.on('NewMessage', (message: Message) => {
      this.messageThread$.pipe(take(1)).subscribe(messagedata => {
        this.messageThreadSource.next([...messagedata, message]);
      });
    });
  }

  stopHubConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop().catch(error => console.log(error));
    }
  }

  getMessages(pageNumber, pageSize, container) {

    let params = new HttpParams();

    params = params.append("pageNumber", pageNumber);
    params = params.append("pageSize", pageSize);
    params = params.append("container", container);


    return this.http.get<Message[]>(this.baseUrl + 'messages', { observe: 'response', params })
      .pipe(
        map(response => {
          this.paginatedResult.result = response.body;
          if (response.headers.has("Pagination")) {
            this.paginatedResult.pagination = JSON.parse(response.headers.get("Pagination"));
          }
          return this.paginatedResult;
        })
      );
  }

  getMessageThread(username) {
    return this.http.get<Message[]>(this.baseUrl + 'messages/thread/' + username);
  }

  async sendMessage(username: string, content: string) {
    return this.hubConnection.invoke("CreateMessage", {
      recipientUsername: username, content: content
    }).catch(error => console.log(error));
  }

  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + "messages/" + id);
  }
}
