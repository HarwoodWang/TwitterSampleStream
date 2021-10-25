import { Injectable, OnInit, OnDestroy } from '@angular/core';
import { environment } from '../../environments/environment';
import { BehaviorSubject, Subject, Observable } from 'rxjs';
import * as signalR from '@microsoft/signalr';
import { SignalRDataModel } from "../models/streamdata.model";
import { map } from "rxjs/operators";
import { debug } from 'console';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {
  private hubConnection!: signalR.HubConnection;
  public hubMessages: SignalRDataModel[];

  public messages: Subject<SignalRDataModel> = new Subject();

  constructor() {
    this.hubMessages = new Array<SignalRDataModel>();
  }

  public init() {
    this.hubConnection = new signalR.HubConnectionBuilder()
                              .configureLogging(signalR.LogLevel.Information)
                              .withUrl(environment.signalrHubUrl) // the SignalR server url as set in the .NET Project properties and Startup class
                              .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('Connection started ' + this.hubConnection.connectionId);
      })
      .catch(err => { 
        console.log('Error while starting connection: ' + err);
      });

    this.hubConnection.on("ReceiveMQMessage", (data: any) => {
      //debugger;
      this.messages.next(data);
      //console.log(JSON.stringify(this.messages));
    });
  }

  receieve(message: SignalRDataModel): SignalRDataModel[] {
    // read in from local strorage
    //debugger;
    const messages = this.load();
    messages.unshift(message);
    localStorage.setItem("messages", JSON.stringify(messages));
    //localStorage.clear();
    return messages;
  }

  load(): SignalRDataModel[] {
    const messagesLocal = localStorage.getItem("messages");
    let messagesResponse = [];
    let messagesResponseJson: SignalRDataModel[] = [];

    if (messagesLocal !== null) {
      messagesResponse = JSON.parse(messagesLocal);
      //messagesResponse = messagesResponse[messagesResponse.length - 1];

      for(var i = 0; i < messagesResponse.length; i++)
      {       
        var strJson = JSON.stringify(messagesResponse[i]);

        if ( strJson !== 'undefined' && strJson !== null ) {  
          var objJson: SignalRDataModel = JSON.parse(strJson);
          messagesResponseJson.push(objJson);
        }
      }
    }
    return messagesResponseJson;
  }
}