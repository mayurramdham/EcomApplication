import { registerLocaleData } from '@angular/common';
import { Routes } from '@angular/router';
import { RegisterComponent } from './features/auth/register/register.component';
import { OtpComponent } from './features/auth/otp/otp.component';
import { LoginComponent } from './features/auth/login/login.component';
import { AuthComponentComponent } from './features/auth/auth-component/auth-component.component';
import { ForgetPasswordComponent } from './features/auth/forget-password/forget-password.component';
import { OrgComponent } from './features/org/org/org.component';
import { HomeComponent } from './features/org/home/home.component';
import { guardsGuard } from './core/guards.guard';
import { ProfileComponent } from './features/org/profile/profile.component';
import { producerAccessed } from '@angular/core/primitives/signals';
import { ProductComponent } from './features/product/product/product.component';
import { AddProductComponent } from './features/product/add-product/add-product.component';
import { UsersComponent } from './features/org/users/users.component';
import { CartComponent } from './features/product/cart/cart.component';
import { InvoiceComponent } from './features/product/invoice/invoice.component';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'auth/sendOtp',
    pathMatch: 'full',
  },
  {
    path: 'auth',
    component: AuthComponentComponent,
    children: [
      {
        path: 'sendOtp',
        component: OtpComponent,
      },
      {
        path: 'login',
        component: LoginComponent,
      },
      {
        path: 'register',
        component: RegisterComponent,
      },
      {
        path: 'forget-password',
        component: ForgetPasswordComponent,
      },
    ],
  },
  {
    path: 'org',
    component: OrgComponent,
    children: [
      {
        path: 'home',
        component: HomeComponent,
        canActivate: [guardsGuard],
        data: { roles: ['Admin', 'User'] },
      },

      {
        path: 'profile',
        component: ProfileComponent,
        canActivate: [guardsGuard],
        data: { roles: ['Admin', 'User'] },
      },
      {
        path: 'users',
        component: UsersComponent,
        canActivate: [guardsGuard],
        data: { roles: ['User'] },
      },
    ],
  },
  {
    path: 'product',
    component: ProductComponent,
    children: [
      {
        path: 'add-product',
        component: AddProductComponent,
        canActivate: [guardsGuard],
        data: { roles: ['Admin'] },
      },
      {
        path: 'cart',
        component: CartComponent,
        canActivate: [guardsGuard],
        data: { roles: ['User'] },
      },
      {
        path: 'Invoice/:id',
        component: InvoiceComponent,
        title: 'Invoice Page',
        canActivate: [guardsGuard],
        data: { roles: ['User'] },
      },
    ],
  },
];
