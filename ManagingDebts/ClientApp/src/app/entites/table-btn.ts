export interface TableBtn {
    text: string;
    type: string;
    condition?: any; // example of usage :condition: (row: any) => row.isSent
}
