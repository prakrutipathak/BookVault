import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { LocalstorageService } from '../services/helpers/localstorage.service';
import { LocalStorageKeys } from '../services/helpers/localstoragekeys';
import { Observable, map } from 'rxjs';

export const adminGuard: CanActivateFn = (route, state) => {

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
      if (response.data && response.data.isAdmin) {
        return true;
      } else {
        router.navigate(['/home']);
        return false;
      }
    })
  ) as Observable<boolean>;

};
