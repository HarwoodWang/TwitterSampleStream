import { Component, OnInit } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { ApiDataModel, SignalRDataModel, SummaryModel } from "../models/streamdata.model";

import { ApiDataService } from "../services/api.service";
import { SignalrService } from "../services/signalr.service"

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  private apiData: ApiDataModel[] = new Array<ApiDataModel>();
  private signalrData: SignalRDataModel[] = new Array<SignalRDataModel>();

  constructor(private apidatasource: ApiDataService, private signalRService: SignalrService ) {
    this.apidatasource.getData().subscribe(data => this.apiData = data);
   }

   public get signalritems(): SignalRDataModel[] {
    return this.signalRService.hubMessages;
  }

  ngOnInit(): void {
    this.signalRService.init();
    this.signalRService.messages.subscribe((message: SignalRDataModel) => this.signalrData = this.signalRService.receieve(message));
  }

  public signalrItems(): SignalRDataModel[] {
    if(this.signalrData) {
      return this.signalrData.map(x => JSON.parse(x.toString()));
    }
    return [];
  }

  public apiItems() : ApiDataModel[] {
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

  public signalrSummary(): SummaryModel {
    let totalCount = this.signalrData.reduce((sum, current) => sum + current.TotalCount, 0);
    let totalMinutes = this.signalrData.reduce((sum, current) => sum + current.TotalMinutes, 0);

    let averageMinute = totalCount / totalMinutes;

    return new SummaryModel(totalCount = totalCount,
                    totalMinutes = totalMinutes,
                    averageMinute = averageMinute);
  }
}


