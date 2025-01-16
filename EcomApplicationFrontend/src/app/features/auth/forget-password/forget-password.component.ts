import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { AuthServicesService } from '../../../core/services/auth-services.service';
import { Router } from '@angular/router';
import { ToaterService } from '../../../core/services/toater.service';

@Component({
  selector: 'app-forget-password',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './forget-password.component.html',
  styleUrl: './forget-password.component.css',
})
export class ForgetPasswordComponent implements OnInit {
  emailForm!: FormGroup;

  resetPasswordForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private authService: AuthServicesService,
    private toasterService: ToaterService
  ) {}

  ngOnInit(): void {
    this.emailForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
    });
  }

  sendResetLink(): void {
    if (this.emailForm.valid) {
      const email = this.emailForm.value.email;
      this.authService.forgotPassword(email).subscribe(
        (response) => {
          if (response.status == 200) {
            this.router.navigateByUrl('/auth/sendOtp');
            this.toasterService.showSuccess(response.message);
          } else {
            this.toasterService.showError(response.message);
          }
        },
        (error) => {
          this.toasterService.showError(error.message);
        }
      );
    }
  }
}
