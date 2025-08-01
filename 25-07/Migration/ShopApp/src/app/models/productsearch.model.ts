export class ProductSearchModel {
    constructor(
        public page : Number =1,
        public category : Number | null = null
    ) {}
}