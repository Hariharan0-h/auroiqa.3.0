// services/data-entry.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface EmrConnectionTestResult {
  success: boolean;
  message: string;
}

export interface EmrSyncResult {
  success: boolean;
  recordsImported: number;
  message: string;
  errors?: string[];
}

export interface BulkUploadResult {
  successCount: number;
  errorCount: number;
  totalRows: number;
  errors?: Array<{
    row: number;
    message: string;
  }>;
}

@Injectable({
  providedIn: 'root'
})
export class DataEntryService {
  private baseUrl = `${environment.BASE_URL}api/DataEntry`;

  constructor(private http: HttpClient) {}

  // EMR Integration Methods
  testEmrConnection(connectionString: string): Observable<EmrConnectionTestResult> {
    return this.http.post<EmrConnectionTestResult>(`${this.baseUrl}/test-emr-connection`, {
      connectionString
    });
  }

  syncFromEmr(connectionString: string, dataType: 'cataract' | 'glaucoma'): Observable<EmrSyncResult> {
    return this.http.post<EmrSyncResult>(`${this.baseUrl}/sync-from-emr`, {
      connectionString,
      dataType
    });
  }

  // Template Methods
  downloadTemplate(dataType: 'cataract' | 'glaucoma'): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/download-template/${dataType}`, {
      responseType: 'blob'
    });
  }

  // Bulk Upload Methods
  uploadBulkData(formData: FormData): Observable<BulkUploadResult> {
    // Don't set Content-Type header, let browser set it with boundary for FormData
    return this.http.post<BulkUploadResult>(`${this.baseUrl}/upload-bulk-data`, formData);
  }

  // Validation Methods
  validateExcelFile(file: File): boolean {
    const allowedTypes = [
      'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
      'application/vnd.ms-excel'
    ];
    return allowedTypes.includes(file.type);
  }

  // Helper Methods
  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  }
}