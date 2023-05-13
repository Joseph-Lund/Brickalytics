export class UserRate {
  userId: number;
  productTypeId: number;
  productSubTypeId: number;
  rate: number;
  percent: number;

  constructor(
    userId: number,
    productTypeId: number,
    productSubTypeId: number,
    rate: number,
    percent: number,
  ) {
    this.userId = userId;
    this.productTypeId = productTypeId;
    this.productSubTypeId = productSubTypeId;
    this.rate = rate;
    this.percent = percent;
  }
}
