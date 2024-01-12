import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Product } from '../shared/models/product';
import { ShopService } from './shop.service';
import { Type } from '../shared/models/type';
import { Brand } from '../shared/models/brand';
import { ShopParameter } from '../shared/models/shopParameters';

@Component({
  selector: 'app-shop',
  templateUrl: './shop.component.html',
  styleUrls: ['./shop.component.scss']
})

export class ShopComponent implements OnInit {
  @ViewChild('search') searchTerm?: ElementRef;
  products: Product[] = [];
  types: Type[] = [];
  brands: Brand[] = [];
  shopParameter = new ShopParameter();
  sortOptions = [
    {name: 'Alphabetic', value: 'name'},
    {name: 'Price: Low to High', value: 'priceAsc'},
    {name: 'Price: High to Low', value: 'priceDesc'}
  ];
  totalCount = 0;

  constructor(private shopService: ShopService) {
  }
  ngOnInit(): void {
    this.getProducts();
    this.getBrands();
    this.getTypes();
  }
  getProducts(){
    this.shopService.getProducts(this.shopParameter).subscribe({
      next: response => {
        this.products = response.data,
        this.shopParameter.pageNumber = response.pageIndex,
        this.shopParameter.pageSize = response.pageSize,
        this.totalCount = response.count
      },
      error: error => console.log(error)
    })
  }
  getBrands(){
    this.shopService.getBrands().subscribe({
      next: response => this.brands = [{id: 0, name: 'All'}, ...response],
      error: error => console.log(error)
    })
  }
  getTypes(){
    this.shopService.getTypes().subscribe({
      next: response => this.types = [{id: 0, name: 'All'}, ...response],
      error: error => console.log(error)
    })
  }

  onBrandSelected(brandId: number){
    this.shopParameter.brandId = brandId;
    this.shopParameter.pageNumber =1;
    this.getProducts();
  }

  onTypeSelected(typeId: number){
    this.shopParameter.typeId = typeId;
    this.shopParameter.pageNumber =1;
    this.getProducts();
  }

  onSortSelected(event: any){
    this.shopParameter.sort = event.target.value;
    this.getProducts();
  }

  onPageChanged(event: any){
    if(this.shopParameter.pageNumber !== event){
      this.shopParameter.pageNumber = event;
      this.getProducts();
    }
  }

  onSearch(){
    this.shopParameter.search = this.searchTerm?.nativeElement.value;
    this.shopParameter.pageNumber =1;
    this.getProducts();
  }

  onReset(){
    if(this.searchTerm) this.searchTerm.nativeElement.value = '';
    this.shopParameter = new ShopParameter();
    this.getProducts(); 
  }
}
