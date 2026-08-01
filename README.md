# AirportWebApp

AirportWebApp es una aplicación web ASP.NET Core MVC para gestionar vuelos, reservas y pagos con integración de Identity, recuperación de contraseña y localización en español.

## Características

- Autenticación mediante ASP.NET Core Identity.
- Login y registro con estilo del proyecto.
- Recuperación y reinicio de contraseña por correo.
- Control de acceso con páginas de bloqueo y acceso denegado.
- Gestión de cuentas: cambio de contraseña y datos básicos.
- Integración de PayPal para el pago de reservas.
- Soporte de roles `Administrador` y `Cliente`.
- Mensajes de validación y errores de Identity en español.
- Envío de correos con SMTP configurable.

## Estructura principal

- `Program.cs`: configuración de servicios, Identity, roles y servicios.
- `Data/AirportDbContext.cs`: contexto EF Core con tablas de aeropuerto y reservas.
- `Services/`: servicios de PayPal, SMTP y localización de errores.
- `Areas/Identity/Pages/Account/`: páginas de login, registro, recuperación y gestión.
- `Views/Flights/`: vistas de vuelos y reserva.
- `wwwroot/lib/jquery-validation/dist/localization/messages_es.js`: validación cliente en español.

## Seguridad y configuración

Esta aplicación usa User Secrets para las credenciales y datos sensibles. No se deben subir las configuraciones reales a GitHub.

- `appsettings.Example.json`: plantilla de configuración segura.
- `appsettings.json` y `appsettings.Development.json` no deben ser subidos.

### Configuración recomendada

Asegúrate de definir al menos:

- `ConnectionStrings:DefaultConnection`
- `Smtp:Host`, `Smtp:Port`, `Smtp:User`, `Smtp:Password`, `Smtp:From`
- `PayPal:ClientId`, `PayPal:ClientSecret`, `PayPal:Mode`

Puedes usar User Secrets en desarrollo:

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

## Usuario administrador inicial

- Email: `admin@airportwebapp.local`
- Contraseña: `Admin123!`

## Ejecución

```powershell
dotnet build
dotnet run
```

Luego abre `https://localhost:5001` o la URL indicada por la aplicación.
