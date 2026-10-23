import { Component } from '@angular/core';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [],
  template: `
    <main class="dashboard">
      <h1>Panel de inventario</h1>
      <p>Has iniciado sesión correctamente.</p>
      <p>En este espacio construiremos el resumen del inventario.</p>
    </main>
  `,
  styles: [`
    .dashboard {
      max-width: 1100px;
      margin: 0 auto;
      padding: 32px 24px;
      color: #172b4d;
      font-family: "Segoe UI", Arial, sans-serif;
    }
  `]
})
export class Dashboard { }
