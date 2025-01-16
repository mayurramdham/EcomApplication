import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LoaderService {
  constructor() {}
  private loadingSubject: BehaviorSubject<boolean> =
    new BehaviorSubject<boolean>(false);
  public loading$ = this.loadingSubject.asObservable(); // asObvservable subscribing it
  show(): void {
    this.loadingSubject.next(true);
  }
  hide(): void {
    this.loadingSubject.next(false);
  }
}
