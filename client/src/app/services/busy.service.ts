import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class BusyService {
  busyRequestCount = 0;

  constructor(private spinnerService: NgxSpinnerService) { }

  busy() {
    this.busyRequestCount++;
    this.spinnerService.show(undefined, {
      type: 'square-jelly-box',
      color: '#5abff0',
      bdColor: 'rgba(0, 0, 0, 0.8)',
      fullScreen: true,
      size: 'large',

    });
  }

  idle() {
    this.busyRequestCount--;

    if (this.busyRequestCount < 0) {
      this.busyRequestCount = 0;
    }

    this.spinnerService.hide();
  }
}
