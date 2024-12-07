-- --------------------------------------------------------------
-- CREACIÓN DE BASE DE DATOS
-- --------------------------------------------------------------

Create Database TechBoard
Go

Use TECH_BOARD
Go

-- --------------------------------------------------------------
-- SECCIÓN DE USUARIOS
-- --------------------------------------------------------------

-- Tabla de Roles
create table Roles
(
	Id_rol int identity (1,1) primary key NOT NULL,
	Nombre varchar(100) NOT NULL
)

-- Agregar roles
insert into Roles (Nombre)
values ('Admin'), ('Empleado'), ('Cliente')
select * from Roles

-- Tabla de Seguridad (para almacenar el PIN)
CREATE TABLE Seguridad (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Pin VARCHAR(10) NOT NULL
);

-- Insertar el PIN de seguridad
INSERT INTO Seguridad (Pin) VALUES ('1098');

-- Tabla de usuarios
create table USUARIO
(
	IdUsuario int primary key identity (1,1),
	Nombre varchar (100)NOT NULL,
	Correo varchar(100)NOT NULL,
	Clave varchar (100)NOT NULL,
	Rol int foreign key references Roles (Id_rol) default 3 NOT NULL
)

-- View de Usuarios para modelo de Visual Studio
create view v_Usuarios as Select
IdUsuario as 'ID',
USUARIO.Nombre as 'Nombre',
Correo as 'Correo',
Rol as 'Id_rol'
from USUARIO
Join Roles 
On USUARIO.Rol = Roles.Id_rol

-- Procedimiento almacenado para el registro de ususarios
create proc sp_RegistrarUsuario
(
	@Correo varchar (100),
	@Clave varchar (500),
	@Nombre varchar (100),
	@Registrado bit output,
	@Mensaje varchar (100) output
)
as
begin
	
		if (not exists (select*from USUARIO where Correo = @Correo))
		begin
			insert into USUARIO (Correo, Clave, Nombre) values (@Correo,@Clave,@Nombre)
			set @Registrado = 1
			set @Mensaje = 'Usuario registrado'
		end
		else
		begin
			set @Registrado = 0
			set @Mensaje = 'Usuario registrado'
		end
end

-- Procedimiento almacenado para acceder al sistema
create procedure sp_ValidarUsuario 
(
	@Correo varchar(100),
	@Clave varchar (500)
)
as
begin
		if(exists(select*from USUARIO where Correo = @Correo and Clave = @Clave))
			select IdUsuario from USUARIO where Correo = @Correo and Clave = @Clave
		else
			select '0'
end

--Procedimineto almacenado para recuperar los roles del usuario
create  proc sp_ObtenerRolesPorUsuario
(
	@Correo varchar(100)
)
as
begin
	if(exists(select*from USUARIO where Correo = @Correo))
		select Rol from USUARIO where Correo = @Correo
	else
		select '3'
end;

-- Procedimiento para ADMINISTRADORES que agrega un nuevo usuario y asigna rol
CREATE PROCEDURE sp_Insertar_Usuario
    @Nombre NVARCHAR(100),
    @Correo NVARCHAR(100),
    @Clave NVARCHAR(500),
    @Id_rol INT,
	@Registrado bit output,
	@Mensaje varchar (100) output
AS
BEGIN
    if (not exists (select*from USUARIO where Correo = @Correo))
	begin
			insert into USUARIO (Correo, Clave, Nombre, Rol) values (@Correo,@Clave,@Nombre,@Id_rol)
			set @Registrado = 1
			set @Mensaje = 'Usuario registrado'
		end
		else
		begin
			set @Registrado = 0
			set @Mensaje = 'Usuario registrado'
		end
END

