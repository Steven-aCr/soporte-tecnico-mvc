/* =====================================================================
   Script: 01_CrearSoporteTecnicoDB.sql
   Base de datos: SoporteTecnicoDB (SQL Server)
   ===================================================================== */

CREATE DATABASE SoporteTecnicoDB;
GO

USE SoporteTecnicoDB;
GO

/* ---------- Rol ---------- */
CREATE TABLE dbo.Rol
(
    IdRol  INT IDENTITY(1,1) NOT NULL,
    Nombre NVARCHAR(30)      NOT NULL,
    CONSTRAINT PK_Rol        PRIMARY KEY (IdRol),
    CONSTRAINT UQ_Rol_Nombre UNIQUE (Nombre)
);
GO

/* ---------- Usuario ---------- */
CREATE TABLE dbo.Usuario
(
    IdUsuario    INT IDENTITY(1,1) NOT NULL,
    Nombre       NVARCHAR(100)     NOT NULL,
    Correo       NVARCHAR(150)     NOT NULL,
    PasswordHash NVARCHAR(512)     NOT NULL,
    Activo       BIT               NOT NULL CONSTRAINT DF_Usuario_Activo DEFAULT (1),
    IdRol        INT               NOT NULL,
    CONSTRAINT PK_Usuario        PRIMARY KEY (IdUsuario),
    CONSTRAINT UQ_Usuario_Correo UNIQUE (Correo),
    CONSTRAINT FK_Usuario_Rol    FOREIGN KEY (IdRol) REFERENCES dbo.Rol (IdRol)
);
GO

/* ---------- Categoria ---------- */
CREATE TABLE dbo.Categoria
(
    IdCategoria INT IDENTITY(1,1) NOT NULL,
    Nombre      NVARCHAR(60)      NOT NULL,
    Descripcion NVARCHAR(250)     NULL,
    Activo      BIT               NOT NULL CONSTRAINT DF_Categoria_Activo DEFAULT (1),
    CONSTRAINT PK_Categoria        PRIMARY KEY (IdCategoria),
    CONSTRAINT UQ_Categoria_Nombre UNIQUE (Nombre)
);
GO

/* ---------- Ticket ---------- */
CREATE TABLE dbo.Ticket
(
    IdTicket      INT IDENTITY(1,1) NOT NULL,
    Titulo        NVARCHAR(150)     NOT NULL,
    Descripcion   NVARCHAR(2000)    NOT NULL,
    Prioridad     INT               NOT NULL CONSTRAINT DF_Ticket_Prioridad DEFAULT (2),
    Estado        INT               NOT NULL CONSTRAINT DF_Ticket_Estado DEFAULT (1),
    FechaCreacion DATETIME2(0)      NOT NULL CONSTRAINT DF_Ticket_FechaCreacion DEFAULT (SYSUTCDATETIME()),
    FechaCierre   DATETIME2(0)      NULL,
    Solucion      NVARCHAR(2000)    NULL,
    IdCategoria   INT               NOT NULL,
    IdSolicitante INT               NOT NULL,
    IdTecnico     INT               NULL,
    CONSTRAINT PK_Ticket PRIMARY KEY (IdTicket),
    CONSTRAINT FK_Ticket_Categoria   FOREIGN KEY (IdCategoria)   REFERENCES dbo.Categoria (IdCategoria),
    CONSTRAINT FK_Ticket_Solicitante FOREIGN KEY (IdSolicitante) REFERENCES dbo.Usuario (IdUsuario),
    CONSTRAINT FK_Ticket_Tecnico     FOREIGN KEY (IdTecnico)     REFERENCES dbo.Usuario (IdUsuario),

    -- Prioridad: 1=Baja, 2=Media, 3=Alta
    CONSTRAINT CK_Ticket_Prioridad CHECK (Prioridad BETWEEN 1 AND 3),
    -- Estado: 1=Pendiente, 2=EnProceso, 3=Resuelto, 4=Cerrado
    CONSTRAINT CK_Ticket_Estado CHECK (Estado BETWEEN 1 AND 4),
    -- Estado distinto de Pendiente requiere técnico
    CONSTRAINT CK_Ticket_TecnicoRequerido CHECK (Estado = 1 OR IdTecnico IS NOT NULL),
    -- Resuelto o Cerrado requieren solución no vacía
    CONSTRAINT CK_Ticket_SolucionRequerida CHECK
        (Estado NOT IN (3, 4) OR (Solucion IS NOT NULL AND LEN(LTRIM(RTRIM(Solucion))) > 0)),
    -- Cerrado: FechaCierre >= FechaCreacion; otros estados: FechaCierre NULL
    CONSTRAINT CK_Ticket_FechaCierre CHECK
        ((Estado = 4 AND FechaCierre IS NOT NULL AND FechaCierre >= FechaCreacion)
         OR (Estado <> 4 AND FechaCierre IS NULL))
);
GO

