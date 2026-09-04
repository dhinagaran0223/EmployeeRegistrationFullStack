import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  Country, EmployeeDetail, EmployeeList, EmployeePayload, PagedResult, State
} from '../models/employee.model';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private http = inject(HttpClient);
  private base = 'http://localhost:5000/api';

  getEmployees(page: number, pageSize: number, name: string, mobile: string): Observable<PagedResult<EmployeeList>> {
    let params = new HttpParams()
      .set('pageNumber', page)
      .set('pageSize', pageSize);
    if (name.trim()) params = params.set('name', name.trim());
    if (mobile.trim()) params = params.set('mobile', mobile.trim());
    return this.http.get<PagedResult<EmployeeList>>(`${this.base}/employees`, { params });
  }

  getEmployee(id: number): Observable<EmployeeDetail> {
    return this.http.get<EmployeeDetail>(`${this.base}/employees/${id}`);
  }

  createEmployee(payload: EmployeePayload): Observable<EmployeeDetail> {
    return this.http.post<EmployeeDetail>(`${this.base}/employees`, payload);
  }

  updateEmployee(id: number, payload: EmployeePayload): Observable<EmployeeDetail> {
    return this.http.put<EmployeeDetail>(`${this.base}/employees/${id}`, payload);
  }

  deleteEmployee(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/employees/${id}`);
  }

  getCountries(): Observable<Country[]> {
    return this.http.get<Country[]>(`${this.base}/countries`);
  }

  getStatesByCountry(countryId: number): Observable<State[]> {
    return this.http.get<State[]>(`${this.base}/countries/${countryId}/states`);
  }

  getCountryByState(stateId: number): Observable<Country> {
    return this.http.get<Country>(`${this.base}/states/${stateId}/country`);
  }
}
