import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
@Injectable({
  providedIn: 'root',
})
export class DropdownService {
  constructor(private http: HttpClient) {}

  getAllCoutries(): Observable<any> {
    return this.http.get(`${environment.apiBaseUrl}/api/Dropdown/GetCountries`);
  }

  getAllStateByCountryId(countryId: number): Observable<any> {
    return this.http.get(
      `${environment.apiBaseUrl}/api/Dropdown/GetStatesByCountry/${countryId}`
    );
  }
}
