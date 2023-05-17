import { ProductsSoldChild } from "./productsSoldChild";

export class ProductsSoldParent {
  productsSoldTotal: number;
  items: ProductsSoldChild[];

  constructor(productsSoldTotal: number, items: ProductsSoldChild[] = []) {
    this.productsSoldTotal = productsSoldTotal;
    this.items = items;
  }
}
