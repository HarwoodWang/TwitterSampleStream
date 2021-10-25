import { Component, OnInit } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { StreamDataModel, SummaryModel } from "../models/streamdata.model";

import { ApiDataService } from "../services/api.service";
import { SignalrService } from "../services/signalr.service"

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  private apiData: StreamDataModel[] = new Array<StreamDataModel>();
  private signalrData: StreamDataModel[] = new Array<StreamDataModel>();

  constructor(private apidatasource: ApiDataService, private signalRService: SignalrService ) {
    this.apidatasource.getData().subscribe(data => this.apiData = data);
   }

   public get signalritems(): StreamDataModel[] {
    return this.signalRService.hubMessages;
  }

  ngOnInit(): void {
    this.signalRService.init();

    this.signalRService.messages.subscribe(message => {
      this.signalrData = this.signalRService.receieve(message);
    });
  }

  public signalrItems(): StreamDataModel[] {
    return this.signalrData;
  }

  public apiItems() : StreamDataModel[] {
    return this.apiData;
  }

  public apiSummary(): SummaryModel {
    let totalCount = this.apiData.reduce((sum, current) => sum + current.totalCount, 0);
    let totalMinutes = this.apiData.reduce((sum, current) => sum + current.totalMinutes, 0);

    let averageMinute = totalCount / totalMinutes;

    return new SummaryModel(totalCount = totalCount,
                    totalMinutes = totalMinutes,
                    averageMinute = averageMinute);
  }
}
