# Prueba Técnica - MiniSistema de Gestión de Inventario

## Descripción del Proyecto

Crear una aplicación web para la gestión de inventario. La aplicación debe permitir el registro de la entrada y salida de productos, así como el manejo del inventario actual.

## Requisitos

1. **Backend (C# con PostgreSQL):**
   - Crear una base de datos PostgreSQL para almacenar la información de productos y movimientos de inventario.
   - Desarrollar una API RESTful utilizando C# que permita registrar la entrada y salida de productos, así como consultar el inventario actual.
   - Utilizar Entity Framework (o Dapper) para interactuar con la base de datos.
   - Implementar stored procedures en PostgreSQL para realizar operaciones específicas, como registrar movimientos de inventario y calcular el inventario actual.

2. **Frontend (JavaScript):**
   - Implementar la interfaz de usuario utilizando JavaScript.
   - La interfaz debe permitir registrar la entrada y salida de productos, así como mostrar el inventario actual.
   - Utilizar cualquier framework de JavaScript (por ejemplo, React, Angular, Vue.js) o Vanilla JavaScript para desarrollar el frontend.

## Detalles Técnicos

- La API RESTful debe seguir las mejores prácticas de diseño de API.
- La autenticación debe ser segura, preferiblemente utilizando tokens JWT.
- Se deben manejar adecuadamente los errores y excepciones tanto en el backend como en el frontend.
- Se debe implementar un diseño responsivo para garantizar una buena experiencia de usuario en dispositivos móviles y de escritorio (Puedes usar una plantilla OpenSource si lo prefieres).
- Los stored procedures en PostgreSQL deben ser utilizados para realizar operaciones específicas en la base de datos (Entradas, Salidas y Estado Actual de Inventario.).

## Entregables

- Código fuente del backend, frontend y script SQL.
- Capturas De Pantalla De La Aplicación.

## Evaluación

Serán evaluados los siguientes aspectos:

- Funcionalidad completa según los requisitos especificados.
- Calidad y claridad del código.
- Manejo adecuado de errores y excepciones.
- Seguridad en la autenticación.
- Diseño de la base de datos, incluyendo la implementación y uso de stored procedures.
- Experiencia del usuario.

## Observaciones

- El tiempo máximo de entrega de esta prueba técnica es de dos dias.
- Se valorará cualquier esfuerzo adicional para mejorar la aplicación más allá de los requisitos mínimos.
