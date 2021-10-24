import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { StreamDataModel, SummaryModel } from "../models/streamdata.model";

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

   public get signalritems(): any {
    return this.signalRService.hubMessage;
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
