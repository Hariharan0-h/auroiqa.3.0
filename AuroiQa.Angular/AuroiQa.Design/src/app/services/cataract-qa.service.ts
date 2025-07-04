import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { PatientCataractQa, ApiResponse, StatsResponse } from '../models/data.models';

@Injectable({
  providedIn: 'root'
})
export class CataractQaService {
  private baseUrl = `${environment.BASE_URL}api/CataractQaReport`;

  constructor(private http: HttpClient) {}

  getAllRecords(filters?: {
    page?: number;
    pageSize?: number;
    patientName?: string;
    mrNo?: string;
    surgeryDateFrom?: Date;
    surgeryDateTo?: Date;
    eye?: string;
    surgeonType?: string;
  }): Observable<ApiResponse<PatientCataractQa[]>> {
    let params = new HttpParams();
    
    if (filters) {
      Object.keys(filters).forEach(key => {
        const value = (filters as any)[key];
        if (value !== undefined && value !== null && value !== '') {
          params = params.set(key, value.toString());
        }
      });
    }

    return this.http.get<ApiResponse<PatientCataractQa[]>>(this.baseUrl, { params });
  }

  getRecord(id: number): Observable<PatientCataractQa> {
    return this.http.get<PatientCataractQa>(`${this.baseUrl}/${id}`);
  }

  createRecord(record: PatientCataractQa): Observable<any> {
    return this.http.post<any>(this.baseUrl, record);
  }

  updateRecord(id: number, record: PatientCataractQa): Observable<any> {
    return this.http.put<any>(`${this.baseUrl}/${id}`, record);
  }

  deleteRecord(id: number): Observable<any> {
    return this.http.delete<any>(`${this.baseUrl}/${id}`);
  }

  searchRecords(searchTerm: string, page: number = 1, pageSize: number = 10): Observable<ApiResponse<PatientCataractQa[]>> {
    const params = new HttpParams()
      .set('searchTerm', searchTerm)
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<ApiResponse<PatientCataractQa[]>>(`${this.baseUrl}/search`, { params });
  }

  getPatientRecords(mrNo: string): Observable<ApiResponse<PatientCataractQa[]>> {
    return this.http.get<ApiResponse<PatientCataractQa[]>>(`${this.baseUrl}/patient/${mrNo}`);
  }

  getStatistics(): Observable<StatsResponse> {
    return this.http.get<StatsResponse>(`${this.baseUrl}/stats`);
  }
}