-- Procedimiento para actualizar usuarios
CREATE OR ALTER PROCEDURE sp_Editar_Usuario
(
	@IdUsuario INT,
	@Nombre NVARCHAR(100),
	@Correo NVARCHAR(100),
	@Rol INT -- Nuevo rol que se desea asignar
)
AS
BEGIN
    -- Declarar variables
    DECLARE @RolActual INT;
    DECLARE @TotalAdmins INT;

    -- Obtener el rol actual del usuario
    SELECT @RolActual = Rol
    FROM USUARIO
    WHERE IdUsuario = @IdUsuario;

    -- Contar el total de administradores actuales
    SELECT @TotalAdmins = COUNT(*)
    FROM USUARIO
    WHERE Rol = 1;

    -- Validar que el cambio de rol no deje al sistema sin administradores
    IF @RolActual = 1 AND @Rol <> 1 AND @TotalAdmins <= 1
    BEGIN
        RAISERROR ('No se puede cambiar el rol del último administrador.', 16, 1);
        RETURN;
    END

    -- Si pasa las validaciones, se actualizan los datos del usuario
    UPDATE USUARIO
    SET Nombre = @Nombre, Correo = @Correo, Rol = @Rol
    WHERE IdUsuario = @IdUsuario;
END;


-- Procedimiento para eliminar usuarios 
CREATE OR ALTER PROCEDURE sp_Eliminar_Usuario
(
	@IdUsuario INT
)
AS
BEGIN
    -- Verificar si el usuario a eliminar es un administrador
    DECLARE @EsAdmin BIT;
    DECLARE @TotalAdmins INT;

    SELECT @EsAdmin = CASE 
                        WHEN Rol = 1 THEN 1 
                        ELSE 0 
                      END
    FROM USUARIO
    WHERE IdUsuario = @IdUsuario;

    -- Contar el total de administradores actuales
    SELECT @TotalAdmins = COUNT(*)
    FROM USUARIO
    WHERE Rol = 1;

    -- Validar que no se elimine el último administrador
    IF @EsAdmin = 1 AND @TotalAdmins <= 1
    BEGIN
        RAISERROR ('No se puede eliminar el último usuario con rol de administrador.', 16, 1);
        RETURN;
    END

    -- Si pasa las validaciones, se elimina el usuario
    DELETE FROM USUARIO WHERE IdUsuario = @IdUsuario;
END;


-- --------------------------------------------------------------
-- INSERTAR USUARIO ADMIN (contraseña encriptada ya proporcionada = 1)
-- --------------------------------------------------------------

-- Insertar un usuario de ejemplo (la contraseña ya debe ser encriptada antes de insertarse)
INSERT INTO USUARIO (Nombre, Correo, Clave, Rol) 
VALUES ('Miguel', '21212325@gmail.com', '6b86b273ff34fce19d6b804eff5a3f5747ada4eaa22f1d49c01e52ddb7875b4b', 1);

-- --------------------------------------------------------------
-- SECCIÓN DE COMPRADOR
-- --------------------------------------------------------------

-- Tabla Dirección del Comprador
CREATE TABLE Direccion_Comprador (
  Direccion_Comprador_Id INT IDENTITY PRIMARY KEY,
  Calle NVARCHAR(255) NOT NULL,
  Ciudad NVARCHAR(100) NOT NULL,
  Estado NVARCHAR(100) NOT NULL,
  Codigo_Postal NVARCHAR(20) NOT NULL,
);

-- Tabla Comprador
CREATE TABLE Comprador (
  Comprador_Id INT IDENTITY PRIMARY KEY,
  Nombre_Empresa NVARCHAR(255) NOT NULL,
  RFC NVARCHAR(20) NOT NULL,
  Correo_Electronico NVARCHAR(255) NOT NULL,
  Telefono NVARCHAR(20) NOT NULL,
  Direccion_Comprador_Id INT FOREIGN KEY REFERENCES Direccion_Comprador(Direccion_Comprador_Id)
);

-- View de comprador
Create OR ALTER View v_Comprador as Select 
Comprador_Id as 'ID',
Nombre_Empresa as 'Empresa',
RFC as 'RFC',
Correo_Electronico as 'Correo',
Telefono as 'Telefono',
Comprador.Direccion_Comprador_Id as 'IDR',
Calle as 'Calle',
Ciudad as 'Ciudad',
Estado as 'Estado',
Codigo_Postal as 'CP'
From Comprador
JOIN Direccion_Comprador ON Comprador.Direccion_Comprador_Id = Direccion_Comprador.Direccion_Comprador_Id

