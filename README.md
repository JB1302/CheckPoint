# 🎮 CheckPoint

<div align="center">

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)
![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core-MVC-5C2D91?style=for-the-badge&logo=dotnet)
![MongoDB](https://img.shields.io/badge/MongoDB-Driver-47A248?style=for-the-badge&logo=mongodb)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=for-the-badge&logo=bootstrap)
![SignalR](https://img.shields.io/badge/SignalR-Realtime-0C7CD5?style=for-the-badge)
![Licencia](https://img.shields.io/badge/Licencia-Uso%20Acad%C3%A9mico-orange?style=for-the-badge)

**Plataforma web para gestionar eventos competitivos gaming, comunidad e interacción social en tiempo real.**

</div>

---

## ✨ Resumen del proyecto

**CheckPoint** es una aplicación **ASP.NET Core MVC (.NET 8)** con base de datos **MongoDB** orientada a comunidades gamer. Permite:

- Crear y administrar eventos competitivos.
- Gestionar inscripciones y reglas por evento.
- Publicar contenido, comentar y reaccionar.
- Moderar con reportes y auditoría.
- Notificar en tiempo real con **SignalR**.

El proyecto combina:
- **Interfaz web MVC (Razor)** para usuarios finales.
- **API REST interna** para operaciones CRUD y consumo programático de entidades del dominio.

---

## 🧱 Arquitectura (alto nivel)

```text
Usuarios (navegador)
   │
   ├─ Vistas Razor + Bootstrap (UI)
   │
   ├─ Controladores MVC (flujo web)
   │
   ├─ Servicios de dominio (lógica de negocio)
   │
   ├─ MongoDB Driver
   │
   └─ MongoDB (CheckPointDB)

+ Canal en tiempo real (SignalR Hub: /notificationsHub)
+ API REST (/api/*) sobre los mismos servicios
```

---

## 🛠️ Stack tecnológico

### Backend
- **.NET 8 / ASP.NET Core MVC**
- **Autenticación por cookies**
- **Autorización por roles** (`Admin`, `Organizer`, `User`)
- **BCrypt.Net** para hash de contraseñas
- **SignalR** para notificaciones en tiempo real

### Persistencia
- **MongoDB** con `MongoDB.Driver`
- Contexto centralizado por colecciones (`ContextoMongoDb`)

### Frontend
- **Razor Views**
- **Bootstrap 5**
- JavaScript ligero para interacciones UI y refresh de notificaciones

### Dependencias principales
- `MongoDB.Driver`
- `BCrypt.Net-Next`
- `bootstrap` (paquete)
- CDN de Bootstrap y Microsoft SignalR en layout

---

## 📁 Estructura del proyecto

```text
CheckPoint/
├─ Program.cs                    # Configuración de DI, auth, rutas, seed de usuarios
├─ appsettings.json              # Configuración (MongoDB, logging)
├─ controllers/                  # MVC + API controllers
├─ Services/                     # Lógica de negocio por agregado
├─ Models/                       # Entidades de dominio (MongoDB/Bson)
├─ ViewModels/                   # Modelos para formularios y vistas
├─ Views/                        # Vistas Razor por módulo
├─ Hubs/                         # SignalR hubs
└─ wwwroot/                      # Assets estáticos (css/js)
```

---

## 🧩 Módulos funcionales

- 👤 **Usuarios y autenticación**
  - Login, logout, registro.
  - Gestión por admin.
  - Control de cuentas activas/inactivas.

- 🪪 **Perfiles**
  - Perfil público por usuario.
  - Bio, avatar, país, juegos favoritos.

- 🎮 **Juegos**
  - Catálogo de juegos.
  - Estado activo/inactivo.

- 🏆 **Eventos**
  - Creación/edición por organizadores/admin.
  - Búsqueda y filtros.
  - Vista de detalle con publicaciones/reglas/inscripciones.

- 📜 **Reglas de evento**
  - Reglas textuales, check-in, plataformas permitidas, rango mínimo.

- 📝 **Publicaciones y comentarios**
  - Feed asociado a eventos.
  - Comentarios moderables.

- ❤️ **Reacciones**
  - Reacciones tipo toggle por entidad (`targetId`, `targetType`).

- ✅ **Inscripciones**
  - Registro de usuarios a eventos.
  - Estados de inscripción (por ejemplo `Pending`, `Confirmed`).

- 🔔 **Notificaciones**
  - Persistencia de notificaciones por usuario.
  - Push en tiempo real vía SignalR.

- 🚨 **Reportes y auditoría**
  - Reportes de contenido/entidades.
  - Trazabilidad de acciones críticas.

---

## 🔌 API REST disponible

> Base local típica: `https://localhost:59446` o `http://localhost:59447`

La API está bajo prefijo `/api` y expone, entre otros:

| Recurso | Base route |
|---|---|
| Usuarios | `/api/usuarios` |
| Perfiles | `/api/perfiles` |
| Juegos | `/api/juegos` |
| Eventos | `/api/eventos` |
| Reglas | `/api/reglas` |
| Inscripciones | `/api/inscripciones` |
| Publicaciones | `/api/publicaciones` |
| Comentarios | `/api/comentarios` |
| Reacciones | `/api/reacciones` |
| Notificaciones | `/api/notificaciones` |

### Ejemplos rápidos

```bash
# Listar eventos
curl http://localhost:59447/api/eventos

# Juegos activos
curl http://localhost:59447/api/juegos/activos

# Reacciones de un target
curl "http://localhost:59447/api/reacciones/contar?targetId=<id>&targetType=Post"
```

---

## 🌐 ¿Qué APIs consume el proyecto?

### APIs/servicios externos de infraestructura
- **MongoDB** (conexión directa por driver):
  - `mongodb://localhost:27017`
  - DB: `CheckPointDB`

### Servicios frontend de terceros (CDN)
- Bootstrap CSS/JS
- Cliente JavaScript de SignalR

### APIs de negocio externas
- Actualmente, **no se observa consumo de APIs de negocio de terceros** (por ejemplo RAWG, Twitch, etc.).
- La API principal consumida por el propio frontend es la **API interna de CheckPoint** (`/api/*`) y acciones MVC del servidor.

---

## 🔐 Seguridad, autenticación y roles

- Autenticación basada en **cookies**.
- Login en: `/Users/Login`.
- Access denied en: `/Users/AccessDenied`.
- Contraseñas con hash **BCrypt**.
- Roles usados en control de acceso:
  - `Admin`
  - `Organizer`
  - `User`

---

## 🗃️ Modelo de datos (colecciones MongoDB)

Colecciones principales:
- `Users`
- `Profiles`
- `Games`
- `Events`
- `EventRules`
- `Registrations`
- `Posts`
- `Comments`
- `Reactions`
- `Notifications`
- `Reports`
- `AuditLogs`

---

## 🚀 Cómo ejecutar en local

### 1) Requisitos
- .NET SDK 8.0+
- MongoDB en local (puerto 27017 por defecto)

### 2) Clonar y restaurar

```bash
git clone https://github.com/JB1302/CheckPoint
cd CheckPoint
dotnet restore CheckPoint/CheckPoint.csproj
```

### 3) Configurar MongoDB

Editar `CheckPoint/appsettings.json` si hace falta:

```json
"MongoDB": {
  "ConnectionString": "mongodb://localhost:27017",
  "DatabaseName": "CheckPointDB"
}
```

### 4) Ejecutar

```bash
dotnet run --project CheckPoint/CheckPoint.csproj
```

### 5) Abrir en navegador
- `https://localhost:59446`
- `http://localhost:59447`

---

## 👥 Usuarios demo sembrados al iniciar

Si no existen, la app crea automáticamente:

| Rol | Email | Usuario | Password |
|---|---|---|---|
| Admin | `admin@checkpoint.local` | `admin` | `Admin123!` |
| Organizer | `organizador@checkpoint.local` | `organizador_demo` | `CheckPoint123!` |
| User | `usuario@checkpoint.local` | `usuario_demo` | `CheckPoint123!` |

> ⚠️ Recomendación: cambiar/retirar estas credenciales en entornos no locales.

---

## ✅ Estado actual y oportunidades de mejora

### Fortalezas
- Separación clara por capas (controllers/services/models).
- Dominio amplio para plataforma social + eventos.
- Soporte de notificaciones en tiempo real.
- API REST extensa para integraciones.

### Mejoras recomendadas
- Agregar **Swagger** para documentación automática.
- Incorporar **tests unitarios/integración**.
- Añadir validaciones más consistentes en todos los endpoints.
- Endurecer reglas de autorización en algunos endpoints API.

---
