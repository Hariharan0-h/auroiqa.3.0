// cataractdashboard.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { CataractQaService } from '../../services/cataract-qa.service';
import { PatientCataractQa, StatsResponse } from '../../models/data.models';

@Component({
  selector: 'app-cataractdashboard',
  imports: [CommonModule, SidebarComponent, FormsModule],
  templateUrl: './cataractdashboard.component.html',
  styleUrl: './cataractdashboard.component.css'
})
export class CataractdashboardComponent implements OnInit {
  activeRoute = 'cataract';
  
  // Data properties
  statistics: StatsResponse | null = null;
  recentProcedures: PatientCataractQa[] = [];
  
  // Loading states
  loadingStats = false;
  loadingProcedures = false;
  
  // Error states
  errorStats = '';
  errorProcedures = '';

  // Filter properties
  searchTerm = '';
  selectedDateRange = 'all';
  selectedSurgeonType = 'all';
  selectedEye = 'all';
  
  constructor(private cataractService: CataractQaService) {}

  ngOnInit(): void {
    this.loadStatistics();
    this.loadRecentProcedures();
  }

  loadStatistics(): void {
    this.loadingStats = true;
    this.errorStats = '';
    
    this.cataractService.getStatistics().subscribe({
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

  loadRecentProcedures(): void {
    this.loadingProcedures = true;
    this.errorProcedures = '';
    
    this.cataractService.getAllRecords({ page: 1, pageSize: 10 }).subscribe({
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
      this.cataractService.searchRecords(this.searchTerm, 1, 10).subscribe({
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

  filterByDateRange(): void {
    // Implementation for date range filtering
    let dateFrom: Date | undefined;
    let dateTo: Date | undefined;
    
    const now = new Date();
    
    switch (this.selectedDateRange) {
      case 'today':
        dateFrom = new Date(now.getFullYear(), now.getMonth(), now.getDate());
        break;
      case 'week':
        dateFrom = new Date(now.getTime() - 7 * 24 * 60 * 60 * 1000);
        break;
      case 'month':
        dateFrom = new Date(now.getFullYear(), now.getMonth(), 1);
        break;
      case 'quarter':
        const quarter = Math.floor(now.getMonth() / 3);
        dateFrom = new Date(now.getFullYear(), quarter * 3, 1);
        break;
      case 'year':
        dateFrom = new Date(now.getFullYear(), 0, 1);
        break;
    }

    if (dateFrom) {
      this.loadingProcedures = true;
      this.cataractService.getAllRecords({
        page: 1,
        pageSize: 10,
        surgeryDateFrom: dateFrom,
        surgeryDateTo: dateTo,
        surgeonType: this.selectedSurgeonType !== 'all' ? this.selectedSurgeonType : undefined,
        eye: this.selectedEye !== 'all' ? this.selectedEye : undefined
      }).subscribe({
        next: (response) => {
          this.recentProcedures = response.data;
          this.loadingProcedures = false;
        },
        error: (error) => {
          this.errorProcedures = 'Failed to filter procedures';
          console.error('Error filtering procedures:', error);
          this.loadingProcedures = false;
        }
      });
    } else {
      this.loadRecentProcedures();
    }
  }

  refreshData(): void {
    this.loadStatistics();
    this.loadRecentProcedures();
  }

  formatDate(date: Date | string | undefined): string {
    if (!date) return 'N/A';
    return new Date(date).toLocaleDateString();
  }

  getSuccessRate(): string {
    if (!this.statistics) return '0%';
    const totalRecords = this.statistics.totalRecords;
    const complicationsCount = this.getComplicationsCount();
    const successRate = ((totalRecords - complicationsCount) / totalRecords) * 100;
    return `${successRate.toFixed(1)}%`;
  }

  getComplicationsCount(): number {
    if (!this.recentProcedures) return 0;
    return this.recentProcedures.filter(p => 
      p.ifanypostopcomplication || 
      p.anesthesia_complication || 
      p.listofIntraopIntraopcomplication
    ).length;
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

  getEyeDistribution(): Array<{ eye: string; count: number; percentage: number }> {
    if (!this.statistics?.eyeStats) return [];
    
    const total = this.statistics.totalRecords;
    return this.statistics.eyeStats
      .map(item => ({
        ...item,
        percentage: (item.count / total) * 100
      }));
  }

  getSurgeonTypeDistribution(): Array<{ surgeonType: string; count: number; percentage: number }> {
    if (!this.statistics?.surgeonTypeStats) return [];
    
    const total = this.statistics.totalRecords;
    return this.statistics.surgeonTypeStats
      .map(item => ({
        ...item,
        percentage: (item.count / total) * 100
      }))
      .slice(0, 3);
  }

  getVisualAcuityImprovement(): { avgPreBCVA: string; avgPostBCVA: string; improvementRate: string } {
    if (!this.recentProcedures || this.recentProcedures.length === 0) {
      return { avgPreBCVA: 'N/A', avgPostBCVA: 'N/A', improvementRate: 'N/A' };
    }

    const validRecords = this.recentProcedures.filter(p => p.pre_BCVA && p.followupBCVA);
    
    if (validRecords.length === 0) {
      return { avgPreBCVA: 'N/A', avgPostBCVA: 'N/A', improvementRate: 'N/A' };
    }

    // Note: This is a simplified calculation. In reality, you'd need proper visual acuity parsing
    const improvementCount = validRecords.filter(p => {
      // Simplified improvement detection
      return p.followupBCVA && p.pre_BCVA && p.followupBCVA > p.pre_BCVA;
    }).length;

    const improvementRate = (improvementCount / validRecords.length) * 100;

    return {
      avgPreBCVA: 'Variable',
      avgPostBCVA: 'Improved',
      improvementRate: `${improvementRate.toFixed(1)}%`
    };
  }

  getTopIOLTypes(): Array<{ iolType: string; count: number }> {
    if (!this.recentProcedures) return [];
    
    const iolCounts: { [key: string]: number } = {};
    
    this.recentProcedures.forEach(p => {
      if (p.lensname) {
        iolCounts[p.lensname] = (iolCounts[p.lensname] || 0) + 1;
      }
    });

    return Object.entries(iolCounts)
      .map(([iolType, count]) => ({ iolType, count }))
      .sort((a, b) => b.count - a.count)
      .slice(0, 5);
  }
}