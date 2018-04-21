import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';

import { Room, RoomState } from '../models/room';

@Component({
    selector: 'room-list',
    templateUrl: './room-list.component.html',
    styleUrls: ['./room-list.component.scss']
})
export class RoomListComponent implements OnInit {

    @Input() rooms: Room[];
    @Output() selectedEvent = new EventEmitter<Room>();

    constructor() { }

    ngOnInit() { }

    onRoomSelected(room: Room): void {
        this.selectedEvent.emit(room);
    }
}