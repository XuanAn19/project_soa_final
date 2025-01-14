import { Component } from '@angular/core';
import { LoadingService } from '../../services/loading.service';

@Component({
  selector: 'app-loading-spinner',
  template: `
    <div *ngIf="loading$ | async" class="loading-spinner">
      <mat-spinner></mat-spinner>
    </div>
  `,
  styleUrls: ['./loading-spinner.component.css'], // Corrected the property name
})
export class LoadingSpinnerComponent {
  loading$;

  constructor(private loadingService: LoadingService) {
      this.loading$ = this.loadingService.loading$;
  }
}
