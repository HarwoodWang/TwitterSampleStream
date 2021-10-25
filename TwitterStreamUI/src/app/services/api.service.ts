import { Injectable,  Inject, InjectionToken } from "@angular/core";
import { HttpClient, HttpClientModule } from "@angular/common/http";
import { Observable, interval } from "rxjs";
import { ApiDataModel } from "../models/streamdata.model";
import { environment } from '../../environments/environment';
import { map } from "rxjs/operators";


@Injectable({
  providedIn: 'root'
})
export class ApiDataService  {
  private accessPointUrl: string = environment.webapiUrl;

  constructor(private http: HttpClient) { }

  public getData(): Observable<ApiDataModel[]> {
    var urlGet = this.http.get<ApiDataModel[]>(this.accessPointUrl);
    var apidata = new Observable<ApiDataModel[]>();

    try {
      apidata = urlGet.pipe(map((response: ApiDataModel[]) => response));
    }
    catch(e) {
      alert(e);
    }

    return apidata;
  }
}