import { Component, inject } from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthServicesService } from '../../../core/services/auth-services.service';
import { ToaterService } from '../../../core/services/toater.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, CommonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  authService = inject(AuthServicesService);
  toasterService = inject(ToaterService);
  router = inject(Router);
  isLoading: boolean = false;
  otpDataValue: any = {};
  otpData: FormGroup = new FormGroup({
    userName: new FormControl(''),
    otpCode: new FormControl('', [
      Validators.required,
      Validators.pattern(/^\d{6}$/), // Ensures only 6-digit numbers are valid
    ]),
  });

  otpVerify() {
    this.isLoading = false;
    if (this.otpData.value) {
      const otpValue = Number(this.otpData.get('otpCode')?.value);
      const payload = {
        OtpCode: otpValue,
        userName: localStorage.getItem('userName'),
      };

      this.authService.verfiOtp(payload).subscribe(
        (res: any) => {
          console.log('api response', res);
          console.error('error at verify');
          if (res.status == 200) {
            this.isLoading = true;
            this.toasterService.showSuccess(res.message);
            localStorage.removeItem('email');
            localStorage.setItem('accessToken', res.token);
            this.router.navigateByUrl('/org/home');
          } else {
            this.toasterService.showError(res.message);
          }
        },
        (error) => {
          this.toasterService.showError(error.message);
        }
      );
    } else {
      this.toasterService.showError('Please enter a valid OTP');
    }
  }

  validateOtpLength(event: Event): void {
    const input = event.target as HTMLInputElement;
    const value = input.value;

    if (value.length > 6) {
      // Limit the input to 6 digits
      input.value = value.slice(0, 6);
    }
  }
}
