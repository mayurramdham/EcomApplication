import { Component, inject, OnInit } from '@angular/core';
import { AuthServicesService } from '../../../core/services/auth-services.service';
import { JwtService } from '../../../core/services/jwt.service';
import { NavbarComponent } from '../../auth/utility/navbar/navbar.component';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CommonModule, formatDate } from '@angular/common';
import { ToaterService } from '../../../core/services/toater.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [NavbarComponent, CommonModule, ReactiveFormsModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css',
})
export class ProfileComponent implements OnInit {
  userData: any;
  updateProfileForm!: FormGroup;
  StateName: any;
  countryName: any;
  changePasswordForm: FormGroup;
  jwtService = inject(JwtService);
  toasterService = inject(ToaterService);
  private router = inject(Router);
  todayDate = new Date().toISOString().split('T')[0];

  constructor(
    private authService: AuthServicesService,
    private fb: FormBuilder
  ) {
    this.changePasswordForm = this.fb.group(
      {
        username: ['', [Validators.required, Validators.minLength(3)]],
        password: ['', [Validators.required, Validators.minLength(6)]],
        confirmPassword: ['', [Validators.required, Validators.minLength(6)]],
      },
      {
        validator: this.passwordsMatchValidator,
      }
    );
  }

  onOpenupdateProfileModal(userData: any) {
    this.updateProfileForm.patchValue(userData);
    this.updateProfileForm
      .get('dob')
      ?.setValue(
        userData.dob ? new Date(userData.dob).toISOString().split('T')[0] : ''
      );

    console.log('updatefromvalue', userData);
    this.updateProfileForm.value.dob = formatDate(
      userData.dob,
      'dd-MM-yyyy',
      'en'
    );
    console.log('updatefromvalue2', this.updateProfileForm.value.dob);
    const modal = document.getElementById('updateProfileModal');
    if (modal) {
      modal.style.display = 'block';
      modal.classList.add('show');
      modal.setAttribute('aria-hidden', 'false');
    }
  }

  onCloseupdateProfileModal() {
    const modal = document.getElementById('updateProfileModal');
    if (modal) {
      modal.style.display = 'none';
      modal.classList.remove('show');
      modal.setAttribute('aria-hidden', 'true');
    }
  }

  //handle update profile logic code
  onUpdateProfile(): void {
    if (this.updateProfileForm.valid) {
      console.log('Form Submitted!', this.updateProfileForm.value);
      this.authService
        .updateUser(this.updateProfileForm.value)
        .subscribe((response) => {
          if (response.status == 200) {
            this.toasterService.showSuccess('Profile updated successfully');
            const userId = this.jwtService.getUserId();
            this.getUserData(userId);
            this.onCloseupdateProfileModal();
          }
        });
    } else {
      this.toasterService.showError('Please fill out the form correctly.');
    }
  }

  openModal() {
    const modal = document.getElementById('changePasswordModal');
    if (modal) {
      modal.style.display = 'block';
      modal.classList.add('show');
      modal.setAttribute('aria-hidden', 'false');
    }
  }

  closeModal() {
    const modal = document.getElementById('changePasswordModal');
    if (modal) {
      modal.style.display = 'none';
      modal.classList.remove('show');
      modal.setAttribute('aria-hidden', 'true');
    }
  }

  changePasswordModal(product: any) {
    console.log('product', product);

    // this.productForm.get('PrName')?.setValue(product.pr)
    // this.productForm.get()
    const modal = document.getElementById('changePasswordModal');
    if (modal) {
      modal.style.display = 'block';
      modal.classList.add('show');
      modal.setAttribute('aria-hidden', 'false');
    }
  }

  passwordsMatchValidator(group: FormGroup): { [key: string]: boolean } | null {
    const password = group.get('password')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;

    return password && confirmPassword && password !== confirmPassword
      ? { passwordMismatch: true }
      : null;
  }

  onSubmit(): void {
    if (this.changePasswordForm.invalid) {
      console.log('Form is invalid!');
      return;
    }
    const payload = {
      username: this.changePasswordForm.get('username')?.value,
      password: this.changePasswordForm.get('password')?.value,
    };
    console.log('payload is', payload);
    this.authService.changePassword(payload).subscribe(
      (response) => {
        if (response.status == 200) {
          this.router.navigateByUrl('auth/sendOtp');
          this.toasterService.showSuccess('Password Change Successfully');
        } else {
          this.toasterService.showError('Invalid credentials');
        }
      },
      (error) => {
        this.toasterService.showError('unable to get response');
      }
    );
    const { username, password } = this.changePasswordForm.value;
    console.log(`Password for ${username} changed to ${password}`);
  }

  ngOnInit(): void {
    const userId = this.jwtService.getUserId();
    // Replace with actual logic to get userId
    this.getUserData(userId);

    this.updateProfileForm = new FormGroup({
      userId: new FormControl(''),
      firstName: new FormControl('', Validators.required),
      lastName: new FormControl('', Validators.required),
      email: new FormControl('', [Validators.required, Validators.email]),
      dob: new FormControl('', Validators.required),
      mobile: new FormControl('', Validators.required),
      address: new FormControl('', Validators.required),
      zipcode: new FormControl('', Validators.required),
    });
  }

  getUserData(userId: number): void {
    this.authService.getUserById(userId).subscribe((response: any) => {
      if (response.status === 200) {
        this.userData = response.userData;
        this.StateName = response.stateName;
        this.countryName = response.countryName;

        console.log('User Data:', this.userData);
      }
    });
  }

  getUserType(userTypeId: number | undefined): string {
    if (userTypeId === 1) return 'Admin';
    if (userTypeId === 2) return 'User';
    return 'Unknown';
  }
}
