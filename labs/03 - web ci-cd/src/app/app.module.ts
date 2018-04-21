import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CommonModule } from '@angular/common';

import { MatToolbarModule, MatButtonModule } from '@angular/material';

import { RoomModule } from './room/room.module';

import { RoomsContainerComponent } from './room/rooms-container/rooms-container.component';
import { AppComponent } from './app.component';
import { RoomService } from './room/services/room.service';

@NgModule({
    declarations: [
        AppComponent
    ],
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        CommonModule,

        MatToolbarModule,
        MatButtonModule,

        RoomModule
    ],
    providers: [],
    bootstrap: [AppComponent]
})
export class AppModule { }