import { TestBed } from '@angular/core/testing';

import { ApiDataService } from './api.service';

describe('ApiService', () => {
  let service: ApiDataService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ApiDataService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});