-- EDITAR COMPRADOR
CREATE OR ALTER PROCEDURE sp_Editar_Comprador
(
    @Comprador_Id INT,
    @Nombre_Empresa NVARCHAR(255),
    @RFC NVARCHAR(20),
    @Correo_Electronico NVARCHAR(255),
    @Telefono NVARCHAR(20),
    @Direccion_Comprador_Id INT,
    @Calle NVARCHAR(255),
    @Ciudad NVARCHAR(100),
    @Estado NVARCHAR(100),
    @Codigo_Postal NVARCHAR(20)
)
AS BEGIN
-- Actualizar la tabla Direccion_Comprador
	UPDATE Direccion_Comprador
	SET Calle = @Calle,
        Ciudad = @Ciudad,
        Estado = @Estado,
        Codigo_Postal = @Codigo_Postal
    WHERE Direccion_Comprador_Id = @Direccion_Comprador_Id;

-- Actualizar la tabla Comprador
	UPDATE Comprador
    SET Nombre_Empresa = @Nombre_Empresa,
        RFC = @RFC,
        Correo_Electronico = @Correo_Electronico,
        Telefono = @Telefono
    WHERE Comprador_Id = @Comprador_Id;
END

-- ELIMINAR COMPRADOR
CREATE OR ALTER PROCEDURE sp_EliminarComprador
(
    @Comprador_Id INT
)
AS BEGIN
        -- Obtener el ID de la dirección asociada antes de eliminar el comprador
        DECLARE @Direccion_Comprador_Id INT;
        SELECT @Direccion_Comprador_Id = Direccion_Comprador_Id
        FROM Comprador
        WHERE Comprador_Id = @Comprador_Id;

        -- Eliminar el comprador
        DELETE FROM Comprador
        WHERE Comprador_Id = @Comprador_Id;

        -- Eliminar la dirección asociada
        DELETE FROM Direccion_Comprador
        WHERE Direccion_Comprador_Id = @Direccion_Comprador_Id;
END;

-- AGREGAR COMPRADOR
create proc sp_Insertar_Comprador
(
	@Calle nvarchar(255),
	@Ciudad nvarchar(100),
	@Estado nvarchar(100),
	@Codigo_Postal nvarchar(20),
	@Nombre_Empresa NVARCHAR(255),
	@RFC NVARCHAR(20),
    @Correo_Electronico NVARCHAR(255),
    @Telefono NVARCHAR(20)
)
as begin 
	declare @IdDireccion int
	insert into Direccion_Comprador (Calle,Ciudad,Estado,Codigo_Postal) VALUES (@Calle, @Ciudad, @Estado, @Codigo_Postal)
	SET @IdDireccion = SCOPE_IDENTITY()
	insert into Comprador (Nombre_Empresa, RFC, Correo_Electronico, Telefono, Direccion_Comprador_Id) VALUES (@Nombre_Empresa, @RFC, @Correo_Electronico, @Telefono, @IdDireccion)
end 

-- --------------------------------------------------------------
-- SECCIÓN DE PAQUETERÍA
-- --------------------------------------------------------------

-- Tabla Paquetería
CREATE TABLE Paqueteria (
    Paqueteria_Id INT IDENTITY PRIMARY KEY,  
    Nombre NVARCHAR(255) NOT NULL,           
    Correo_Electronico NVARCHAR(255) NOT NULL UNIQUE,  -- Correo único para la paquetería
    Telefono NVARCHAR(20) NOT NULL           
);

-- INSERTAR PAQUETERIA
create procedure Registrar_Paqueteria
(
	@Nombre NVARCHAR(255),
	@Correo_Electronico NVARCHAR(255),
	@Telefono NVARCHAR(20)
) 
as begin
	insert into Paqueteria (Nombre, Correo_Electronico, Telefono) Values (@Nombre, @Correo_Electronico, @Telefono)
end

