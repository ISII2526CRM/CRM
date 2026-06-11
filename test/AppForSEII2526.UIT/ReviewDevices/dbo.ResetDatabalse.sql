-- 1. LIMPIEZA DE RESEÑAS (Nombres en Singular)
-- Borramos primero los items porque dependen de la reseña
DELETE FROM [dbo].[ReviewItem]; 
DELETE FROM [dbo].[Review];

-- 2. LIMPIEZA DE DISPOSITIVOS
-- Borramos los dispositivos de prueba si existen
DELETE FROM [dbo].[Device] WHERE [Name] IN ('XPS 15', 'MX Keys S');

-- 3. PREPARAR EL MODELO (Requisito de tu base de datos)
-- Tu tabla Device obliga a tener un ModelId, así que creamos un modelo genérico primero.
IF NOT EXISTS (SELECT 1 FROM [dbo].[Model] WHERE [NameModel] = 'Standard')
BEGIN
    INSERT INTO [dbo].[Model] ([NameModel]) VALUES ('Standard');
END

-- Guardamos el ID del modelo para usarlo abajo
DECLARE @ModelId INT = (SELECT TOP 1 [Id] FROM [dbo].[Model] WHERE [NameModel] = 'Standard');

-- 4. INSERTAR DISPOSITIVOS (Con todas las columnas obligatorias del log)
-- XPS 15
INSERT INTO [dbo].[Device] (
    [ModelId], [Name], [Brand], [Color], 
    [PriceForPurchase], [PriceForRent], 
    [QuantityForPurchase], [QuantityForRent], 
    [Year]
)
VALUES (
    @ModelId, 'XPS 15', 'Dell', 'Plata', 
    1500, 50,  -- Precios compra/alquiler
    10, 10,    -- Stock
    2023
);

-- MX Keys S
INSERT INTO [dbo].[Device] (
    [ModelId], [Name], [Brand], [Color], 
    [PriceForPurchase], [PriceForRent], 
    [QuantityForPurchase], [QuantityForRent], 
    [Year]
)
VALUES (
    @ModelId, 'MX Keys S', 'Logitech', 'Grafito', 
    120, 15, 
    20, 20, 
    2024
);

-- MX Keys S
INSERT INTO [dbo].[Device] (
    [ModelId], [Name], [Brand], [Color], 
    [PriceForPurchase], [PriceForRent], 
    [QuantityForPurchase], [QuantityForRent], 
    [Year]
)
VALUES (
    @ModelId, 'Otro', 'Logitech', 'Grafito', 
    120, 15, 
    20, 2, 
    2024
);


-- ... (después de insertar los dispositivos) ...

-- 5. USUARIOS: Crear usuario de prueba 'alice@test.com'
-- Primero borramos si existe para evitar duplicados
DELETE FROM [dbo].[AspNetUserRoles] WHERE [UserId] = 'user-alice-id';
DELETE FROM [dbo].[AspNetUsers] WHERE [Id] = 'user-alice-id';



-- Insertamos el Usuario
-- NOTA: El PasswordHash de abajo corresponde a "Password123!"
INSERT INTO [dbo].[AspNetUsers] (
    [Id], 
    [UserName], [NormalizedUserName], 
    [Email], [NormalizedEmail], 
    [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], 
    [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnabled], [AccessFailedCount]
)
VALUES (
    'user-alice-id',                         -- ID Fijo para el test
    'alice@test.com', 'ALICE@TEST.COM',      -- UserName (Normal y Mayúsculas)
    'alice@test.com', 'ALICE@TEST.COM',      -- Email (Normal y Mayúsculas)
    1,                                       -- Email Confirmado (True)
    'AQAAAAIAAYagAAAAELhRvI/wEgy6nh17TyzxsPoZopxw9W6lsTQKurI3thyB5q78vGtMpEP+hwab3wFuJA==', -- Hash de "Password.123" (aproximado)
    'DUMMYSECURITYSTAMP',                    -- Security Stamp
    'DUMMYCONCURRENCYSTAMP',                 -- Concurrency Stamp
    0, 0, 1, 0                               -- Flags varios por defecto
);

-- 6. ROLES (Opcional, pero recomendado si tu app usa roles)
-- Aseguramos que existe el rol 'Customer'
IF NOT EXISTS (SELECT 1 FROM [dbo].[AspNetRoles] WHERE [Name] = 'Customer')
BEGIN
    INSERT INTO [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName])
    VALUES ('role-customer-id', 'Customer', 'CUSTOMER');
END

-- Asignamos el rol a Alice
INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId])
VALUES ('user-alice-id', 'role-customer-id');