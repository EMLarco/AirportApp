# Arquitectura

La aplicación utiliza ASP.NET Core MVC con Identity para autenticación, Entity Framework Core para datos, y PayPal para la pasarela de pago.

- `Areas/Identity`: UI y lógica de autenticación.
- `Data/AirportDbContext.cs`: contexto de base de datos.
- `Services/`: servicios de correo y pago.
