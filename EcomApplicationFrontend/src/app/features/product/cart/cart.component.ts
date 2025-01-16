import { CommonModule } from '@angular/common';
import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ElementRef,
  OnInit,
  ViewChild,
} from '@angular/core';
import { CartService } from '../../../core/services/cart.service';
import { ToaterService } from '../../../core/services/toater.service';
import { NavbarComponent } from '../../auth/utility/navbar/navbar.component';
import { JwtService } from '../../../core/services/jwt.service';
import { loadStripe, Stripe } from '@stripe/stripe-js';
import { AngularStripeService } from '@fireflysemantics/angular-stripe-service';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import Swal from 'sweetalert2';
import { zip } from 'rxjs';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, NavbarComponent, ReactiveFormsModule],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css',
})
export class CartComponent implements OnInit, AfterViewInit {
  //private stripe: Stripe | null = null;
  @ViewChild('cardNumber', { static: false }) cardNumber!: ElementRef;
  @ViewChild('expiryDate', { static: false }) expiryDate!: ElementRef;
  @ViewChild('cvv', { static: false }) cvv!: ElementRef;
  stripe: any;
  cardNumberElement: any;
  cardExpiryElement: any;
  cardCvcElement: any;
  stripeToken: any;
  paymentForm: FormGroup;
  error: any;
  errorbutton: boolean = false;
  stripePaymentData:
    | { amount: any; customerName: string; customerEmail: string }
    | undefined;
  publisherKey =
    'pk_test_51QWiU5LbhFwVXMJv5qpYjm0FQV1VlQJjbZ02PpsBjgRM4yk9uTmQYrhndE6RPVH8Wa1VpKsAdN0tolfy6GIKJ5ER00ikws9sA7';
  stateName: any;
  countryName: any;
  cartItems: any[] = [];
  cart = new Set<number>();
  cartItemCount: number = 0;
  invoiceData: any[] = [];
  users: any = {};
  isLoading: boolean = false;

  // cardElement!: StripeCardElement | null;
  cardElement: any;

  constructor(
    private fb: FormBuilder,
    private cartService: CartService,
    private toasterService: ToaterService,
    private jwtService: JwtService,
    private router: Router,
    private stripeService: AngularStripeService,
    private cd: ChangeDetectorRef
  ) {
    this.paymentForm = this.fb.group({
      cardNumber: ['', [Validators.required, Validators.pattern(/^\d{16}$/)]],
      expiryDate: ['', Validators.required],
      cvv: ['', [Validators.required, Validators.pattern(/^\d{3}$/)]],
    });
  }

  // cardElement: any;
  async ngOnInit() {
    this.getCartDetails();
    this.getUserForAddress(this.userId);
    this.cartService.cartItemCount$.subscribe((count) => {
      this.cartItemCount = count;
    });
    this.cartService.updateCartItemCount();

    // Initialize Stripe
    this.stripe = await loadStripe('your-publishable-key-here');
    if (!this.stripe) {
      this.toasterService.showError('Stripe initialization failed.');
      return;
    }

    // Initialize the card element
    this.initializeStripe();
  }

  userId = this.jwtService.getUserId();

  //using subject behavoir for handlin adding and removing the data
  addItemToCart(item: any): void {
    this.cartService.addItemToCart(item);
  }

  removeItemFromCart(item: any): void {
    this.cartService.removeItemFromCart(item);
  }

  clearCart(): void {
    this.cartService.clearCart();
  }

  getCartDetails() {
    this.cartService.getProductFromCart(this.userId).subscribe(
      (response: any) => {
        if (response.status === 200) {
          console.log('responseOnly', response);
          this.cartItems = response.cartItems;
        } else {
          this.toasterService.showError('Failed to fetch cart items');
        }
      },
      (error) => {
        this.toasterService.showError('Error fetching cart items');
      }
    );
  }

  calculateTotalPrice(): number {
    return this.cartItems.reduce(
      (total, item) => total + item.price * item.quantity,
      0
    );
  }
  getUserForAddress(data: any) {
    this.cartService.getUserById(data).subscribe(
      (response) => {
        if (response.status == 200) {
          this.users = response.userData;
          this.stateName = response.stateName;
          this.countryName = response.countryName;
          console.log('userDataAddress', this.users);
        } else {
          console.log(Error);
        }
      },
      (error) => {
        console.error('unable to get response');
      }
    );
  }

