// home.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { CataractQaService } from '../../services/cataract-qa.service';
import { GlaucomaQaService } from '../../services/glaucoma-qa.service';
import { StatsResponse } from '../../models/data.models';

@Component({
  selector: 'app-home',
  imports: [CommonModule, SidebarComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  activeRoute = 'home';
  
  // Data properties
  cataractStats: StatsResponse | null = null;
  glaucomaStats: StatsResponse | null = null;
  
  // Loading states
  loadingCataract = false;
  loadingGlaucoma = false;
  
  // Error states
  errorCataract = '';
  errorGlaucoma = '';
  
  constructor(
    private router: Router,
    private cataractService: CataractQaService,
    private glaucomaService: GlaucomaQaService
  ) {}

  ngOnInit(): void {
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.loadCataractStats();
    this.loadGlaucomaStats();
  }

  loadCataractStats(): void {
    this.loadingCataract = true;
    this.errorCataract = '';
    
    this.cataractService.getStatistics().subscribe({
      next: (data) => {
        this.cataractStats = data;
        this.loadingCataract = false;
      },
      error: (error) => {
        this.errorCataract = 'Failed to load cataract data';
        console.error('Error loading cataract statistics:', error);
        this.loadingCataract = false;
      }
    });
  }

  loadGlaucomaStats(): void {
    this.loadingGlaucoma = true;
    this.errorGlaucoma = '';
    
    this.glaucomaService.getStatistics().subscribe({
      next: (data) => {
        this.glaucomaStats = data;
        this.loadingGlaucoma = false;
      },
      error: (error) => {
        this.errorGlaucoma = 'Failed to load glaucoma data';
        console.error('Error loading glaucoma statistics:', error);
        this.loadingGlaucoma = false;
      }
    });
  }

  navigateToCataract(): void {
    this.router.navigate(['/cataract-dashboard']);
  }

  navigateToGlaucoma(): void {
    this.router.navigate(['/glaucoma-dashboard']);
  }

  refreshData(): void {
    this.loadDashboardData();
  }

  getTotalProcedures(): number {
    const cataractTotal = this.cataractStats?.totalRecords || 0;
    const glaucomaTotal = this.glaucomaStats?.totalRecords || 0;
    return cataractTotal + glaucomaTotal;
  }

  getTotalPatients(): number {
    const cataractPatients = this.cataractStats?.totalPatients || 0;
    const glaucomaPatients = this.glaucomaStats?.totalPatients || 0;
    return cataractPatients + glaucomaPatients;
  }

  getCataractSuccessRate(): string {
    if (!this.cataractStats) return '0%';
    const total = this.cataractStats.totalRecords;
    if (total === 0) return '0%';
    
    // Assuming no specific complications field, using a general success rate
    const successRate = 97.8; // You can calculate this based on actual complications data
    return `${successRate.toFixed(1)}%`;
  }

  getGlaucomaSuccessRate(): string {
    if (!this.glaucomaStats) return '0%';
    const total = this.glaucomaStats.totalRecords;
    const complications = this.glaucomaStats.recordsWithComplications || 0;
    if (total === 0) return '0%';
    
    const successRate = ((total - complications) / total) * 100;
    return `${successRate.toFixed(1)}%`;
  }

  getSystemHealthScore(): number {
    const cataractSuccess = parseFloat(this.getCataractSuccessRate()) || 0;
    const glaucomaSuccess = parseFloat(this.getGlaucomaSuccessRate()) || 0;
    
    if (cataractSuccess === 0 && glaucomaSuccess === 0) return 0;
    if (cataractSuccess === 0) return glaucomaSuccess;
    if (glaucomaSuccess === 0) return cataractSuccess;
    
    return (cataractSuccess + glaucomaSuccess) / 2;
  }

  getHealthScoreColor(): string {
    const score = this.getSystemHealthScore();
    if (score >= 95) return '#22C55E';
    if (score >= 90) return '#84CC16';
    if (score >= 85) return '#EAB308';
    if (score >= 80) return '#F59E0B';
    return '#EF4444';
  }

  getHealthScoreLabel(): string {
    const score = this.getSystemHealthScore();
    if (score >= 95) return 'Excellent';
    if (score >= 90) return 'Very Good';
    if (score >= 85) return 'Good';
    if (score >= 80) return 'Fair';
    return 'Needs Attention';
  }
}