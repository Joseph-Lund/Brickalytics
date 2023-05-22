export class Dates {
  id: number | null;
  start: Date;
  end: Date;

  constructor(start: Date, end: Date, id: number | null = null ) {
    this.id = id;
    this.start = start;
    this.end = end;
  }
}
