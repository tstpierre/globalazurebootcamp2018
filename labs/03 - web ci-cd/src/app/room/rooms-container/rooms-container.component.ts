import { Component, OnInit, OnDestroy } from '@angular/core';
import { TimerObservable } from 'rxjs/observable/TimerObservable';
import 'rxjs/add/operator/takeWhile';
import { JsonPipe } from '@angular/common';

import { Room, RoomDetail } from '../models/room';
import { RoomService } from '../services/room.service';
import { Observable } from 'rxjs/Observable';

import { environment } from '../../../environments/environment';

@Component({
    selector: 'rooms-container',
    templateUrl: './rooms-container.component.html',
    styleUrls: ['./rooms-container.component.scss']
})
export class RoomsContainerComponent implements OnInit, OnDestroy {

    private _alive: boolean;
    private _interval: number;

    rooms: Room[];
    selectedRoomDetail: RoomDetail;

    constructor(private roomService: RoomService) {
        this._alive = true;
        this._interval = environment.queryInterval;
    }

    ngOnInit() {
        this.getRooms();
    }

    ngOnDestroy() {
        this._alive = false;
    }

    getRooms(): void {

        TimerObservable.create(0, this._interval)
            .takeWhile(() => this._alive)
            .subscribe(() => {
                this.roomService
                    .getRooms()
                    .subscribe(rooms => {
                        this.rooms = rooms;

                        if(this.selectedRoomDetail) {
                            this.selectRoom(this.selectedRoomDetail.id);
                        }
                    });
            });
    }

    selectRoom(roomId: string): void {

        this.roomService.getRoom(roomId)
            .subscribe(roomDetail => {
                this.selectedRoomDetail = roomDetail;

                let r: Room = this.rooms.find(r => r.id === roomId);
                let idx: number = this.rooms.indexOf(r);

                this.rooms[idx].state = roomDetail.state;
            });
    }

    onRoomSelected(room: Room): void {
        this.selectRoom(room.id);
    }
}