-- ELIMINAR PAQUETERIA
create procedure Eliminar_Paqueteria
(
	@Paqueteria_Id INT
)
as begin
	delete from Paqueteria where Paqueteria_Id = @Paqueteria_Id
end

-- ACTUALIZAR PAQUETERIA
create procedure Editar_Paqueteria
(
	@Paqueteria_Id INT,
	@Nombre NVARCHAR(255),
	@Correo_Electronico NVARCHAR(255),
	@Telefono NVARCHAR(20)
) 
as begin
	UPDATE Paqueteria SET Nombre = @Nombre, Correo_Electronico = @Correo_Electronico, Telefono = @Telefono where @Paqueteria_Id = Paqueteria_Id
end

-- --------------------------------------------------------------
-- SECCIÓN DE ESTADO DE ENVÍO
-- --------------------------------------------------------------

-- Tabla Estado de Envío
CREATE TABLE Estado_Envio (
  Estado_Envio_Id INT IDENTITY PRIMARY KEY,
  Estado NVARCHAR(255) NOT NULL,
  Fecha_Actualizacion DATETIME DEFAULT GETDATE(),
);

INSERT INTO Estado_Envio (Estado, Fecha_Actualizacion) VALUES ('En proceso',Getdate())
INSERT INTO Estado_Envio (Estado, Fecha_Actualizacion) VALUES ('Enviado',Getdate())
INSERT INTO Estado_Envio (Estado, Fecha_Actualizacion) VALUES ('Entregado',Getdate())

-- --------------------------------------------------------------
-- SECCIÓN DEL VENDEDOR
-- --------------------------------------------------------------

-- Tabla Dirección del Vendedor
CREATE TABLE Direccion_Vendedor (
  Direccion_Vendedor_Id INT IDENTITY PRIMARY KEY,
  Calle NVARCHAR(255) NOT NULL,
  Ciudad NVARCHAR(100) NOT NULL,
  Estado NVARCHAR(100) NOT NULL,
  Codigo_Postal NVARCHAR(20) NOT NULL
);

-- Tabla Vendedor
CREATE TABLE Vendedor (
  Vendedor_Id INT IDENTITY PRIMARY KEY,
  Nombre_Empresa NVARCHAR(255) NOT NULL,
  Correo_Electronico NVARCHAR(255) NOT NULL,
  Telefono NVARCHAR(20) NOT NULL,
  Direccion_Vendedor_Id INT FOREIGN KEY REFERENCES Direccion_Vendedor(Direccion_Vendedor_Id)
);

-- View de Vendedor
create view v_Vendedor as select 
Vendedor_Id as 'ID',
Nombre_Empresa as 'Empresa',
Correo_Electronico as 'Email',
Telefono as 'Telefono',
Direccion_Vendedor.Direccion_Vendedor_Id as 'IDdir',
Calle as 'Calle',
Ciudad as 'Ciudad',
Estado as 'Estado',
Codigo_Postal as 'CP'
From Vendedor
JOIN Direccion_Vendedor ON Direccion_Vendedor.Direccion_Vendedor_Id = Vendedor.Direccion_Vendedor_Id

-- INSERTAR VENDEDOR
create procedure sp_Insertar_Vendedor
(
	@Calle nvarchar(255),
	@Ciudad nvarchar(100),
	@Estado nvarchar(100),
	@Codigo_Postal nvarchar(100),
	@Nombre_Empresa nvarchar (255),
	@Correo_Electronico nvarchar (255),
	@Telefono nvarchar (100)
)
as begin 
	declare @IdDireccion int
	insert into Direccion_Vendedor (Calle, Ciudad, Estado, Codigo_Postal) VALUES (@Calle, @Ciudad, @Estado, @Codigo_Postal)
	SET @IdDireccion = SCOPE_IDENTITY()
	insert into Vendedor (Nombre_Empresa, Correo_Electronico, Telefono, Direccion_Vendedor_Id) VALUES (@Nombre_Empresa, @Correo_Electronico, @Telefono, @IdDireccion)
end 

