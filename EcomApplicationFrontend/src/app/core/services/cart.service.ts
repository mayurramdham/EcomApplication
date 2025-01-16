import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { jwtDecode } from 'jwt-decode';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  constructor(private http: HttpClient) {
    this.updateCartItemCount();
    this.setItemCountSubject();
  }

  addToCart(body: any): Observable<any> {
    return this.http.post(`${environment.apiBaseUrl}/api/Cart/AddToCart`, body);
  }

  getProductFromCart(prId: any): Observable<any> {
    return this.http.get(`${environment.apiBaseUrl}/api/Cart/${prId}`);
  }

  RemoveItemFromCart(cartId: any): Observable<any> {
    return this.http.delete(`${environment.apiBaseUrl}/api/Cart/${cartId}`);
  }

  IncrementQuantity(quantity: any): Observable<any> {
    return this.http.post(
      `${environment.apiBaseUrl}/api/Cart/incrementcart`,
      quantity
    );
  }

  DecrementQuantity(quantity: any): Observable<any> {
    return this.http.post(
      `${environment.apiBaseUrl}/api/Cart/decrementcart`,
      quantity
    );
  }

  getUserById(userId: any): Observable<any> {
    return this.http.get(
      `${environment.apiBaseUrl}/api/User/getUserById/${userId}`
    );
  }

  getCartProductCount$(userId: number): Observable<any> {
    return this.http.get(
      `${environment.apiBaseUrl}/api/Cart/getCartCount/${userId}`
    );
  }

  addPayment(payment: any): Observable<any> {
    return this.http.post(
      `${environment.apiBaseUrl}/api/Cart/AddPayment`,
      payment
    );
  }

  addPaymentWithStripe(payment: any): Observable<any> {
    return this.http.post(
      `${environment.apiBaseUrl}/api/Cart/paymentWithStripe`,
      payment
    );
  }

  GenerateInvoice(invoiceId: number): Observable<any> {
    return this.http.get(
      `${environment.apiBaseUrl}/api/Cart/generateInvoic/${invoiceId}`
    );
  }

  // Subject behavior and other methods remain unchanged
  private cartItemCountSubject = new BehaviorSubject<number>(0);
  public cartItem$ = new BehaviorSubject<number[]>([]);

  setItemCountSubject() {
    const accessToken = localStorage.getItem('accessToken');
    if (accessToken) {
      const decodedToken: any = jwtDecode(accessToken);
      const userId = decodedToken.UserId;
      this.getCartProductCount$(userId).subscribe(
        (response) => {
          if (response.status == 200) {
            this.cartItem$.next(response.cartProductId);
          } else {
            this.cartItem$.next([]);
          }
        },
        (error) => {
          this.cartItem$.next([]);
        }
      );
    } else {
      this.cartItem$.next([]);
    }
  }

  resetSetCount() {
    this.setItemCountSubject();
  }

  cartItemCount$ = this.cartItemCountSubject.asObservable();

  updateCartItemCount(): void {
    const storedCart = localStorage.getItem('cart');
    const cartArray = storedCart ? JSON.parse(storedCart) : [];
    this.cartItemCountSubject.next(cartArray.length);
  }

  addItemToCart(item: any): void {
    const storedCart = localStorage.getItem('cart');
    let cartArray = storedCart ? JSON.parse(storedCart) : [];
    cartArray.push(item);
    localStorage.setItem('cart', JSON.stringify(cartArray));
    this.updateCartItemCount();
  }

  removeItemFromCart(itemToRemove: any): void {
    const storedCart = localStorage.getItem('cart');
    if (storedCart) {
      let cartArray = JSON.parse(storedCart);
      cartArray = cartArray.filter(
        (item: any) => item.cartId !== itemToRemove.cartId
      );
      localStorage.setItem('cart', JSON.stringify(cartArray));
      this.updateCartItemCount();
    }
  }

  clearCart(): void {
    localStorage.removeItem('cart');
    this.updateCartItemCount();
  }
}
