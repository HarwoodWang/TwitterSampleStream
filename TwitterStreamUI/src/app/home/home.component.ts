import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { StreamDataModel } from "../models/streamdata.model";

import { ApiDataService } from "../services/api.service";
import { SignalrService } from "../services/signalr.service"

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {

  private apiData: StreamDataModel[] = new Array<StreamDataModel>();;

  constructor(private apidatasource: ApiDataService, private signalRService: SignalrService ) {
    this.apidatasource.getData().subscribe(data => this.apiData = data);
   }

  public apiItems() : StreamDataModel[] {
    console.log(this.apiData);
    return this.apiData;
  }


  get signalritems(): any {
    return this.signalRService.hubMessage;
  }
}
