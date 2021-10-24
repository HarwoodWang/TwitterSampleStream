import { Injectable, OnInit, OnDestroy } from '@angular/core';
import { environment } from '../../environments/environment';
import { BehaviorSubject } from 'rxjs';
import * as signalR from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class SignalrService implements OnInit, OnDestroy {
  connection!: signalR.HubConnection;
  hubMessage: BehaviorSubject<string>;

  constructor() {
    this.hubMessage = new BehaviorSubject<string>(null!);
  }

  ngOnInit(): void {
    this.initiateSignalrConnection();
  }

  ngOnDestroy(): void {
    this.connection.stop();
  }

  // Establish a connection to the SignalR server hub
  private initiateSignalrConnection(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.connection = new signalR.HubConnectionBuilder()
        .configureLogging(signalR.LogLevel.Information)
        .withUrl(environment.signalrHubUrl) // the SignalR server url as set in the .NET Project properties and Startup class
        .build();

      this.connection
        .start()
        .then(() => {
          console.log(
            `SignalR connection success! connectionId: ${this.connection.connectionId} `
          );
          resolve();
        })
        .catch((error) => {
          console.log(`SignalR connection error: ${error}`);
          reject();
        });

        // This method will implement the methods defined in the ISignalrDemoHub interface in the SignalrDemo.Server .NET solution
        this.connection.on('SendMessage', (message: string) => {
          this.hubMessage.next(message);
        });
      });
    }
  }

