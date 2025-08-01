export class ProductModel {
    constructor(
        public productId : number  = 0,
        public productName : string = "",
        public image : string = "",
        public price : number | null = null,
        public userId : number | null| undefined = null,
        public categoryId : number | null = null,
        public colorId : number | null = null,
        public modelId : number | null = null,
        public storageId : number | null = null,
        public sellStartDate : Date | null = null,
        public sellEndDate : Date | null = null,
        public isNew : number | null = null
    ){}
    
}