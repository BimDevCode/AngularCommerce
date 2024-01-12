import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Pagination } from '../shared/models/pagination';
import { Product } from '../shared/models/product';
import { Type } from '../shared/models/type';
import { ShopParameter } from '../shared/models/shopParameters';
import { Brand } from '../shared/models/brand';

@Injectable({
  providedIn: 'root'
})
export class ShopService {
  baseUrl = 'https://localhost:7275/api/'
  constructor(private http: HttpClient) { }

  getProducts(shopParameters: ShopParameter){
    let params = new HttpParams();
    if(shopParameters.brandId > 0) params = params.append('brandId', shopParameters.brandId);
    if(shopParameters.typeId > 0) params = params.append('typeId', shopParameters.typeId);
    params = params.append('sort', shopParameters.sort);
    params = params.append('pageIndex', shopParameters.pageNumber);
    params = params.append('pageSize', shopParameters.pageSize);
    if(shopParameters.search) params = params.append('search', shopParameters.search);  
    return this.http.get<Pagination<Product[]>>(this.baseUrl + 'products', {params: params});
  }

  getProduct(id: number){
    return this.http.get<Product>(this.baseUrl + 'products/' + id);
  }
  getBrands(){
    return this.http.get<Brand[]>(this.baseUrl + 'products/brands');
  }

  getTypes(){
    return this.http.get<Type[]>(this.baseUrl + 'products/types');
  }
}
