import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  imports: [CommonModule, RouterModule],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css'
})
export class SidebarComponent {
  @Input() activeRoute: string = '';
  isExpanded = false;
  tooltipText = '';
  tooltipTop = 0;

  constructor(private router: Router) {}

  expandSidebar() {
    this.isExpanded = true;
  }

  collapseSidebar() {
    this.isExpanded = false;
  }

  navigateToHome() {
    this.router.navigate(['/home']);
  }

  navigateToGlaucoma() {
    this.router.navigate(['/glaucoma-dashboard']);
  }

  navigateToCataract() {
    this.router.navigate(['/cataract-dashboard']);
  }

  navigateToReports() {
    this.router.navigate(['/reports']);
  }

  navigateToSettings() {
    this.router.navigate(['/settings']);
  }

  logout() {
    console.log('Logging out...');
    if (confirm('Are you sure you want to logout?')) {
      this.router.navigate(['/login']);
    }
  }
}