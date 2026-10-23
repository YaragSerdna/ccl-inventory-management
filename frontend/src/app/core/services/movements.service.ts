import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { InventoryMovement, RegisterMovement } from '../models/movement.models';

@Injectable({ providedIn: 'root' })
export class MovementsService {
    private readonly http = inject(HttpClient);
    private readonly apiUrl = `${environment.apiUrl}/Inventory`;

    getMovements(productId?: number): Observable<InventoryMovement[]> {

        let url = `${this.apiUrl}/movements`;

        if (productId) {
            url += `?productId=${productId}`;
        }

        return this.http.get<InventoryMovement[]>(url);
    }

    registerEntry(data: RegisterMovement): Observable<any> {
        return this.http.post(`${this.apiUrl}/entries`, data);
    }

    registerExit(data: RegisterMovement): Observable<any> {
        return this.http.post(`${this.apiUrl}/exits`, data);
    }
}
