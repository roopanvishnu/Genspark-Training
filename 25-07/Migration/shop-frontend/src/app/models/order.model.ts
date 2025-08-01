import { OrderDetailModel } from "./orderdetail.model";

export class OrderModel {
    constructor (
        public customerAddress: string = "",
        public customerEmail : string = "",
        public customerName : string = "",
        public customerPhone : string = "",
        public orderDate :  Date | null = null,
        public orderID : number = 0,
        public paymentType : string ="",
        public status : string ="",
        public orderDetails : OrderDetailModel[] = [],
    ){}
}