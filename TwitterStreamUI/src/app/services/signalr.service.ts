import { Injectable, OnInit, OnDestroy } from '@angular/core';
import { environment } from '../../environments/environment';
import { BehaviorSubject, Subject, Observable } from 'rxjs';
import * as signalR from '@microsoft/signalr';
import { StreamDataModel } from "../models/streamdata.model";
import { map } from "rxjs/operators";
import { debug } from 'console';
import { SSL_OP_NETSCAPE_CA_DN_BUG } from 'constants';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {
  private hubConnection!: signalR.HubConnection;
  public hubMessages: StreamDataModel[];

  public messages: Subject<StreamDataModel> = new Subject();

  constructor() {
    this.hubMessages = new Array<StreamDataModel>();
  }

  public init() {
    this.hubConnection = new signalR.HubConnectionBuilder()
                              .configureLogging(signalR.LogLevel.Information)
                              .withUrl(environment.signalrHubUrl) // the SignalR server url as set in the .NET Project properties and Startup class
                              .build();

    this.hubConnection
      .start()
      .then(() => {
        debugger;
        console.log('Connection started');
      })
      .catch(err => { 
        debugger;
        console.log('Error while starting connection: ' + err);
      });

      this.hubConnection.on("SendMQMessage", (data: any) => {
        debugger;
        this.messages.next(data);
        //console.log(JSON.stringify(this.messages));
      });
  }

  receieve(message: StreamDataModel): StreamDataModel[] {
    // read in from local strorage
    const messages = this.load();
    messages.unshift(message);
    localStorage.setItem("messages", JSON.stringify(messages));
    return messages;
  }

  load(): StreamDataModel[] {
    const messagesLocal = localStorage.getItem("messages");
    let messagesResponse = [];
    if (messagesLocal !== null) {
      messagesResponse = JSON.parse(messagesLocal);
    }
    return messagesResponse;
  }
}