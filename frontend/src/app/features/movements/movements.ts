import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';

import { RouterLink } from '@angular/router';

import { ProductsService } from '../../core/services/products.service';
import { MovementsService } from '../../core/services/movements.service';

import { Product } from '../../core/models/product.models';
import { InventoryMovement } from '../../core/models/movement.models';

@Component({
  selector: 'app-movements',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterLink],
  templateUrl: './movements.html',
  styleUrl: './movements.scss'
})
export class Movements implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly productsService = inject(ProductsService);
  private readonly movementsService = inject(MovementsService);

  products: Product[] = [];
  movements: InventoryMovement[] = [];

  loading = false;
  saving = false;

  successMessage = '';
  errorMessage = '';

  movementType: 'ENTRY' | 'EXIT' = 'ENTRY';

  selectedProductFilter = 0;

  readonly movementForm = this.fb.nonNullable.group({
    productId: [0, Validators.min(1)],
    quantity: [1, Validators.min(1)],
    notes: ['']
  });

  ngOnInit(): void {
    this.loadProducts();
    this.loadMovements();
  }

  loadProducts() {
    this.productsService.getAll().subscribe({ next: products => this.products = products });
  }

  loadMovements() {
    this.loading = true;

    const filter = this.selectedProductFilter > 0
      ? this.selectedProductFilter
      : undefined;

    this.movementsService.getMovements(filter).subscribe({
      next: movements => {
        this.movements = movements;
        this.loading = false;
      },

      error: () => {
        this.errorMessage = 'No fue posible cargar los movimientos.';
        this.loading = false;
      }
    });

  }

  saveMovement() {
    if (this.movementForm.invalid) {
      this.movementForm.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.successMessage = '';
    this.errorMessage = '';

    const payload = this.movementForm.getRawValue();

    const request = this.movementType === 'ENTRY'
      ? this.movementsService.registerEntry(payload)
      : this.movementsService.registerExit(payload);

    request.subscribe({
      next: () => {
        this.successMessage = this.movementType === 'ENTRY'
          ? 'Entrada registrada correctamente.'
          : 'Salida registrada correctamente.';

        this.saving = false;

        this.movementForm.reset({ productId: 0, quantity: 1, notes: '' });

        this.loadProducts();
        this.loadMovements();

      },

      error: err => {
        this.saving = false;
        this.errorMessage = err.error?.message ?? 'No fue posible registrar el movimiento.';
      }
    });
  }
}
