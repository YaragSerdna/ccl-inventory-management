import { CommonModule, CurrencyPipe } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

import { AuthService } from '../../core/services/auth.service';
import { InventoryService } from '../../core/services/inventory.service';
import { CurrentInventory } from '../../core/models/inventory.models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, CurrencyPipe, RouterLink],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard implements OnInit {
  private readonly inventoryService = inject(InventoryService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  inventory: CurrentInventory[] = [];
  loading = true;
  errorMessage = '';

  ngOnInit(): void { this.loadInventory(); }

  loadInventory(): void {
    this.loading = true;
    this.errorMessage = '';

    this.inventoryService.getCurrentInventory().subscribe({
      next: (data) => {
        this.inventory = data;
        this.loading = false;
      },
      error: (error) => {
        this.loading = false;

        if (error.status === 401) {
          this.authService.logout();
          void this.router.navigateByUrl('/login');
          return;
        }

        this.errorMessage = error.status === 0
          ? 'No fue posible conectar con el servidor.'
          : 'No fue posible cargar el inventario. Intenta nuevamente.';
      }
    });
  }

  get totalProducts(): number {
    return this.inventory.length;
  }

  get totalUnits(): number {
    return this.inventory.reduce((total, item) => total + item.stock, 0);
  }

  get totalValue(): number {
    return this.inventory.reduce((total, item) => total + item.inventoryValue, 0);
  }

  get lowStockProducts(): number {
    return this.inventory.filter(item => item.stock > 0 && item.stock <= 5).length;
  }

  get outOfStockProducts(): number {
    return this.inventory.filter(item => item.stock === 0).length;
  }
  
  logout(): void {
    this.authService.logout();
    void this.router.navigateByUrl('/login');
  }
}
