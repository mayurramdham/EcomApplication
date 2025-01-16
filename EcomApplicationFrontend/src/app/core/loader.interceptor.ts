import { HttpInterceptorFn } from '@angular/common/http';
import { LoaderService } from './services/loader.service';
import { inject } from '@angular/core';
import { finalize } from 'rxjs';

export const loaderInterceptor: HttpInterceptorFn = (req, next) => {
  const loadingServies = inject(LoaderService);
  loadingServies.show();
  return next(req).pipe(
    finalize(() => {
      loadingServies.hide();
    })
  );
};
