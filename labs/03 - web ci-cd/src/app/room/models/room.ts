export enum RoomState {
    Unknown = 0,
    Occupied = 1,
    Unoccupied = 2
}

export class Room {
    id: string;
    displayName: string;
    state: RoomState;
}

export class RoomDetail extends Room {
    usageHistory: RoomUsage[];
    statistics: any;
}

export class RoomUsage {
    state: RoomState;
    startTime: Date;
    endTime: Date;
}

export class RoomStatistics {
    averageOccupationTimeInMinutes: number;
    busyTimes: RoomBusyTime[];
}

export class RoomBusyTime {
    dayOfWeek: number;
}