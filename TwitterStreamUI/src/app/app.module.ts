import { NgModule, APP_INITIALIZER, } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';

import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { SignalrService } from './services/signalr.service';
import { ApiDataService } from './services/api.service';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule
  ],
  providers: [
    ApiDataService,
    SignalrService,
    // {
    //   provide: APP_INITIALIZER,
    //   useFactory: (signalrService: SignalrService) => () => signalrService.initiateSignalrConnection(),
    //   deps: [SignalrService],
    //   multi: true,
    // },

  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
