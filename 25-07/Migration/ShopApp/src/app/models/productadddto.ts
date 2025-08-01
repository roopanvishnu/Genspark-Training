import { ProductModel } from "./product.model";

export class ProductAddDTO {
    constructor(
        public productName: string = "",
        public image: string = "",
        public price: number | null = null,
        public userId: number| null = null,
        public categoryId: number | null = null,
        public colorId: number | null = null,
        public modelId: number | null = null,
        public storageId: number | null = null,
        public sellStartDate: Date | null = null,
        public sellEndDate: Date | null = null,
        public isNew: number | null = null
    ) {}

    static fromModel(product : ProductModel) {
        return new ProductAddDTO(
            product.productName,
            product.image,
            product.price,
            product.userId,
            product.categoryId,
            product.colorId,
            product.modelId,
            product.storageId,
            product.sellStartDate,
            product.sellEndDate,
            product.isNew
        );
    }
}