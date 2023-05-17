export class ProductsSoldChild {
  itemName: string;
  total: number;
  count: number;

  constructor(itemName: string, total: number, count: number) {
    this.itemName = itemName;
    this.total = total;
    this.count = count;
  }
}
