# Soporte Técnico MVC

Sistema web para gestionar tickets de soporte técnico, desarrollado con **C#**, **ASP.NET Core MVC (10.0)** y **SQL Server** mediante una arquitectura en capas, con pruebas unitarias en **MSTest**.

> **Estado:** en desarrollo. Las funcionalidades descritas corresponden al alcance previsto.

## Contenido

- [Objetivo](#objetivo)
- [Tecnologías](#tecnologías)
- [Funcionalidades](#funcionalidades)
- [Roles](#roles)
- [Arquitectura](#arquitectura)
- [Estructura del repositorio](#estructura-del-repositorio)
- [Base de datos](#base-de-datos)
- [Flujo de atención](#flujo-de-atención)
- [Reglas de negocio](#reglas-de-negocio)
- [Puesta en marcha](#puesta-en-marcha)
- [Pruebas unitarias](#pruebas-unitarias)
- [Alcance de la primera versión](#alcance-de-la-primera-versión)
- [Contexto académico](#contexto-académico)

## Objetivo

Centralizar las solicitudes de soporte técnico, facilitar la asignación de responsables y registrar el seguimiento de cada incidencia hasta su resolución y cierre.

## Tecnologías

| Área | Tecnología |
|---|---|
| Lenguaje y framework | C#, ASP.NET Core MVC (10.0), vistas Razor |
| Acceso a datos | Entity Framework Core, SQL Server |
| Interfaz | HTML, CSS |
| Pruebas | MSTest |
| Herramientas | Visual Studio 2026, Git, GitHub |

La versión de .NET se define en los archivos `.csproj` de la solución.

## Funcionalidades

- Inicio y cierre de sesión.
- Control de acceso por roles.
- Gestión de usuarios y categorías.
- Creación y consulta de tickets.
- Asignación de técnicos.
- Seguimiento de estados y prioridades.
- Comentarios y registro de soluciones.
- Historial de cambios de estado.
- Búsqueda y filtros de tickets.
- Panel con indicadores.
- Pruebas unitarias de las reglas de negocio.

## Roles

| Rol | Funciones |
|---|---|
| Administrador | Gestiona usuarios y categorías, asigna técnicos, consulta todos los tickets y cierra solicitudes resueltas. |
| Técnico | Consulta y atiende los tickets asignados, agrega comentarios y registra soluciones. |
| Solicitante | Crea tickets, consulta sus propias solicitudes, agrega comentarios y cierra sus tickets resueltos. |

## Arquitectura

La solución se organiza en cinco proyectos:

| Proyecto | Responsabilidad |
|---|---|
| `SoporteTecnico.EN` | Entidades y enumeraciones. |
| `SoporteTecnico.DAL` | Acceso a SQL Server, interfaces de acceso a datos y configuración de Entity Framework Core. |
| `SoporteTecnico.BL` | Validaciones y reglas de negocio. |
| `SoporteTecnico.Web` | Controladores, ViewModels, vistas Razor y configuración de la aplicación. |
| `SoporteTecnico.Tests` | Pruebas unitarias de las reglas de negocio. |

Flujo de una operación:

```text
Vista → Controlador → BL → DAL → SQL Server
```

Los controladores utilizan la capa BL, que recibe las dependencias de acceso a datos mediante interfaces. Esto permite sustituirlas por implementaciones simuladas en las pruebas.

### Referencias entre proyectos

| Proyecto | Referencia a |
|---|---|
| EN | Ninguno |
| DAL | EN |
| BL | EN, DAL |
| Web | EN, BL, DAL |
| Tests | EN, BL, DAL |

Web referencia a DAL únicamente para configurar el contexto y registrar las dependencias en `Program.cs`. Las operaciones de negocio se realizan a través de BL.

## Estructura del repositorio

```text
SoporteTecnico/
├── README.md
├── .gitignore
├── DiagramaBD.puml
├── SoporteTecnicoDB.sql
├── SoporteTecnico.slnx
├── SoporteTecnico.EN/
│   ├── Entidades/
│   └── Enumeraciones/
├── SoporteTecnico.DAL/
│   ├── Contexto/
│   ├── Interfaces/
│   └── Repositorios/
├── SoporteTecnico.BL/
│   └── Servicios/
├── SoporteTecnico.Web/
│   ├── Controllers/
│   ├── ViewModels/
│   ├── Views/
│   ├── wwwroot/
│   ├── appsettings.json
│   └── Program.cs
└── SoporteTecnico.Tests/
    ├── Servicios/
    └── Dobles/
```

> El archivo de solución puede ser `.sln` o `.slnx`, según el formato utilizado.

## Base de datos

Nombre: **SoporteTecnicoDB**.

| Tabla | Propósito |
|---|---|
| `Rol` | Roles disponibles. |
| `Usuario` | Usuarios, credenciales y rol. |
| `Categoria` | Clasificación de las solicitudes. |
| `Ticket` | Incidencia, prioridad, estado, solicitante, técnico y solución. |
| `Comentario` | Mensajes asociados a cada ticket. |
| `HistorialEstado` | Creación y cambios de estado de los tickets. |

### Relaciones principales

- Un rol puede pertenecer a varios usuarios.
- Una categoría puede agrupar varios tickets.
- Un usuario puede crear varios tickets y un técnico puede tener varios tickets asignados.
- Un ticket puede tener varios comentarios y varios registros de historial.
- Cada comentario y cada cambio de estado identifica al usuario responsable.

### Estados y prioridades

| Valor | Estado | | Valor | Prioridad |
|:---:|---|---|:---:|---|
| 1 | Pendiente | | 1 | Baja |
| 2 | EnProceso | | 2 | Media |
| 3 | Resuelto | | 3 | Alta |
| 4 | Cerrado | | | |

Estos valores deben coincidir con las enumeraciones definidas en C#.

### Fechas

Las fechas se almacenan en **UTC** y se convierten a la hora local al presentarlas al usuario.

## Flujo de atención

1. El solicitante crea un ticket con título, descripción, categoría y prioridad.
2. El sistema lo registra en estado **Pendiente**.
3. El administrador asigna un técnico activo.
4. El técnico inicia la atención y cambia el estado a **EnProceso**.
5. Los participantes agregan comentarios de seguimiento.
6. El técnico registra la solución y cambia el estado a **Resuelto**.
7. El solicitante propietario o el administrador cambia el estado a **Cerrado**.

La creación y cada cambio de estado quedan registrados en el historial.

## Reglas de negocio

**Acceso**

- El solicitante solo puede consultar y comentar sus propios tickets.
- El técnico solo puede atender y comentar los tickets asignados a él.
- El administrador puede consultar todos los tickets.
- Los permisos se validan en el servidor, además de controlar las opciones visibles en las vistas.

**Tickets y estados**

- Solo se pueden asignar usuarios activos con rol Técnico.
- Los nuevos tickets deben usar una categoría activa.
- Para pasar a EnProceso, el ticket debe tener un técnico asignado.
- Para pasar a Resuelto, debe registrarse una solución.
- Solo se pueden cerrar tickets resueltos.
- Los tickets cerrados no admiten nuevos comentarios.
- Cada cambio de estado genera un registro de historial, y ambos se guardan en una misma transacción.

**Datos**

- Los usuarios y categorías con registros relacionados se desactivan en lugar de eliminarse.
- Los correos de usuarios y los nombres de categorías son únicos.
- Las contraseñas se almacenan como hashes, nunca como texto plano.

## Puesta en marcha

> Estas instrucciones aplican cuando la configuración y los módulos necesarios estén implementados.

### Requisitos

- Visual Studio 2026 con la carga de trabajo *ASP.NET y desarrollo web*.
- SDK de .NET compatible con los archivos `.csproj`.
- SQL Server y SQL Server Management Studio (u otro cliente compatible).
- Git.

### Instalación

1. Clonar el repositorio.
2. Abrir `SoporteTecnico.slnx` (o el archivo de solución disponible).
3. Restaurar los paquetes NuGet.
4. Ejecutar `SoporteTecnicoDB.sql` en SQL Server.
5. Configurar la cadena de conexión.
6. Establecer `SoporteTecnico.Web` como proyecto de inicio.
7. Compilar y ejecutar la aplicación.

### Cadena de conexión

Agregar la sección `ConnectionStrings` a `SoporteTecnico.Web/appsettings.json`, conservando las demás configuraciones. Ejemplo para una instancia local con autenticación de Windows:

```json
{
  "ConnectionStrings": {
    "SoporteTecnicoDB": "Server=localhost;Database=SoporteTecnicoDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

- El nombre `SoporteTecnicoDB` debe coincidir con el usado en `Program.cs`.
- Ajustar `Server` según la instancia instalada.
- `TrustServerCertificate=True` es adecuado solo para desarrollo local.
- **No publicar credenciales en el repositorio.** Si la conexión requiere usuario y contraseña, usar secretos de usuario o variables de entorno.

### Gestión del esquema

El script SQL crea las tablas y sus restricciones. Si se usa para preparar la base de datos, la configuración de Entity Framework Core debe respetar el esquema existente: **no ejecutar una migración inicial** que intente crear de nuevo las mismas tablas.

El script debe ejecutarse sobre una base sin las tablas del sistema; si detecta tablas existentes, detiene la ejecución para no sobrescribir información.

### Datos semilla

| Datos | Cantidad |
|---|:---:|
| Roles | 3 |
| Usuarios | 5 |
| Categorías | 5 |
| Tickets | 4 |
| Comentarios | 5 |
| Registros de historial | 10 |

Los cuatro tickets de ejemplo cubren los cuatro estados previstos.

| Correo | Rol |
|---|---|
| `admin@soporte.test` | Administrador |
| `carlos@soporte.test` | Técnico |
| `ana@soporte.test` | Técnico |
| `maria@soporte.test` | Solicitante |
| `jose@soporte.test` | Solicitante |

> **Importante:** estos usuarios tienen el marcador `HASH_PENDIENTE` en `PasswordHash`, que no es un hash válido y no permite iniciar sesión. Antes de probar la autenticación, reemplazarlo por hashes generados con el mismo mecanismo que usa la aplicación.

## Pruebas unitarias

`SoporteTecnico.Tests` utiliza MSTest para verificar las reglas de negocio de forma aislada. Las pruebas **no dependen de SQL Server**: las interfaces de acceso a datos se sustituyen por dobles de prueba que controlan los datos y resultados.

Las pruebas siguen el patrón **Arrange, Act, Assert**: preparar datos y dependencias simuladas, ejecutar el método y comprobar el resultado o la excepción esperada.

### Organización

| Clase | Casos previstos |
|---|---|
| `UsuarioBLTests` | Datos obligatorios, correo duplicado y desactivación de usuarios. |
| `CategoriaBLTests` | Nombre obligatorio, nombre duplicado y desactivación de categorías. |
| `TicketBLTests` | Creación, permisos, asignación de técnicos y transiciones de estado. |
| `ComentarioBLTests` | Contenido obligatorio, permisos y restricciones de tickets cerrados. |

### Casos principales

- Crear un ticket con datos válidos.
- Rechazar un ticket sin título o descripción.
- Rechazar la asignación de un usuario que no sea técnico o de un técnico inactivo.
- Impedir iniciar la atención sin técnico asignado.
- Impedir resolver un ticket sin solución.
- Impedir cerrar un ticket pendiente.
- Impedir consultar tickets sin autorización.
- Rechazar comentarios vacíos y comentarios en tickets cerrados.
- Verificar que el cambio de estado solicite el registro del historial.

La persistencia real y la atomicidad de las transacciones se verifican con pruebas de integración, separadas de las unitarias.

### Ejecución

**Visual Studio:** abrir *Prueba → Explorador de pruebas*, compilar la solución y seleccionar *Ejecutar todas las pruebas*.

**Terminal**, desde la carpeta de la solución:

```bash
dotnet test
```

## Alcance de la primera versión

Incluye la gestión básica de tickets, el control por roles y las pruebas unitarias de las reglas de negocio.

Reservado para futuras versiones:

- Archivos adjuntos.
- Notificaciones por correo.
- Chat en tiempo real.
- Recuperación de contraseñas.
- Acuerdos de nivel de servicio y tiempos de atención.
- Reportes exportables.

## Contexto académico

Proyecto desarrollado como práctica de arquitectura en capas, ASP.NET Core MVC, bases de datos relacionales y pruebas unitarias.