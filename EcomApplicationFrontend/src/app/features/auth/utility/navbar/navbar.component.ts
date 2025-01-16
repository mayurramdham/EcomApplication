import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { JwtService } from '../../../../core/services/jwt.service';
import { ToaterService } from '../../../../core/services/toater.service';
import { AuthServicesService } from '../../../../core/services/auth-services.service';
import { CartService } from '../../../../core/services/cart.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, CommonModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
})
export class NavbarComponent implements OnInit {
  userId?: number;
  usersData: any = {};
  userName: string = '';
  isAdmin = false;
  currentRole: string = '';
  cartItemCount: number = 0;
  cartCaunt: number = 0;

  constructor(private router: Router) {
    this.currentRole = this.jwtService.getRole();
    this.userName = this.jwtService.getName();
    this.userId = this.jwtService.getUserId();
  }
  ngOnInit(): void {
    if (this.userId) this.getUserData(this.userId);
    console.log('currentRole', this.currentRole);
    this.cartService.cartItemCount$.subscribe((count) => {
      this.cartItemCount = count; // Update the cart item count
    });
    this.cartService.cartItem$.subscribe((count) => {
      this.cartCaunt = count.length;
    });
  }
  private jwtService = inject(JwtService);
  private authService = inject(AuthServicesService);
  private toaster = inject(ToaterService);
  private cartService = inject(CartService);

  logout() {
    Swal.fire({
      title: 'Are You Sure',
      text: 'Are You Sure You want to logout from the application?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Yes, Logout from application',
      cancelButtonText: 'No, keep it log in',
    }).then((result) => {
      if (result.isConfirmed) {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('userId');
        localStorage.removeItem('userName');
        this.router.navigate(['/auth/sendOtp']); // Redirect to login page
      }
    });
  }
  profile() {
    this.router.navigate(['/org/profile']);
  }
  goToCart() {
    this.router.navigate(['/product/cart']);
  }
  Test() {
    console.log('in jwt test');
    const role = this.jwtService.getRole();
    const name = this.jwtService.getName();
    const userId = this.jwtService.getUserId();
    console.log('userId', userId);
  }
  getUserData(userId: number) {
    console.log('id', userId);
    this.authService.getUserById(userId).subscribe((response) => {
      if (response.status == 200) {
        this.usersData = response.userData;
        console.log('userData', this.usersData);
      }
    });
  }
}
