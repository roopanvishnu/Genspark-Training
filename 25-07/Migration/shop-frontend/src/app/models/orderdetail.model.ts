import { OrderModel } from "./order.model";
import { ProductModel } from "./product.model";

export class OrderDetailModel {
    constructor(
        public orderDetailID : number = 0,
        public orderID : number = 0,
        public productID : number = 0,
        public price : number = 0,
        public quantity : number = 0,
    
        public order : OrderModel | null = null,
        public product : ProductModel | undefined| null = null
    ){}
}