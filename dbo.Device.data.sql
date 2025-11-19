SET IDENTITY_INSERT [dbo].[Device] ON
INSERT INTO [dbo].[Device] ([Id], [ModelId], [Name], [Brand], [Color], [PriceForPurchase], [PriceForRent], [QuantityForPurchase], [QuantityForRent], [Year]) VALUES (1, 1, N'Dispositivo 1', N'Samsung', N'Rojo', 500, 600, 1, 1, 2002)
INSERT INTO [dbo].[Device] ([Id], [ModelId], [Name], [Brand], [Color], [PriceForPurchase], [PriceForRent], [QuantityForPurchase], [QuantityForRent], [Year]) VALUES (5, 1, N'Dispositivo 2', N'Iphone', N'Azul', 1000, 800, 1, 1, 2006)
INSERT INTO [dbo].[Device] ([Id], [ModelId], [Name], [Brand], [Color], [PriceForPurchase], [PriceForRent], [QuantityForPurchase], [QuantityForRent], [Year]) VALUES (8, 2, N'Dispositivo 3', N'Samsung', N'Verde', 1500, 1200, 1, 1, 2008)
INSERT INTO [dbo].[Device] ([Id], [ModelId], [Name], [Brand], [Color], [PriceForPurchase], [PriceForRent], [QuantityForPurchase], [QuantityForRent], [Year]) VALUES (10, 3, N'Dispositivo 4', N'Samsung', N'Negro', 1200, 1100, 1, 1, 2010)
SET IDENTITY_INSERT [dbo].[Device] OFF