  increaseQuantity(item: any): void {
    const payload = {
      prId: item.prId,
      cartId: item.cartId,
      quantity: 1,
    };
    this.cartService.IncrementQuantity(payload).subscribe(
      (response) => {
        if (response.status === 200) {
          this.updateCartInLocalStorage();
          this.getCartDetails();
        } else if (response.status === 404) {
          this.toasterService.showError(response.message);
        }
      },
      (error) => {
        this.toasterService.showError('Item is out of stock');
      }
    );
  }

  decreaseQuantity(item: any): void {
    const payload = {
      prId: item.prId,
      cartId: item.cartId,
      quantity: 1,
    };

    this.cartService.DecrementQuantity(payload).subscribe(
      (response) => {
        if (response.status == 200) {
          console.log('decrement quanity response', response);
          this.cartService.resetSetCount();
          this.getCartDetails();
        }
      },
      (error) => {
        console.error('qyanity error', error);
      }
    );
  }

  removeCartInLocalStorage() {
    localStorage.removeItem('cart');
  }

  updateCartInLocalStorage() {
    localStorage.setItem('cart', JSON.stringify(Array.from(this.cart)));
  }

  removeCartItemFromLocalStorage(itemToRemove: any): void {
    // Retrieve the current cart from localStorage
    const storedCart = localStorage.getItem('cart');
    console.log('storedCart:', storedCart);

    if (storedCart) {
      // Parse the stored cart string into an array
      let cartArray = JSON.parse(storedCart);
      console.log('cartArray before removal:', cartArray);

      cartArray.shift();

      localStorage.setItem('cart', JSON.stringify(cartArray));

      this.cart = new Set(cartArray);

      console.log('cartArray after removal:', cartArray);
    }
  }

  removeCartItem(cartId: number): void {
    console.log('cartId', cartId);

    Swal.fire({
      title: 'Are you sure?',
      text: 'Are you sure you want to delete this item from the cart?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Yes, delete it!',
      cancelButtonText: 'No, keep it',
    }).then((result) => {
      if (result.isConfirmed) {
        // Proceed with the deletion if confirmed
        this.cartService.RemoveItemFromCart(cartId).subscribe({
          next: (result: any) => {
            if (result.status == 200) {
              //   this.removeCartInLocalStorage();
              this.cartService.resetSetCount();
              this.getCartDetails();
              this.toasterService.showSuccess(
                result.message || 'Cart item deleted successfully'
              );
            } else {
              this.toasterService.showError('Unable to delete the cart item');
            }
          },
          error: (error: Error) => {
            this.toasterService.showError('Unable to get response');
            console.log(error);
          },
        });
      } else {
        // If the user cancels, do nothing
        console.log('Item deletion canceled');
      }
    });
  }

  openPaymentModal(): void {
    const modalElement = document.getElementById('paymentModal');
    if (modalElement) {
      modalElement.style.display = 'block';
      modalElement.classList.add('show');
      modalElement.setAttribute('aria-hidden', 'false');
    }
  }

  closePaymentModal(): void {
    const modal = document.getElementById('paymentModal');
    if (modal) {
      modal.style.display = 'none';
      modal.classList.remove('show');
      modal.setAttribute('aria-hidden', 'true');
    }
  }

  // processPayment(): void {
  //   if (this.paymentForm.valid) {
  //     const expiryDateRaw = this.paymentForm.get('expiryDate')?.value;
  //     const formattedExpiryDate = new Date(
  //       expiryDateRaw + 'T00:00:00.000Z'
  //     ).toISOString();
  //     const payload = {
  //       cardNumber: this.paymentForm.get('cardNumber')?.value,
  //       expiryDate: formattedExpiryDate,
  //       address: this.users.address,
  //       userId: this.users.userId,
  //       zipcode: String(this.users.zipcode),
  //       cvv: this.paymentForm.get('cvv')?.value,
  //       stateName: this.stateName,
  //       countryName: this.countryName,
  //     };
  //     console.log('payment payload', payload);

  //     this.cartService.addPayment(payload).subscribe(
  //       (response) => {
  //         if (response.status == 200) {
  //           console.log('payment', response);
  //           this.cartService.resetSetCount();
  //           this.toasterService.showSuccess('Payment successful!');

