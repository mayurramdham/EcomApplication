import { TestBed } from '@angular/core/testing';

import { ToaterService } from './toater.service';

describe('ToaterService', () => {
  let service: ToaterService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ToaterService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
