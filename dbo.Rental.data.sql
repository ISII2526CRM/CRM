SET IDENTITY_INSERT [dbo].[Rental] ON
INSERT INTO [dbo].[Rental] ([Id], [DeliveryAddress], [PaymentMethod], [RentalDate], [RentalDateFrom], [RentalDateTo], [TotalPrice], [UserId]) VALUES (4, N'Calle 1', 1, N'2000-05-05 00:00:00', N'2000-02-02 00:00:00', N'2000-03-02 00:00:00', 0, N'1')
INSERT INTO [dbo].[Rental] ([Id], [DeliveryAddress], [PaymentMethod], [RentalDate], [RentalDateFrom], [RentalDateTo], [TotalPrice], [UserId]) VALUES (5, N'Calle 2', 2, N'2002-05-05 00:00:00', N'2000-01-01 00:00:00', N'2000-01-16 00:00:00', 0, N'2')
SET IDENTITY_INSERT [dbo].[Rental] OFF
