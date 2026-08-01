# AirportWebApp

AirportWebApp es una aplicación web ASP.NET Core MVC para gestionar vuelos, reservas y pagos con integración de Identity, recuperación de contraseña y localización en español.

## Descripción del ejercicio asignado

**Tipo 2: Reserva y selección de asientos**

La aplicación permite al usuario:

- Buscar un vuelo.
- Consultar la aeronave asignada.
- Mostrar los asientos disponibles.
- Filtrar por clase.
- Seleccionar un asiento.
- Evitar reservar un asiento ocupado.
- Crear una reserva temporal.
- Calcular el precio.
- Procesar el pago.
- Confirmar la asignación.
- Mostrar el resumen de la reserva.

## Características principales

- Autenticación con ASP.NET Core Identity.
- Registro y login con diseño consistente.
- Recuperación y reinicio de contraseña por correo.
- Roles `Administrador` y `Cliente`.
- Validación y mensajes en español.
- Pasarela de pago PayPal.
- SMTP configurable para envío de correos.
- Gestión de reservas, asientos y pagos.

## Contenido del repositorio

- `AirportWebApp.sln`: solución de Visual Studio / .NET.
- `AirportWebApp.csproj`: proyecto web ASP.NET Core.
- `Migrations/`: migraciones de Entity Framework Core.
- `README.md`: documentación del proyecto.
- `appsettings.Example.json`: ejemplo de configuración.
- `Areas/Identity/`: páginas de Identity personalizadas.
- `Views/Flights/`: vistas de búsqueda, selección y pago.
- `Services/`: servicios de PayPal, SMTP y localización de errores.
- `Controllers/FlightsController.cs`: lógica de reserva, pago y confirmación.

## Dependencias

- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` v10.0.10
- `Microsoft.AspNetCore.Identity.UI` v10.0.10
- `Microsoft.EntityFrameworkCore.Design` v10.0.10
- `Microsoft.EntityFrameworkCore.SqlServer` v10.0.10
- `Microsoft.EntityFrameworkCore.Tools` v10.0.10
- `Npgsql.EntityFrameworkCore.PostgreSQL` v10.0.3
- `PayPalCheckoutSdk` v1.0.4

## Configuración necesaria

El proyecto utiliza PostgreSQL como base de datos y PayPal para la pasarela de pagos. Para ejecutar correctamente la aplicación necesitas:

- Cadena de conexión en `ConnectionStrings:DefaultConnection`.
- Configuración SMTP:
  - `Smtp:Host`
  - `Smtp:Port`
  - `Smtp:User`
  - `Smtp:Password`
  - `Smtp:From`
- Configuración PayPal:
  - `PayPal:ClientId`
  - `PayPal:ClientSecret`
  - `PayPal:Mode`

### Ejemplo de configuración

El archivo de ejemplo `appsettings.Example.json` incluye estas claves sin datos sensibles.

```json
{
  "Smtp": {
    "Host": "smtp.example.com",
    "Port": "587",
    "User": "your-smtp-user@example.com",
    "Password": "your-smtp-password",
    "From": "no-reply@example.com",
    "EnableSsl": "true"
  },
  "PayPal": {
    "ClientId": "YOUR_PAYPAL_CLIENT_ID",
    "ClientSecret": "YOUR_PAYPAL_CLIENT_SECRET",
    "Mode": "sandbox"
  }
}
```

### Uso de User Secrets

En desarrollo se recomienda usar User Secrets para no guardar datos sensibles en el repositorio.

```powershell
cd C:\Users\DETPC\Desktop\AirportWebApp
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=demo;Username=postgres;Password=..."
dotnet user-secrets set "Smtp:Host" "smtp.example.com"
dotnet user-secrets set "Smtp:Port" "587"
dotnet user-secrets set "Smtp:User" "usuario@example.com"
dotnet user-secrets set "Smtp:Password" "contraseña"
dotnet user-secrets set "Smtp:From" "no-reply@example.com"
dotnet user-secrets set "PayPal:ClientId" "YOUR_PAYPAL_CLIENT_ID"
dotnet user-secrets set "PayPal:ClientSecret" "YOUR_PAYPAL_CLIENT_SECRET"
dotnet user-secrets set "PayPal:Mode" "sandbox"
```

## Ejecución

```powershell
dotnet restore
dotnet build
dotnet run
```

Luego abre `https://localhost:5001` o la URL indicada por la aplicación.

## Usuario administrador inicial

- Email: `admin@airportwebapp.local`
- Contraseña: `Admin123!`

## Migraciones

La carpeta `Migrations/` contiene las migraciones necesarias para crear las tablas de vuelos, reservas, pedidos y pagos.

## Capturas principales

Se sugiere agregar capturas de pantalla en la carpeta `docs/screenshots/` para mostrar:

- Búsqueda de vuelo.
- Selección de asiento.
- Resumen de reserva.
- Pago con PayPal.
- Confirmación de reserva.

Actualmente esta carpeta incluye una plantilla para las capturas.

## Notas adicionales

- El proyecto usa `AirportWebApp.sln` como archivo de solución.
- La navegación y la UI de Identity están traducidas al español.
- La aplicación protege roles y muestra solo las operaciones del cliente a cada usuario.
