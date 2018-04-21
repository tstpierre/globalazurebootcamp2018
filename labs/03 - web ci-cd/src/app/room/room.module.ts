import { NgModule } from '@angular/core';
import { CommonModule, JsonPipe, AsyncPipe } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';

import { RoomsContainerComponent } from './rooms-container/rooms-container.component';
import { RoomListComponent } from './room-list/room-list.component';
import { RoomDetailComponent } from './room-detail/room-detail.component';
import { RoomService } from './services/room.service';

import { MatGridListModule } from '@angular/material/grid-list';
import { MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule, MatIcon } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';

@NgModule({
    imports: [
        CommonModule,
        HttpClientModule,
        
        MatGridListModule,
        MatListModule,
        MatButtonModule,
        MatCardModule,
        MatIconModule,
        MatTooltipModule
    ],
    declarations: [
        RoomsContainerComponent,
        RoomListComponent,
        RoomDetailComponent
    ],
    exports: [
        RoomsContainerComponent
    ],
    providers: [RoomService]
})
export class RoomModule { }