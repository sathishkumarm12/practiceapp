import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Message } from '../models/message';
import { PaginatedResult } from '../models/pagination';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  baseUrl = environment.apiBaseUrl;
  paginatedResult: PaginatedResult<Message[]> = new PaginatedResult<Message[]>();

  constructor(private http: HttpClient) { }

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

  sendMessage(username: string, content: string) {
    return this.http.post<Message>(this.baseUrl + "messages", {
      recipientUsername: username, content: content
    });
  }

  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + "messages/" + id);
  }
}
