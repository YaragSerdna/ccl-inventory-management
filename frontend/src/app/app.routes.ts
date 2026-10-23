import { Routes } from '@angular/router';

import { Login } from './features/auth/login/login';
import { Dashboard } from './features/dashboard/dashboard';
import { authGuard } from './core/guards/auth.guard';
import { Products } from './features/products/products';
import { Movements } from './features/movements/movements';

export const routes: Routes = [
    {
        path: 'login',
        component: Login
    },
    {
        path: 'dashboard',
        component: Dashboard,
        canActivate: [authGuard]
    },
    {
        path: 'products',
        component: Products,
        canActivate: [authGuard]
    },
    {
        path: 'movements',
        component: Movements,
        canActivate: [authGuard]
    },
    {
        path: '',
        pathMatch: 'full',
        redirectTo: 'dashboard'
    },
    {
        path: '**',
        redirectTo: 'dashboard'
    }
];
