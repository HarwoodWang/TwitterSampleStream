// export interface StreamDataModel {
//     starttime: Date;
//     endtime: Date;
//     totalcount: number;
//     averageperminute: number;
//     totalminutes: number;
// }

export class ApiDataModel {
    constructor
    (
        public startTime: Date,
        public endTime: Date,
        public totalCount: number,
        public averagePerMinute: number,
        public totalMinutes: number
    ) {}
}

export class SignalRDataModel {
    constructor
    (
        public StartTime: Date,
        public EndTime: Date,
        public TotalCount: number,
        public AveragePerMinute: number,
        public TotalMinutes: number
    ) {}
}

export class SummaryModel {
    constructor
    (
        public averageMinute: number,
        public totalCount: number,
        public totalMinutes: number
    ) {}
}