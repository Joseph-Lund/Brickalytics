import * as moment from "moment";

export class Payment {
  id: number | null;
  userId: number | null;
  paymentDate: Date;
  formattedDate: string | null = null;
  formattedTime: string | null = null;
  paymentAmount: number;

  constructor(id: number | null, userId: number | null, paymentDate: Date, paymentAmount: number) {
    var formatedDate = moment(paymentDate).format('MM/DD/YYYY');
    var formatedTime = moment(paymentDate).format('h:mm A');
    this.id = id;
    this.userId = userId;
    this.paymentDate = paymentDate;
    this.formattedDate = formatedDate;
    this.formattedTime = formatedTime;
    this.paymentAmount = paymentAmount;
  }
}
