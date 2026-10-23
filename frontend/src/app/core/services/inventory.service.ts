import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { CurrentInventory } from '../models/inventory.models';

@Injectable({ providedIn: 'root' })
export class InventoryService {
    private readonly http = inject(HttpClient);
    private readonly apiUrl = `${environment.apiUrl}/Inventory`;

    getCurrentInventory(): Observable<CurrentInventory[]> {
        return this.http.get<CurrentInventory[]>(`${this.apiUrl}/current`);
    }
}
