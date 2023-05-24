export class Payment {
  id: number | null;
  userId: number | null;
  paymentDate: Date;
  paymentAmount: number;

  constructor(id: number | null, userId: number | null, paymentDate: Date, paymentAmount: number) {
    this.id = id;
    this.userId = userId;
    this.paymentDate = paymentDate;
    this.paymentAmount = paymentAmount;
  }
}
