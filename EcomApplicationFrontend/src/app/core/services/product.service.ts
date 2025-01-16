import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  constructor(private http: HttpClient) {}

  addProduct(data: any): Observable<any> {
    return this.http.post(
      `${environment.apiBaseUrl}/api/Product/AddProduct`,
      data
    );
  }

  getProducts(): Observable<any> {
    return this.http.get(`${environment.apiBaseUrl}/api/Product`);
  }

  updateProducts(product: any): Observable<any> {
    return this.http.put(`${environment.apiBaseUrl}/api/Product`, product);
  }

  deleteProducts(PrId: any): Observable<any> {
    return this.http.delete(`${environment.apiBaseUrl}/api/Product/${PrId}`);
  }
}
