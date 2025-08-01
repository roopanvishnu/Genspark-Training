import { Component } from '@angular/core';
import { OrderService } from '../services/order.service';
import { OrderDetailService } from '../services/orderdetail.service';
import { ProductService } from '../services/product.service';
import { OrderModel } from '../models/order.model';
import { ProductModel } from '../models/product.model';
import { MatCardModule } from '@angular/material/card';
import { DatePipe } from '@angular/common';
import { MatTableModule } from '@angular/material/table';

@Component({
  selector: 'app-orders',
  imports: [
    MatCardModule,
    MatTableModule,
    DatePipe
  ],
  templateUrl: './orders.html',
  styleUrl: './orders.css'
})
export class Orders {

  orders : OrderModel[] =[];
  products : ProductModel[] = [];
  displayedColumns = ["productName", "price","quantity","total"]

  constructor(
    private orderService : OrderService,
    private productService : ProductService
  ){

    this.loadData();
  }
  async loadData(){
    const ordersPromise = this.orderService.getAll().toPromise();
    const productsPromise = this.productService.getAll().toPromise();

    const [ordersData, productsData] = await Promise.all([ordersPromise, productsPromise]);

    this.orders = ordersData as OrderModel[];
    this.products = productsData as ProductModel[];
    
    this.orders.forEach(o => {
      o.orderDetails?.forEach(od => {
        od.product = this.products.find(p => p.productId == od.productID);
      });
    });
    this.orders.sort(o => o.orderID).reverse();
    console.log(this.orders);


  }
}