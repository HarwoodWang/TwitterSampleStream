import { Injectable, OnInit, OnDestroy } from '@angular/core';
import { environment } from '../../environments/environment';
import { BehaviorSubject, Subject, Observable } from 'rxjs';
import * as signalR from '@microsoft/signalr';
import { StreamDataModel } from "../models/streamdata.model";
import { map } from "rxjs/operators";
import { debug } from 'console';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {
  private hubConnection!: signalR.HubConnection;
  public hubMessages: StreamDataModel[];

  public messages: Subject<string> = new Subject();

  constructor() {
    this.hubMessages = new Array<StreamDataModel>();
  //}


  //public ngOnInit() {

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
        this.hubConnection.invoke('ReceiveMQMessage', data).catch(err => console.log(err));


        //console.log(JSON.stringify(this.messages));
      });
    }
  }