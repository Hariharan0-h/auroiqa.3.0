// settings.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { CataractQaService } from '../../services/cataract-qa.service';
import { GlaucomaQaService } from '../../services/glaucoma-qa.service';
import { DataEntryService } from '../../services/data-entry.service';
import { PatientCataractQa, PatientGlaucomaQa } from '../../models/data.models';

export type DataEntryMethod = 'emr' | 'manual' | 'bulk' | null;
export type DataType = 'cataract' | 'glaucoma';

@Component({
  selector: 'app-settings',
  imports: [CommonModule, FormsModule, SidebarComponent],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.css'
})
export class SettingsComponent implements OnInit {
  activeRoute = 'settings';
  
  // Data Entry State
  selectedMethod: DataEntryMethod = null;
  selectedDataType: DataType = 'cataract';
  
  // EMR Integration
  emrConnectionString = '';
  testingConnection = false;
  connectionTestResult = '';
  
  // Manual Entry Forms
  cataractForm: Partial<PatientCataractQa> = this.getEmptyCataractForm();
  glaucomaForm: Partial<PatientGlaucomaQa> = this.getEmptyGlaucomaForm();
  
  // Bulk Upload
  selectedFile: File | null = null;
  uploadProgress = 0;
  uploadResults: any = null;
  
  // Loading States
  savingForm = false;
  uploadingFile = false;
  
  // Error States
  formError = '';
  uploadError = '';
  
  constructor(
    private cataractService: CataractQaService,
    private glaucomaService: GlaucomaQaService,
    private dataEntryService: DataEntryService
  ) {}

  ngOnInit(): void {
    // Load saved EMR connection string if exists
    this.loadSavedSettings();
  }

  loadSavedSettings(): void {
    const savedConnectionString = localStorage.getItem('emr_connection_string');
    if (savedConnectionString) {
      this.emrConnectionString = savedConnectionString;
    }
  }

  selectMethod(method: DataEntryMethod): void {
    this.selectedMethod = method;
    this.resetForms();
  }

  selectDataType(type: DataType): void {
    this.selectedDataType = type;
    this.resetForms();
  }

  goBack(): void {
    this.selectedMethod = null;
    this.resetForms();
  }

  resetForms(): void {
    this.cataractForm = this.getEmptyCataractForm();
    this.glaucomaForm = this.getEmptyGlaucomaForm();
    this.formError = '';
    this.uploadError = '';
    this.uploadResults = null;
    this.selectedFile = null;
  }

  // EMR Integration Methods
  async testEmrConnection(): Promise<void> {
    if (!this.emrConnectionString.trim()) {
      this.connectionTestResult = 'Please enter a connection string';
      return;
    }

    this.testingConnection = true;
    this.connectionTestResult = '';

    try {
      const result = await this.dataEntryService.testEmrConnection(this.emrConnectionString).toPromise();
      if (result) {
        this.connectionTestResult = result.success ? 'Connection successful!' : 'Connection failed: ' + result.message;
        
        if (result.success) {
          localStorage.setItem('emr_connection_string', this.emrConnectionString);
        }
      } else {
        this.connectionTestResult = 'Connection test failed - no response received';
      }
    } catch (error) {
      this.connectionTestResult = 'Connection failed: ' + (error as any).message;
    } finally {
      this.testingConnection = false;
    }
  }

  async syncFromEmr(): Promise<void> {
    if (!this.emrConnectionString.trim()) {
      alert('Please test the EMR connection first');
      return;
    }

    this.testingConnection = true;
    try {
      const result = await this.dataEntryService.syncFromEmr(this.emrConnectionString, this.selectedDataType).toPromise();
      if (result) {
        alert(`Sync completed! ${result.recordsImported} records imported successfully.`);
      } else {
        alert('Sync failed - no response received');
      }
    } catch (error) {
      alert('Sync failed: ' + (error as any).message);
    } finally {
      this.testingConnection = false;
    }
  }

