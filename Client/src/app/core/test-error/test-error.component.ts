import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-test-error',
  templateUrl: './test-error.component.html',
  styleUrls: ['./test-error.component.scss']
})
export class TestErrorComponent {
  baseUrl = environment.apiUrl;
  validationErrors: string[] = [];
  constructor(private http: HttpClient) {
  }

  getError404(){
    this.http.get(this.baseUrl + 'product/100').subscribe({
      next: response => console.log(response),
      error: error => console.log(error)
    })
  }

  getError500(){
    this.http.get(this.baseUrl + 'buggy/servererror').subscribe({
      next: response => console.log(response),
      error: error => console.log(error)
    })
  }

  getError400(){
    this.http.get(this.baseUrl + 'buggy/badrequest').subscribe({
      next: response => console.log(response),
      error: error => console.log(error)
    })
  }

  getValidationError400(){
    this.http.get(this.baseUrl + 'products/fortytwo').subscribe({
      next: response => console.log(response),
      error: error => {
        console.log(error);
        this.validationErrors = error.errors;
      }
    })
  }
}
