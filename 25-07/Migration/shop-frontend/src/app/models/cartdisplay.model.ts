export class CartDisplayModel {
    public productName : string = "";
    public price : number = 0;
    public categoryId : number | null = null;
    public colorId : number | null = null;


    constructor(public productId : number, public quantity : number){
        
    }
}