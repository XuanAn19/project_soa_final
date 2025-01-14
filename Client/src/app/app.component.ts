import { Component, OnInit } from '@angular/core';
import { Router, NavigationStart, NavigationEnd, NavigationError } from '@angular/router';
import { LoadingService } from './services/loading.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  constructor(private router: Router, private loadingService: LoadingService) {}

  ngOnInit() {
    this.router.events.subscribe((event) => {
      if (event instanceof NavigationStart) {
        // Start loading when navigating to a new route
        this.loadingService.startLoading();
      }

      if (event instanceof NavigationEnd || event instanceof NavigationError) {
        // Stop loading when navigation ends or encounters an error
        this.loadingService.stopLoading();
      }
    });
  }
}
