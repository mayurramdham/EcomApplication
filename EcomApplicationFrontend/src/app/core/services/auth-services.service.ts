import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
//import { environment } from '../../enviorments/enviorment';

@Injectable({
  providedIn: 'root',
})
export class AuthServicesService {
  private baseUrl = environment.apiBaseUrl; // Use the base URL from the environment file

  constructor(private http: HttpClient) {}

  registerData(data: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/api/User/register`, data);
  }

  loginData(data: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/api/User/Otp`, data);
  }

  verfiOtp(data: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/api/User/login`, data);
  }

  getUserById(userId: any): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/api/User/getUserById/${userId}`);
  }

  changePassword(userData: any): Observable<any> {
    return this.http.post<any>(
      `${this.baseUrl}/api/User/ChangePassword`,
      userData
    );
  }

  forgotPassword(useremail: any): Observable<any> {
    return this.http.get<any>(
      `${this.baseUrl}/api/User/ForgetPassword/${useremail}`
    );
  }

  updateUser(userData: any): Observable<any> {
    return this.http.put<any>(`${this.baseUrl}/api/User/updateUser`, userData);
  }
}
