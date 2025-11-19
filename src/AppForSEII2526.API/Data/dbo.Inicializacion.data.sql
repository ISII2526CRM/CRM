INSERT INTO [dbo].[AspNetUsers] ([Id], [Name], [Surname], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'1', N'miguel', N'moreno', N'mi', N'1 ', N'1', N'1', 1, N'1', N'1', N'1', N'1', 1, 1, N'10/10/2000 0:00:00 +02:00', 1, 1)
INSERT INTO [dbo].[AspNetUsers] ([Id], [Name], [Surname], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'2', N'manuel', N'moreno', N'ma', N'2', N'2', N'2', 1, N'2', N'1', N'1', N'1', 1, 1, N'10/10/2000 0:00:00 +02:00', 1, 1)

INSERT INTO [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'1', N'm', N'h', NULL)
INSERT INTO [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'2', N'l', N'j', NULL)

INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'1', N'1')
INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'2', N'2')

SET IDENTITY_INSERT [dbo].[Model] ON
INSERT INTO [dbo].[Model] ([Id], [NameModel]) VALUES (1, N'Movil')
INSERT INTO [dbo].[Model] ([Id], [NameModel]) VALUES (2, N'Tablet')
INSERT INTO [dbo].[Model] ([Id], [NameModel]) VALUES (3, N'Television')
SET IDENTITY_INSERT [dbo].[Model] OFF

SET IDENTITY_INSERT [dbo].[Device] ON
INSERT INTO [dbo].[Device] ([Id], [ModelId], [Name], [Brand], [Color], [PriceForPurchase], [PriceForRent], [QuantityForPurchase], [QuantityForRent], [Year]) VALUES (1, 1, N'Dispositivo 1', N'Samsung', N'Rojo', 500, 600, 1, 1, 2002)
INSERT INTO [dbo].[Device] ([Id], [ModelId], [Name], [Brand], [Color], [PriceForPurchase], [PriceForRent], [QuantityForPurchase], [QuantityForRent], [Year]) VALUES (5, 1, N'Dispositivo 2', N'Iphone', N'Azul', 1000, 800, 1, 1, 2006)
INSERT INTO [dbo].[Device] ([Id], [ModelId], [Name], [Brand], [Color], [PriceForPurchase], [PriceForRent], [QuantityForPurchase], [QuantityForRent], [Year]) VALUES (8, 2, N'Dispositivo 3', N'Samsung', N'Verde', 1500, 1200, 1, 1, 2008)
INSERT INTO [dbo].[Device] ([Id], [ModelId], [Name], [Brand], [Color], [PriceForPurchase], [PriceForRent], [QuantityForPurchase], [QuantityForRent], [Year]) VALUES (10, 3, N'Dispositivo 4', N'Samsung', N'Negro', 1200, 1100, 1, 1, 2010)
SET IDENTITY_INSERT [dbo].[Device] OFF

SET IDENTITY_INSERT [dbo].[Rental] ON
INSERT INTO [dbo].[Rental] ([Id], [DeliveryAddress], [PaymentMethod], [RentalDate], [RentalDateFrom], [RentalDateTo], [TotalPrice], [UserId]) VALUES (4, N'Calle 1', 1, N'2000-05-05 00:00:00', N'2000-02-02 00:00:00', N'2000-03-02 00:00:00', 0, N'1')
INSERT INTO [dbo].[Rental] ([Id], [DeliveryAddress], [PaymentMethod], [RentalDate], [RentalDateFrom], [RentalDateTo], [TotalPrice], [UserId]) VALUES (5, N'Calle 2', 2, N'2002-05-05 00:00:00', N'2000-01-01 00:00:00', N'2000-01-16 00:00:00', 0, N'2')
SET IDENTITY_INSERT [dbo].[Rental] OFF

INSERT INTO [dbo].[RentDevice] ([DeviceId], [RentalId], [Price], [Quantity]) VALUES (1, 4, 0, 1)
INSERT INTO [dbo].[RentDevice] ([DeviceId], [RentalId], [Price], [Quantity]) VALUES (5, 5, 0, 1)
INSERT INTO [dbo].[RentDevice] ([DeviceId], [RentalId], [Price], [Quantity]) VALUES (8, 4, 0, 1)