import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Basket, BasketItem, BasketTotals } from '../shared/models/basket';
import { DeliveryMethod } from '../shared/models/deliveryMethod';
import { Product } from '../shared/models/product';


@Injectable({
  providedIn: 'root'
})
export class BasketService {
  baseUrl = environment.apiUrl;
  private basketSource = new BehaviorSubject<Basket | null>(null);
  basketSource$ = this.basketSource.asObservable();
  private basketTotalsSource = new BehaviorSubject<BasketTotals | null>(null);
  basketTotalsSource$ = this.basketTotalsSource.asObservable();

  constructor(private httpClient : HttpClient ) { }
  
  createPaymentIntent() {
    return this.httpClient.post<Basket>(this.baseUrl + 'payments/' + this.getBasketValue()?.id, {})
      .pipe(
        map(basket => {
          this.basketSource.next(basket);
        })
      )
  }
  setShippingPrice(deliveryMethod: DeliveryMethod) {
    const basket = this.getBasketValue();
    if (basket) {
      basket.shippingPrice = deliveryMethod.price;
      basket.deliveryMethodId = deliveryMethod.id;
      this.setBasket(basket);
    }
  }

  getBasket(id: string) {
    return this.httpClient.get<Basket>(this.baseUrl + 'basket?id=' + id).subscribe({
      next: basket => {
        this.basketSource.next(basket);
        this.calculateTotals();
      },
      error: error => console.log(error)
    })
  }

  setBasket(basket: Basket) {
    return this.httpClient.post<Basket>(this.baseUrl + 'basket', basket).subscribe({
      next: basket => {
        this.basketSource.next(basket);
        this.calculateTotals();
      },
      error: error => console.log(error)
    })
  }

  getBasketValue() {
    return this.basketSource.value;
  }

  addItemToBasket(item: Product | BasketItem, quantity = 1) {
    if(this.isProduct(item)) item = this.mapProductItemToBasketItem(item);
    const basket = this.getBasketValue() ?? this.createBasket(); 
    basket.items = this.addOrUpdateItem(basket.items, item, quantity);
    this.setBasket(basket);
  }

  removeItemFromBasket(id: number, quantity = 1) {
    const basket = this.getBasketValue();
    if(!basket) return;
    const item = basket.items.find(i => i.id === id);
    if(item) {
      item.quantity -= quantity;
      if(item.quantity === 0) {
        basket.items = basket.items.filter(x => x.id !== id);
      }
      if(basket.items.length > 0) this.setBasket(basket);
      else this.deleteBasket(basket);
    }
  }
  
  deleteBasket(basket: Basket) {
    return this.httpClient.delete(this.baseUrl + 'basket?id=' + basket.id).subscribe({
      next: () => {
        this.basketSource.next(null);
        this.basketTotalsSource.next(null);
        localStorage.removeItem('basket_id');
      },
      error: error => console.log(error)
    })
  }

  addOrUpdateItem(items: BasketItem[], itemToAdd: BasketItem, quantity: number): BasketItem[] {
    const item = items.find(x => x.id === itemToAdd.id);
    if(item) item.quantity += quantity;
    else {
      itemToAdd.quantity = quantity;
      items.push(itemToAdd);
    }
    return items;
  }

  createBasket(): Basket{
    const basket = new Basket();
    localStorage.setItem('basket_id', basket.id);
    return basket;
  }

  private mapProductItemToBasketItem(item: Product): BasketItem {
    return {
      id: item.id,
      productName: item.name,
      price: item.price,
      quantity :  0,
      pictureUrl: item.pictureUrl,
      brand: item.productBrand,
      type: item.productType
    }
  }

  private calculateTotals() {
    const basket = this.getBasketValue();
    if (!basket || basket.items === undefined) return;
    const shipping = 0;
    const subtotal = basket.items.reduce((a, b) => (b.price * b.quantity) + a, 0);
    const total = subtotal + shipping;
    this.basketTotalsSource.next({shipping, total, subtotal});
  }

  private isProduct(item: Product | BasketItem): item is Product {
    return (item as Product).productBrand !== undefined;
  }
}
