import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { ProductsService } from '../../core/services/products.service';
import { Product, CreateProduct, UpdateProduct } from '../../core/models/product.models';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';



@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, RouterLink],
  templateUrl: './products.html',
  styleUrl: './products.scss'
})
export class Products implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly productsService = inject(ProductsService);

  products: Product[] = [];
  loading = false;
  saving = false;
  errorMessage = '';
  successMessage = '';
  showForm = false;
  editingProduct: Product | null = null;
  searchTerm = '';

  readonly productForm = this.fb.nonNullable.group({
    code: ['', [Validators.required, Validators.maxLength(50)]],
    name: ['', [Validators.required, Validators.maxLength(150)]],
    description: ['', [Validators.maxLength(500)]],
    unitPrice: [0, [Validators.required, Validators.min(0), Validators.max(9999999999.99)]]
  });

  ngOnInit(): void {
    this.loadProducts();
  }

  get filteredProducts(): Product[] {
    const term = this.searchTerm.trim().toLowerCase();

    if (!term) {
      return this.products;
    }

    return this.products.filter(product =>
      product.code.toLowerCase().includes(term) ||
      product.name.toLowerCase().includes(term)
    );
  }

  loadProducts(): void {
    this.loading = true;
    this.errorMessage = '';

    this.productsService.getAll().subscribe({
      next: products => {
        this.products = products;
        this.loading = false;
      },
      error: error => {
        this.errorMessage = this.getErrorMessage(error, 'No fue posible cargar los productos.');
        this.loading = false;
      }
    });
  }

  openCreateForm(): void {
    this.editingProduct = null;
    this.successMessage = '';
    this.errorMessage = '';

    this.productForm.reset({ code: '', name: '', description: '', unitPrice: 0 });

    this.showForm = true;
  }

  openEditForm(product: Product): void {
    this.editingProduct = product;
    this.successMessage = '';
    this.errorMessage = '';

    this.productForm.reset({
      code: product.code,
      name: product.name,
      description: product.description ?? '',
      unitPrice: product.unitPrice
    });

    this.showForm = true;
  }

  cancelForm(): void {
    this.showForm = false;
    this.editingProduct = null;
    this.productForm.reset();
  }

  saveProduct(): void {
    if (this.productForm.invalid || this.saving) {
      this.productForm.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.errorMessage = '';
    this.successMessage = '';

    const value = this.productForm.getRawValue();
    const description = value.description.trim() || null;

    if (this.editingProduct) {
      const payload: UpdateProduct = {
        code: value.code.trim(),
        name: value.name.trim(),
        description,
        unitPrice: Number(value.unitPrice)
      };

      this.productsService.update(this.editingProduct.id, payload)
        .subscribe({
          next: () => {
            this.saving = false;
            this.showForm = false;
            this.editingProduct = null;
            this.successMessage = 'Producto actualizado correctamente.';
            this.loadProducts();
          },
          error: error => this.handleSaveError(error)
        });
    } else {
      const payload: CreateProduct = {
        code: value.code.trim(),
        name: value.name.trim(),
        description,
        unitPrice: Number(value.unitPrice)
      };

      this.productsService.create(payload).subscribe({
        next: () => {
          this.saving = false;
          this.showForm = false;
          this.successMessage = 'Producto creado correctamente.';
          this.loadProducts();
        },
        error: error => this.handleSaveError(error)
      });
    }
  }

  deactivateProduct(product: Product): void {
    const confirmed = window.confirm(`¿Deseas desactivar el producto "${product.name}"?`);

    if (!confirmed) {
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';

    this.productsService.deactivate(product.id).subscribe({
      next: () => {
        this.successMessage = 'Producto desactivado correctamente.';
        this.loadProducts();
      },
      error: error => {
        this.errorMessage = this.getErrorMessage(error, 'No fue posible desactivar el producto.');
      }
    });
  }

  private handleSaveError(error: unknown): void {
    this.saving = false;
    this.errorMessage = this.getErrorMessage(error, 'No fue posible guardar el producto.');
  }

  private getErrorMessage(error: unknown, fallback: string): string {
    if (error instanceof HttpErrorResponse) {
      if (error.status === 0) {
        return 'No se pudo conectar con la API. Verifica que el backend esté ejecutándose.';
      }

      if (error.status === 401) {
        return 'Tu sesión expiró o no es válida. Inicia sesión nuevamente.';
      }

      if (error.status === 403) {
        return 'No tienes permisos para realizar esta operación.';
      }

      const apiMessage =
        error.error?.detail ??
        error.error?.message ??
        error.error?.title;

      if (typeof apiMessage === 'string' && apiMessage.trim()) {
        return apiMessage;
      }

      if (error.status === 409) {
        return 'Ya existe un producto con ese código.';
      }

      if (error.status === 400) {
        return 'Los datos enviados no son válidos. Revisa el formulario.';
      }
    }

    return fallback;
  }
}
