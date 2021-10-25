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
    return this.http.get<ApiDataModel[]>(this.accessPointUrl)
              .pipe(map((response: ApiDataModel[]) => response));
  }

  private handleError(error: any): Promise<any> {
    console.error('An error occurred', error);
    return Promise.reject(error.message || error);
  }
}