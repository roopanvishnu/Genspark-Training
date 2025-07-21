// import { Injectable } from '@angular/core';
// import { Actions, createEffect, ofType } from '@ngrx/effects';
// import * as AuthActions from './auth.actions';
// import { AuthService } from '../../services/auth.service'; // Create this service
// import { catchError, map, mergeMap, of } from 'rxjs';

// @Injectable()
// export class AuthEffects {

//   constructor(private actions$: Actions, private authService: AuthService) {}

//   login$ = createEffect(() =>
//     this.actions$.pipe(
//       ofType(AuthActions.login),
//       mergeMap(action =>
//         this.authService.login(action.email, action.password).pipe(
//           map(user => AuthActions.loginSuccess({ user })),
//           catchError(err => of(AuthActions.loginFailure({ error: err.message })))
//         )
//       )
//     )
//   );
// }