  //           this.closePaymentModal();
  //           this.router.navigateByUrl(
  //             `/product/Invoice/${response.data.salesId}`
  //           );
  //           this.paymentForm.reset();
  //         } else {
  //           this.toasterService.showError(response.message);
  //         }
  //       },
  //       (error) => {
  //         this.toasterService.showError(error.message);
  //       }
  //     );
  //   } else {
  //     this.paymentForm.markAllAsTouched(); // Show validation errors
  //   }
  // }

  ngAfterViewInit() {
    this.initializeStripe();
  }
  initializeStripe() {
    this.stripeService.setPublishableKey(this.publisherKey).then((stripe) => {
      this.stripe = stripe;
      const elements = stripe.elements();

      this.cardNumberElement = elements.create('cardNumber', {
        placeholder: '4242 4242 4242 4242',
      });

      // Initialize the Expiry Date element
      this.cardExpiryElement = elements.create('cardExpiry', {
        placeholder: 'MM/YY',
      });

      // Initialize the CVV element
      this.cardCvcElement = elements.create('cardCvc', {
        placeholder: 'CVV',
      });

      // Mount the elements to their respective HTML div containers
      this.cardNumberElement.mount(this.cardNumber.nativeElement);
      this.cardExpiryElement.mount(this.expiryDate.nativeElement);
      this.cardCvcElement.mount(this.cvv.nativeElement);

      // Event listener to handle errors
      this.cardNumberElement.addEventListener(
        'change',
        this.onChange.bind(this)
      );
      this.cardExpiryElement.addEventListener(
        'change',
        this.onChange.bind(this)
      );
      this.cardCvcElement.addEventListener('change', this.onChange.bind(this));
    });
  }
  onChange({ error }: { error: Error }) {
    if (error) {
      this.error = error.message;
      this.errorbutton = false;
    } else {
      this.error = null;
      this.errorbutton = true;
    }
    this.cd.detectChanges();
  }

  // async onClickPayBtn() {
  //   console.log('put your stripe publishable key here');

  //   // Ensure card elements are valid
  //   if (
  //     !this.cardNumberElement ||
  //     !this.cardExpiryElement ||
  //     !this.cardCvcElement
  //   ) {
  //     alert('Stripe Elements are not initialized correctly');
  //     return;
  //   }

  //   const { token, error } = await this.stripe.createToken(
  //     this.cardNumberElement
  //   );
  //   if (token != undefined) {
  //     this.stripeToken = token;
  //     this.processPayment();
  //   } else {
  //     alert(error.message);
  //   }
  // }
  async onClickPayBtn() {
    // Ensure card elements are valid
    if (
      !this.cardNumberElement ||
      !this.cardExpiryElement ||
      !this.cardCvcElement
    ) {
      alert('Stripe Elements are not initialized correctly');
      return;
    }

    // this.isLoader = true;
    // Create the token for the card details entered by the user
    const { token, error } = await this.stripe.createToken(
      this.cardNumberElement
    );
    if (token != undefined) {
      this.stripeToken = token;
      this.processPayment();
    } else {
      alert(error.message);
    }
  }

  processPayment() {
    this.isLoading = true;
    const paymentPayload = {
      sourceToken: this.stripeToken.id.toString(),
      amount: this.calculateTotalPrice(),
      customerName: this.users.firstName,
      customerEmail: this.users.email,
      address: this.users.address,
      countryName: String(this.users.countryId),
      stateName: String(this.users.stateId),
      userId: this.users.userId,
      zipCode: String(this.users.zipcode),
      email: this.users.email,
    };

    this.cartService.addPaymentWithStripe(paymentPayload).subscribe({
      next: (res) => {
        if (res.status === 200) {
          Swal.fire({
            title: 'Payment Successful!',
            text: `Your payment of ${paymentPayload.amount} was successful.`,
            icon: 'success',
            confirmButtonText: 'OK',
          });
          this.paymentForm.reset();
          this.cartService.resetSetCount();
          this.closePaymentModal();
          this.getCartDetails();
          this.toasterService.showSuccess(res.message);
          this.router.navigateByUrl(`/product/Invoice/${res.data.salesId}`);
        } else {
          this.isLoading = false;
          Swal.fire({
            title: 'Payment Failed!',
            text: res.message || 'An error occurred during payment.',
            icon: 'error',
            confirmButtonText: 'Try Again',
          });
        }
      },
      error: (err: Error) => {
        this.isLoading = false;
        console.log('Error to Order : ', err);
        this.toasterService.showError('Server Error...!');
      },
    });
  }
}
