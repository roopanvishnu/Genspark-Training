export class Product {
  id: number;
  name: string;
  price: number;

  constructor(id: number = 101, name: string = "Abibas Shoe", price: number = 99.99) {
    this.id = id;
    this.name = name;
    this.price = price;
  }
}
