import { GenericType } from "./genericType";

export class ProductSubType extends GenericType {
  productTypeId: number;

  constructor(
    id: number,
    name: string,
    productTypeId: number
  ) {
    super(id, name);
    this.productTypeId = productTypeId;
  }
}
