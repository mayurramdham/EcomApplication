import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { ProductService } from '../../../core/services/product.service';
import { NavbarComponent } from '../../auth/utility/navbar/navbar.component';
import { CartService } from '../../../core/services/cart.service';
import { JwtService } from '../../../core/services/jwt.service';
import { ToaterService } from '../../../core/services/toater.service';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, NavbarComponent],
  templateUrl: './users.component.html',
  styleUrl: './users.component.css',
})
export class UsersComponent implements OnInit {
  products: any[] = [];
  productService = inject(ProductService);
  cartService = inject(CartService);
  jwtService = inject(JwtService);
  toasterService = inject(ToaterService);
  productId: number = 0;
  userId: number = 0;
  quantity: number = 1;
  cartItemList: number[] = [];

  ngOnInit(): void {
    this.getAllProducts();
    this.loadCartFromLocalStorage();
    this.userId = this.jwtService.getUserId();
    this.cartService.cartItem$.subscribe({
      next: (res: any) => {
        console.log('cartres', res);
        this.cartItemList = res;
      },
    });
  }

  isExistInCart(productId: number): boolean {
    console.log(this.cartItemList.includes(productId));
    return this.cartItemList.includes(productId);
    // return this.cartItemList.some((x) => x == productId);
  }
  getAllProducts() {
    this.productService.getProducts().subscribe(
      (response: any) => {
        console.log('productResponse', response);
        if (response.status === 200) {
          this.products = response.product;

          console.log('productResponse2', this.products);
        } else {
          console.error('Error fetching products:', response.message);
        }
      },
      (error) => {
        console.error('Error:', error);
      }
    );
  }

  ///***********Delieving With Cart Item************ */
  addItemsToCart(prId: number) {
    const payload = {
      prId: prId,
      userId: this.userId,
      quantity: 1,
    };

    // Prevent duplicate additions
    if (this.cart.has(prId)) {
      this.toasterService.showError('Item is already in the cart');
      return;
    }

    this.cartService.addToCart(payload).subscribe(
      (response) => {
        if (response.status === 200) {
          // Add item to local cart state
          this.cart.add(prId);
          this.cartService.resetSetCount();
          //   this.updateCartInLocalStorage(); // changes made bu subject behaviour 05-12
          //  this.cartService.updateCartItemCount();

          this.toasterService.showSuccess('Item Added Successfully');
        } else {
          this.toasterService.showError('Error while adding the item');
        }
      },
      (error) => {
        this.toasterService.showError('Unable to add item to cart');
      }
    );
  }

  cart = new Set<number>();
  isProductInCart(productId: number): boolean {
    return this.cart.has(productId);
  }

  updateCartInLocalStorage() {
    localStorage.setItem('cart', JSON.stringify(Array.from(this.cart)));
  }

  removeCartInLocalStorage() {
    localStorage.removeItem('cart');
  }
  removeCartItemFromLocalStorage(itemToRemove: any): void {
    // Remove the item from the Set
    this.cart.delete(itemToRemove);

    // Update localStorage with the new state of the cart
    this.updateCartInLocalStorage();
  }

  loadCartFromLocalStorage() {
    const storedCart = localStorage.getItem('cart');
    if (storedCart) {
      this.cart = new Set(JSON.parse(storedCart));
    }
  }
}