/* ---------- Comentario ---------- */
CREATE TABLE dbo.Comentario
(
    IdComentario  INT IDENTITY(1,1) NOT NULL,
    Contenido     NVARCHAR(2000)    NOT NULL,
    FechaCreacion DATETIME2(0)      NOT NULL CONSTRAINT DF_Comentario_FechaCreacion DEFAULT (SYSUTCDATETIME()),
    IdTicket      INT               NOT NULL,
    IdUsuario     INT               NOT NULL,
    CONSTRAINT PK_Comentario PRIMARY KEY (IdComentario),
    CONSTRAINT FK_Comentario_Ticket  FOREIGN KEY (IdTicket)  REFERENCES dbo.Ticket (IdTicket),
    CONSTRAINT FK_Comentario_Usuario FOREIGN KEY (IdUsuario) REFERENCES dbo.Usuario (IdUsuario),
    -- El contenido no puede estar vacío
    CONSTRAINT CK_Comentario_Contenido CHECK (LEN(LTRIM(RTRIM(Contenido))) > 0)
);
GO

/* ---------- HistorialEstado ---------- */
CREATE TABLE dbo.HistorialEstado
(
    IdHistorialEstado INT IDENTITY(1,1) NOT NULL,
    EstadoAnterior    INT               NULL,
    EstadoNuevo       INT               NOT NULL,
    FechaCambio       DATETIME2(0)      NOT NULL CONSTRAINT DF_Historial_FechaCambio DEFAULT (SYSUTCDATETIME()),
    IdTicket          INT               NOT NULL,
    IdUsuario         INT               NOT NULL,
    CONSTRAINT PK_HistorialEstado PRIMARY KEY (IdHistorialEstado),
    CONSTRAINT FK_Historial_Ticket  FOREIGN KEY (IdTicket)  REFERENCES dbo.Ticket (IdTicket),
    CONSTRAINT FK_Historial_Usuario FOREIGN KEY (IdUsuario) REFERENCES dbo.Usuario (IdUsuario),

    -- Valores válidos del 1 al 4
    CONSTRAINT CK_Historial_Rango CHECK
        (EstadoNuevo BETWEEN 1 AND 4 AND (EstadoAnterior IS NULL OR EstadoAnterior BETWEEN 1 AND 4)),
    -- Anterior NULL = creación inicial (nuevo = 1); en otros casos deben diferir
    CONSTRAINT CK_Historial_Cambio CHECK
        ((EstadoAnterior IS NULL AND EstadoNuevo = 1)
         OR (EstadoAnterior IS NOT NULL AND EstadoAnterior <> EstadoNuevo))
);
GO

/* ---------- Datos iniciales opcionales: roles */
IF NOT EXISTS (SELECT 1 FROM dbo.Rol)
    INSERT INTO dbo.Rol (Nombre) VALUES (N'Administrador'), (N'Tecnico'), (N'Solicitante');
GO