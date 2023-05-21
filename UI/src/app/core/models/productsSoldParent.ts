import { ProductsSoldChild } from "./productsSoldChild";

export class ProductsSoldParent {
  productsSoldProfit: number;
  productsSoldTotal: number;
  items: ProductsSoldChild[];

  constructor(productsSoldProfit: number, productsSoldTotal: number, items: ProductsSoldChild[] = []) {
    this.productsSoldProfit = productsSoldProfit;
    this.productsSoldTotal = productsSoldTotal;
    this.items = items;
  }
}