-- EDITAR VENDEDOR
CREATE OR ALTER PROCEDURE sp_Editar_Vendedor
    @Vendedor_Id INT,
    @Nombre_Empresa NVARCHAR(255),
    @Correo_Electronico NVARCHAR(255),
    @Telefono NVARCHAR(20),
    @Direccion_Vendedor_Id INT,
    @Calle NVARCHAR(255),
    @Ciudad NVARCHAR(100),
    @Estado NVARCHAR(100),
    @Codigo_Postal NVARCHAR(20)
AS
BEGIN
        -- Actualiza la información del vendedor
        UPDATE Vendedor
        SET
            Nombre_Empresa = @Nombre_Empresa,
            Correo_Electronico = @Correo_Electronico,
            Telefono = @Telefono,
            Direccion_Vendedor_Id = @Direccion_Vendedor_Id
        WHERE Vendedor_Id = @Vendedor_Id;

        -- Actualiza la información de la dirección del vendedor
        UPDATE Direccion_Vendedor
        SET
            Calle = @Calle,
            Ciudad = @Ciudad,
            Estado = @Estado,
            Codigo_Postal = @Codigo_Postal
        WHERE Direccion_Vendedor_Id = @Direccion_Vendedor_Id;
END;

-- ELIMINAR VENDEDOR 
CREATE OR ALTER PROCEDURE sp_EliminarVendedor
    @Vendedor_Id INT
AS
BEGIN
            -- Declarar una variable para almacenar el ID de la dirección del vendedor a eliminar
        DECLARE @Direccion_Vendedor_Id INT;

        -- Obtener el ID de la dirección asociada al vendedor que se desea eliminar
        SELECT @Direccion_Vendedor_Id = Direccion_Vendedor_Id
        FROM Vendedor
        WHERE Vendedor_Id = @Vendedor_Id;

        -- Eliminar el registro del vendedor
        DELETE FROM Vendedor
        WHERE Vendedor_Id = @Vendedor_Id;

            -- Si no está asociada a ningún otro vendedor, eliminar la dirección
            DELETE FROM Direccion_Vendedor
            WHERE Direccion_Vendedor_Id = @Direccion_Vendedor_Id;
END

-- --------------------------------------------------------------
-- SECCIÓN DE PRODUCTOS
-- --------------------------------------------------------------

-- Tabla Tipo de Producto
CREATE TABLE Tipo_Producto (
  Tipo_Producto_Id INT IDENTITY PRIMARY KEY,
  Nombre_Tipo NVARCHAR(100) NOT NULL,
);

INSERT INTO Tipo_Producto (Nombre_Tipo) VALUES
('Placa PCB Monocapa'),
('Placa PCB Doble cara'),
('Placa PCB Multicapa'),
('Placa PCB Flex'),
('Placa PCB Rígida'),
('Placa PCB Flexible-Rígida'),
('Placa PCB de Alta frecuencia'),
('Placa PCB para microondas'),
('Placa PCB con tecnología HDI'),
('Placa PCB para dispositivos móviles');

-- Tabla Producto
CREATE TABLE Producto (
  Producto_Id INT IDENTITY PRIMARY KEY,
  Tipo_Producto_Id INT FOREIGN KEY REFERENCES Tipo_Producto(Tipo_Producto_Id),
  Cantidad INT NOT NULL CHECK (Cantidad >= 0),
  Nombre_Producto nvarchar(100)
);

-- View de Producto
create view v_Productos as Select
Producto_ID as 'ID',
Nombre_Producto as 'Nombre',
Nombre_Tipo as 'Categoria',
Cantidad as 'Stock'
from Producto
Join Tipo_Producto 
On Producto.Tipo_Producto_Id = Tipo_Producto.Tipo_Producto_Id

