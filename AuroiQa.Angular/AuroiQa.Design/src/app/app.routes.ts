import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { GlaucomadashboardComponent } from './components/glaucomadashboard/glaucomadashboard.component';
import { CataractdashboardComponent } from './components/cataractdashboard/cataractdashboard.component';
import { SettingsComponent } from './components/settings/settings.component';

export const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent },
  { path: 'glaucoma-dashboard', component: GlaucomadashboardComponent },
  { path: 'cataract-dashboard', component: CataractdashboardComponent },
  { path: 'reports', redirectTo: '/home', pathMatch: 'full' }, // Placeholder for reports
  { path: 'settings', component: SettingsComponent }, 
  { path: '**', redirectTo: '/home' }
];