  // Manual Entry Methods
  async saveManualEntry(): Promise<void> {
    this.savingForm = true;
    this.formError = '';

    try {
      let result;
      if (this.selectedDataType === 'cataract') {
        result = await this.cataractService.createRecord(this.cataractForm as PatientCataractQa).toPromise();
      } else {
        result = await this.glaucomaService.createRecord(this.glaucomaForm as PatientGlaucomaQa).toPromise();
      }
      
      if (result) {
        alert('Record saved successfully!');
        this.resetForms();
      } else {
        this.formError = 'Failed to save record - no response received';
      }
    } catch (error) {
      this.formError = 'Failed to save record: ' + (error as any).message;
    } finally {
      this.savingForm = false;
    }
  }

  // Bulk Upload Methods
  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file && (file.type === 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' || 
                 file.type === 'application/vnd.ms-excel')) {
      this.selectedFile = file;
      this.uploadError = '';
    } else {
      this.uploadError = 'Please select a valid Excel file (.xlsx or .xls)';
      this.selectedFile = null;
    }
  }

  downloadTemplate(): void {
    this.dataEntryService.downloadTemplate(this.selectedDataType).subscribe(
      (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `${this.selectedDataType}_template.xlsx`;
        link.click();
        window.URL.revokeObjectURL(url);
      },
      (error) => {
        alert('Failed to download template: ' + error.message);
      }
    );
  }

  async uploadBulkData(): Promise<void> {
    if (!this.selectedFile) {
      this.uploadError = 'Please select a file first';
      return;
    }

    this.uploadingFile = true;
    this.uploadProgress = 0;
    this.uploadError = '';
    this.uploadResults = null;

    try {
      const formData = new FormData();
      formData.append('file', this.selectedFile);
      formData.append('dataType', this.selectedDataType);

      const result = await this.dataEntryService.uploadBulkData(formData).toPromise();
      if (result) {
        this.uploadResults = result;
        this.uploadProgress = 100;
        
        if (result.errors && result.errors.length > 0) {
          this.uploadError = `Upload completed with ${result.errors.length} errors. Check results for details.`;
        }
      } else {
        this.uploadError = 'Upload failed - no response received';
      }
    } catch (error) {
      this.uploadError = 'Upload failed: ' + (error as any).message;
    } finally {
      this.uploadingFile = false;
    }
  }

  // Form Helpers
  private getEmptyCataractForm(): Partial<PatientCataractQa> {
    return {
      PATIENT_NAME: '',
      Mr_no: '',
      SurgeryDate: undefined,
      Lensname: '',
      Modelname: '',
      IolPower: '',
      Eye: '',
      SurgeryType: '',
      Age: undefined,
      SEX: '',
      SurgeonType: '',
      Anesthesia: '',
      Pre_BCVA: '',
      Pre_UCVA: '',
      FollowupBCVA: '',
      FollowupUCVA: ''
    };
  }

  private getEmptyGlaucomaForm(): Partial<PatientGlaucomaQa> {
    return {
      Patient_name: '',
      Mr_no: '',
      Surgery_date: undefined,
      Eye_part: '',
      Surgery_type: '',
      Age: undefined,
      Sex: '',
      Surgeon_type: '',
      Anesthesia: '',
      Pre_operative_iop: undefined,
      Post_operative_iop: undefined,
      Pre_bcva: '',
      Pre_ucva: '',
      Followup_bcva: '',
      Followup_ucva: '',
      Is_active: true
    };
  }

  // Validation Helpers
  isFormValid(): boolean {
    if (this.selectedDataType === 'cataract') {
      return !!(this.cataractForm.PATIENT_NAME && 
                this.cataractForm.Mr_no && 
                this.cataractForm.SurgeryType);
    } else {
      return !!(this.glaucomaForm.Patient_name && 
                this.glaucomaForm.Mr_no && 
                this.glaucomaForm.Surgery_type);
    }
  }
}