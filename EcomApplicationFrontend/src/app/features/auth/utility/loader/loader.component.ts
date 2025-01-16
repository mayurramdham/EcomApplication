import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { interval, Observable } from 'rxjs';
import { LoaderService } from '../../../../core/services/loader.service';

@Component({
  selector: 'app-loader',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './loader.component.html',
  styleUrl: './loader.component.css',
})
export class LoaderComponent {
  isLoading!: Observable<boolean>;
  loadingValue: number = 0;
  private loadingService = inject(LoaderService);
  ngOnInit(): void {
    this.isLoading = this.loadingService.loading$;
  }
  simulateLoading(): void {
    const interval = setInterval(() => {
      if (this.loadingValue < 100) {
        this.loadingValue += 10;
      } else {
        clearInterval(interval);
      }
    }, 500);
  }
}
