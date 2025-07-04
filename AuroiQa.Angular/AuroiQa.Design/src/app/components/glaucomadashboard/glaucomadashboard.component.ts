// glaucomadashboard.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { GlaucomaQaService } from '../../services/glaucoma-qa.service';
import { PatientGlaucomaQa, StatsResponse, IOPAnalysisResponse, ComplicationsResponse } from '../../models/data.models';

@Component({
  selector: 'app-glaucomadashboard',
  imports: [CommonModule, SidebarComponent, FormsModule],
  templateUrl: './glaucomadashboard.component.html',
  styleUrl: './glaucomadashboard.component.css'
})
export class GlaucomadashboardComponent implements OnInit {
  activeRoute = 'glaucoma';
  
  // Data properties
  statistics: StatsResponse | null = null;
  iopAnalysis: IOPAnalysisResponse | null = null;
  complications: ComplicationsResponse | null = null;
  recentProcedures: PatientGlaucomaQa[] = [];
  
  // Loading states
  loadingStats = false;
  loadingIOP = false;
  loadingComplications = false;
  loadingProcedures = false;
  
  // Error states
  errorStats = '';
  errorIOP = '';
  errorComplications = '';
  errorProcedures = '';

  // Filter properties
  searchTerm = '';
  selectedDateRange = 'all';
  selectedSurgeonType = 'all';
  
  constructor(private glaucomaService: GlaucomaQaService) {}

  ngOnInit(): void {
    this.loadStatistics();
    this.loadIOPAnalysis();
    this.loadComplications();
    this.loadRecentProcedures();
  }

  loadStatistics(): void {
    this.loadingStats = true;
    this.errorStats = '';
    
    this.glaucomaService.getStatistics().subscribe({
      next: (data) => {
        this.statistics = data;
        this.loadingStats = false;
      },
      error: (error) => {
        this.errorStats = 'Failed to load statistics';
        console.error('Error loading statistics:', error);
        this.loadingStats = false;
      }
    });
  }

  loadIOPAnalysis(): void {
    this.loadingIOP = true;
    this.errorIOP = '';
    
    this.glaucomaService.getIOPAnalysis().subscribe({
      next: (data) => {
        this.iopAnalysis = data;
        this.loadingIOP = false;
      },
      error: (error) => {
        this.errorIOP = 'Failed to load IOP analysis';
        console.error('Error loading IOP analysis:', error);
        this.loadingIOP = false;
      }
    });
  }

  loadComplications(): void {
    this.loadingComplications = true;
    this.errorComplications = '';
    
    this.glaucomaService.getComplicationsAnalysis().subscribe({
      next: (data) => {
        this.complications = data;
        this.loadingComplications = false;
      },
      error: (error) => {
        this.errorComplications = 'Failed to load complications data';
        console.error('Error loading complications:', error);
        this.loadingComplications = false;
      }
    });
  }

  loadRecentProcedures(): void {
    this.loadingProcedures = true;
    this.errorProcedures = '';
    
    this.glaucomaService.getAllRecords({ page: 1, pageSize: 10 }).subscribe({
      next: (response) => {
        this.recentProcedures = response.data;
        this.loadingProcedures = false;
      },
      error: (error) => {
        this.errorProcedures = 'Failed to load recent procedures';
        console.error('Error loading procedures:', error);
        this.loadingProcedures = false;
      }
    });
  }

  searchProcedures(): void {
    if (this.searchTerm.trim()) {
      this.loadingProcedures = true;
      this.glaucomaService.searchRecords(this.searchTerm, 1, 10).subscribe({
        next: (response) => {
          this.recentProcedures = response.data;
          this.loadingProcedures = false;
        },
        error: (error) => {
          this.errorProcedures = 'Failed to search procedures';
          console.error('Error searching procedures:', error);
          this.loadingProcedures = false;
        }
      });
    } else {
      this.loadRecentProcedures();
    }
  }

  refreshData(): void {
    this.loadStatistics();
    this.loadIOPAnalysis();
    this.loadComplications();
    this.loadRecentProcedures();
  }

  formatDate(date: Date | string | undefined): string {
    if (!date) return 'N/A';
    return new Date(date).toLocaleDateString();
  }

  getSuccessRate(): string {
    if (!this.statistics) return '0%';
    const totalRecords = this.statistics.totalRecords;
    const complicationsCount = this.statistics.recordsWithComplications || 0;
    const successRate = ((totalRecords - complicationsCount) / totalRecords) * 100;
    return `${successRate.toFixed(1)}%`;
  }

  getTopSurgeryTypes(): Array<{ surgeryType: string; count: number; percentage: number }> {
    if (!this.statistics?.surgeryTypeStats) return [];
    
    const total = this.statistics.totalRecords;
    return this.statistics.surgeryTypeStats
      .map(item => ({
        ...item,
        percentage: (item.count / total) * 100
      }))
      .slice(0, 5);
  }

  getTopComplications(): Array<{ complicationType: string; count: number }> {
    if (!this.complications?.postOperativeComplications) return [];
    return this.complications.postOperativeComplications.slice(0, 5);
  }
}