-- INSERTAR PRODUCTO
CREATE PROCEDURE sp_Insertar_Producto
(
    @Tipo NVARCHAR(100),
    @Nombre NVARCHAR(100),
    @Stock INT,
    @Vendedor_Id INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Tipo_Producto_Id INT;
    DECLARE @Producto_Id INT;

    -- Verifica si el tipo de producto ya existe
    SELECT @Tipo_Producto_Id = Tipo_Producto_Id 
    FROM Tipo_Producto 
    WHERE Nombre_Tipo = @Tipo;

    -- Si no existe, retornar un error y finalizar
    IF @Tipo_Producto_Id IS NULL
    BEGIN
        RAISERROR('El tipo de producto especificado no existe. Solo se pueden usar tipos precargados.', 16, 1);
        RETURN;
    END

    -- Inserta en la tabla Producto utilizando el Tipo_Producto_Id existente
    INSERT INTO Producto (Tipo_Producto_Id, Cantidad, Nombre_Producto)
    VALUES (@Tipo_Producto_Id, @Stock, @Nombre);

    -- Obtener el Producto_Id generado
    SET @Producto_Id = SCOPE_IDENTITY();

    -- Inserta la relación en la tabla Vendedor_Producto
    INSERT INTO Vendedor_Producto (Vendedor_Id, Producto_Id)
    VALUES (@Vendedor_Id, @Producto_Id);
END;


-- ELIMINAR PRODUCTO
CREATE PROCEDURE sp_Eliminar_Producto
(
	@Id_Producto int
)
AS BEGIN
	Delete From Producto where Producto_Id = @Id_Producto
End

-- ACTUALIZAR PRODUCTO
CREATE PROCEDURE sp_Editar_Producto
(
	@Producto_Id int,
	@Nombre_Producto nvarchar(100),
	@Stock int
)
AS BEGIN
	UPDATE Producto SET Cantidad = @Stock, Nombre_Producto = @Nombre_Producto where @Producto_Id = Producto_Id
End

-- --------------------------------------------------------------
-- SECCIÓN DE PEDIDO
-- --------------------------------------------------------------

-- Tabla Pedido
CREATE TABLE Pedido (
  Pedido_Id INT IDENTITY PRIMARY KEY,
  Comprador_Id INT FOREIGN KEY REFERENCES Comprador(Comprador_Id),
  Fecha_Pedido DATETIME DEFAULT GETDATE(),
  Estado_Envio_Id INT FOREIGN KEY REFERENCES Estado_Envio ON DELETE CASCADE,
  Paqueteria_Id INT FOREIGN KEY REFERENCES Paqueteria ON DELETE CASCADE
);

-- Tabla Pedido_Producto (relación de muchos a muchos entre Pedido y Producto)
CREATE TABLE Pedido_Producto (
  Pedido_Producto_Id INT IDENTITY PRIMARY KEY,
  Pedido_Id INT FOREIGN KEY REFERENCES Pedido(Pedido_Id) ON DELETE CASCADE,
  Producto_Id INT FOREIGN KEY REFERENCES Producto(Producto_Id) ON DELETE CASCADE,
  Cantidad INT NOT NULL CHECK (Cantidad > 0)
);

-- Tabla Vendedor_Producto (Entidad de relación entre Vendedor y Producto)
CREATE TABLE Vendedor_Producto (
  Vendedor_Producto_Id INT IDENTITY PRIMARY KEY,
  Vendedor_Id INT FOREIGN KEY REFERENCES Vendedor(Vendedor_Id) ON DELETE CASCADE,
  Producto_Id INT FOREIGN KEY REFERENCES Producto(Producto_Id) ON DELETE CASCADE
);

-- View para ver los pedidos
CREATE or alter VIEW v_Detalles_Pedido AS
SELECT 
    p.Pedido_Id AS 'ID',
    p.Fecha_Pedido AS 'Fecha Pedido',
    c.Nombre_Empresa AS 'Comprador',
    es.Estado AS 'Estado',
    tp.Nombre_Tipo AS 'Tipo de producto',
    pr.Nombre_Producto AS 'Nombre Producto',
    pp.Cantidad AS 'Cantidad',
    v.Nombre_Empresa AS 'Vendedor',
    pa.Nombre AS 'Paquetería'  -- Agregando el nombre de la paquetería
FROM 
    Pedido p
JOIN 
    Comprador c ON p.Comprador_Id = c.Comprador_Id
JOIN 
    Estado_Envio es ON p.Estado_Envio_Id = es.Estado_Envio_Id
JOIN 
    Pedido_Producto pp ON p.Pedido_Id = pp.Pedido_Id
JOIN 
    Producto pr ON pp.Producto_Id = pr.Producto_Id
JOIN 
    Tipo_Producto tp ON pr.Tipo_Producto_Id = tp.Tipo_Producto_Id
JOIN 
    Vendedor_Producto vp ON pr.Producto_Id = vp.Producto_Id
JOIN 
    Vendedor v ON vp.Vendedor_Id = v.Vendedor_Id
JOIN 
    Paqueteria pa ON p.Paqueteria_Id = pa.Paqueteria_Id;  -- Unión con la tabla Paqueteria

-- View de datos de comprobante
CREATE OR ALTER VIEW v_DatosComprobante AS
SELECT
    -- Tabla Pedido
    Pedido.Pedido_Id AS 'ID',
    Pedido.Fecha_Pedido AS 'Fecha_del_pedido',

    -- Tabla Paqueteria
    Paqueteria.Nombre AS 'Paqueteria',
    Paqueteria.Correo_Electronico AS 'Correo_Paqueteria',
    Paqueteria.Telefono AS 'Telefono_Paqueteria',

    -- Tabla Comprador
    Comprador.Nombre_Empresa AS 'Empresa_Comprador',
    Comprador.Correo_Electronico AS 'Correo_Comprador',
    Comprador.Telefono AS 'Telefono_Comprador',

    -- Tabla Direccion_Comprador 
    Direccion_Comprador.Calle AS 'Calle_Comprador',
    Direccion_Comprador.Ciudad AS 'Ciudad_Comprador',
    Direccion_Comprador.Estado AS 'Estado_Comprador',
    Direccion_Comprador.Codigo_Postal AS 'Codigo_Postal_Comprador',

    -- Tabla Pedido_Producto
    Pedido_Producto.Cantidad AS 'Cantidad',

    -- Tabla Producto
    Producto.Nombre_Producto AS 'Producto',

    -- Tabla Tipo_Producto
    Tipo_Producto.Nombre_Tipo AS 'Categoria_Producto',

    -- Tabla Vendedor
    Vendedor.Nombre_Empresa AS 'Empresa_Vendedor',
    Vendedor.Correo_Electronico AS 'Correo_Vendedor',
    Vendedor.Telefono AS 'Telefono_Vendedor',

    -- Tabla Direccion_Vendedor
    Direccion_Vendedor.Calle AS 'Calle_Vendedor',
    Direccion_Vendedor.Ciudad AS 'Ciudad_Vendedor',
    Direccion_Vendedor.Estado AS 'Estado_Vendedor',
    Direccion_Vendedor.Codigo_Postal AS 'Codigo_Postal_Vendedor'

FROM 
    Pedido
    JOIN Paqueteria ON Pedido.Paqueteria_Id = Paqueteria.Paqueteria_Id
    JOIN Comprador ON Pedido.Comprador_Id = Comprador.Comprador_Id
    JOIN Direccion_Comprador ON Comprador.Direccion_Comprador_Id = Direccion_Comprador.Direccion_Comprador_Id
    JOIN Pedido_Producto ON Pedido_Producto.Pedido_Id = Pedido.Pedido_Id
    JOIN Producto ON Producto.Producto_Id = Pedido_Producto.Producto_Id
    JOIN Tipo_Producto ON Tipo_Producto.Tipo_Producto_Id = Producto.Tipo_Producto_Id
    JOIN Vendedor_Producto ON Vendedor_Producto.Producto_Id = Producto.Producto_Id
    JOIN Vendedor ON Vendedor.Vendedor_Id = Vendedor_Producto.Vendedor_Id
    JOIN Direccion_Vendedor ON Direccion_Vendedor.Direccion_Vendedor_Id = Vendedor.Direccion_Vendedor_Id;

-- INSERTAR PEDIDO
CREATE OR ALTER PROCEDURE sp_Insertar_Pedido
(
    @Comprador_Id INT,
    @Producto_Id INT,
    @Cantidad INT,
    @Paqueteria_Id INT,
    @Estado_Envio_Id INT,
    @Vendedor_Id INT
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Declarar la variable interna para el Pedido_Id
    DECLARE @NuevoPedidoId INT;

    -- Verificar si el Estado_Envio_Id proporcionado existe en la tabla Estado_Envio
    IF NOT EXISTS (SELECT 1 FROM Estado_Envio WHERE Estado_Envio_Id = @Estado_Envio_Id)
    BEGIN
        RAISERROR('El Estado_Envio_Id proporcionado no existe en la tabla Estado_Envio.', 16, 1);
        RETURN;
    END

    -- Verificar si el Paqueteria_Id proporcionado existe en la tabla Paqueteria
    IF NOT EXISTS (SELECT 1 FROM Paqueteria WHERE Paqueteria_Id = @Paqueteria_Id)
    BEGIN
        RAISERROR('El Paqueteria_Id proporcionado no existe en la tabla Paqueteria.', 16, 1);
        RETURN;
    END

    -- Insertar el Pedido
    INSERT INTO Pedido (Comprador_Id, Estado_Envio_Id, Paqueteria_Id)
    VALUES (@Comprador_Id, @Estado_Envio_Id, @Paqueteria_Id);

    -- Obtener el Pedido_Id generado
    SET @NuevoPedidoId = SCOPE_IDENTITY();

    -- Insertar el Producto asociado al Pedido
    INSERT INTO Pedido_Producto (Pedido_Id, Producto_Id, Cantidad)
    VALUES (@NuevoPedidoId, @Producto_Id, @Cantidad);

    -- Verificar si ya existe la relación entre el Producto_Id y el Vendedor_Id en Vendedor_Producto
    IF NOT EXISTS (SELECT 1 FROM Vendedor_Producto WHERE Vendedor_Id = @Vendedor_Id AND Producto_Id = @Producto_Id)
    BEGIN
        -- Si no existe, insertamos la relación
        INSERT INTO Vendedor_Producto (Vendedor_Id, Producto_Id)
        VALUES (@Vendedor_Id, @Producto_Id);
    END
END;

-- FINALIZAR PEDIDO
create or alter procedure sp_Finalizar_Pedido
(
	@Id_Pedido int
)
as begin 
	UPDATE Pedido SET Estado_Envio_Id = 3 Where Pedido_Id = @Id_Pedido
end 

-- TRIGGER PARA ACTUALIZAR STOCK
CREATE TRIGGER tr_Actualizar_Stock
ON Pedido_Producto
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- Actualizar la cantidad de productos en la tabla Producto
    UPDATE Producto
    SET Producto.Cantidad = Producto.Cantidad - inserted.Cantidad
    FROM Producto
    INNER JOIN inserted ON Producto.Producto_Id = inserted.Producto_Id;

    -- Verificación para asegurarse de que no se tenga una cantidad negativa
    IF EXISTS (SELECT 1 FROM Producto WHERE Producto.Cantidad < 0)
    BEGIN
        -- Revertir la operación si hay cantidades negativas
        RAISERROR('No hay suficiente stock para completar el pedido.', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;


-- --------------------------------------------------------------
-- SECCIÓN DE FACTURACIÓN
-- --------------------------------------------------------------

-- Tabla Factura (relacionada con Pedido)
CREATE TABLE Factura (
  Factura_Id INT IDENTITY PRIMARY KEY,
  Pedido_Id INT FOREIGN KEY REFERENCES Pedido(Pedido_Id) ON DELETE CASCADE,
  Numero_Factura NVARCHAR(50) NOT NULL UNIQUE,
  Fecha_Emision DATETIME2 DEFAULT SYSDATETIME(),
  Monto_Total DECIMAL(10, 2) NOT NULL,
  Impuesto DECIMAL(10, 2) DEFAULT 0
);