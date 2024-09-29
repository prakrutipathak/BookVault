import { CanActivateFn, Router } from '@angular/router';
import { Observable, map } from 'rxjs';
import { LocalStorageKeys } from '../services/helpers/localstoragekeys';
import { AuthService } from '../services/auth.service';
import { LocalstorageService } from '../services/helpers/localstorage.service';
import { inject } from '@angular/core';

export const userGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const localStorageHelper = inject(LocalstorageService);
  const router = inject(Router);

  const userIdString = localStorageHelper.getItem(LocalStorageKeys.UserId);
  const userId = userIdString ? Number(userIdString) : undefined;

  if (!userId) {
    router.navigate(['/home']);
    return false;
  }

  return authService.getUserById(userId).pipe(
    map(response => {
      if (response.data && !response.data.isAdmin) {
        return true;
      } else {
        router.navigate(['/home']);
        return false;
      }
    })
  ) as Observable<boolean>;

};
