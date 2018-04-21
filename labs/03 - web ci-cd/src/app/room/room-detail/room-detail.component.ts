import { Component, Input, OnInit } from '@angular/core';

import { RoomDetail, RoomState } from '../models/room';

@Component({
    selector: 'room-detail',
    templateUrl: './room-detail.component.html',
    styleUrls: ['./room-detail.component.scss']
})
export class RoomDetailComponent implements OnInit {

    @Input() roomDetail: RoomDetail;

    constructor() { }

    ngOnInit() { }
}