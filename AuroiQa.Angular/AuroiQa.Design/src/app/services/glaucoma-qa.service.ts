import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { PatientGlaucomaQa, ApiResponse, StatsResponse, IOPAnalysisResponse, ComplicationsResponse } from '../models/data.models';

@Injectable({
  providedIn: 'root'
})
export class GlaucomaQaService {
  private baseUrl = `${environment.BASE_URL}api/GlaucomaQaReport`;

  constructor(private http: HttpClient) {}

  getAllRecords(filters?: {
    page?: number;
    pageSize?: number;
    patientName?: string;
    mrNo?: string;
    surgeryDateFrom?: Date;
    surgeryDateTo?: Date;
    eyePart?: string;
    surgeonType?: string;
    surgeryType?: string;
    isActive?: boolean;
  }): Observable<ApiResponse<PatientGlaucomaQa[]>> {
    let params = new HttpParams();
    
    if (filters) {
      Object.keys(filters).forEach(key => {
        const value = (filters as any)[key];
        if (value !== undefined && value !== null && value !== '') {
          params = params.set(key, value.toString());
        }
      });
    }

    return this.http.get<ApiResponse<PatientGlaucomaQa[]>>(this.baseUrl, { params });
  }

  getRecord(id: number): Observable<PatientGlaucomaQa> {
    return this.http.get<PatientGlaucomaQa>(`${this.baseUrl}/${id}`);
  }

  createRecord(record: PatientGlaucomaQa): Observable<any> {
    return this.http.post<any>(this.baseUrl, record);
  }

  updateRecord(id: number, record: PatientGlaucomaQa): Observable<any> {
    return this.http.put<any>(`${this.baseUrl}/${id}`, record);
  }

  deleteRecord(id: number, hardDelete: boolean = false): Observable<any> {
    const params = new HttpParams().set('hardDelete', hardDelete.toString());
    return this.http.delete<any>(`${this.baseUrl}/${id}`, { params });
  }

  searchRecords(searchTerm: string, page: number = 1, pageSize: number = 10): Observable<ApiResponse<PatientGlaucomaQa[]>> {
    const params = new HttpParams()
      .set('searchTerm', searchTerm)
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<ApiResponse<PatientGlaucomaQa[]>>(`${this.baseUrl}/search`, { params });
  }

  getPatientRecords(mrNo: string): Observable<ApiResponse<PatientGlaucomaQa[]>> {
    return this.http.get<ApiResponse<PatientGlaucomaQa[]>>(`${this.baseUrl}/patient/${mrNo}`);
  }

  getStatistics(): Observable<StatsResponse> {
    return this.http.get<StatsResponse>(`${this.baseUrl}/stats`);
  }

  getIOPAnalysis(): Observable<IOPAnalysisResponse> {
    return this.http.get<IOPAnalysisResponse>(`${this.baseUrl}/iop-analysis`);
  }

  getComplicationsAnalysis(): Observable<ComplicationsResponse> {
    return this.http.get<ComplicationsResponse>(`${this.baseUrl}/complications`);
  }
}