CREATE TABLE Customer(
	CustomerId int NOT NULL,
	CustomerName nvarchar(50) NULL,
 CONSTRAINT PK_Customer PRIMARY KEY CLUSTERED (CustomerId)
)
GO

CREATE TABLE [Order](
	OrderId int NOT NULL,
	CustomerId int NOT NULL,
	ReceivedDate datetime NULL,
 CONSTRAINT PK_Order PRIMARY KEY CLUSTERED (OrderId)
)
GO

CREATE TABLE OrderDetail(
	OrderId int NOT NULL,
	ProductId int NOT NULL,
	Amount int NOT NULL,
 CONSTRAINT [PK_OrderDetail] PRIMARY KEY CLUSTERED (OrderId, ProductId)
)
GO

CREATE TABLE Product(
	ProductId int NOT NULL,
	ProductDescr nvarchar(50) NOT NULL,
	Price money NULL,
 CONSTRAINT PK_Product PRIMARY KEY CLUSTERED (ProductId ASC)
)
GO

ALTER TABLE [Order] ADD  CONSTRAINT DF_Order_ReceivedDate  DEFAULT (getdate()) FOR ReceivedDate
GO

ALTER TABLE [Order]  WITH CHECK ADD  CONSTRAINT FK_Order_Customer FOREIGN KEY(CustomerId)
REFERENCES Customer (CustomerId)
GO

ALTER TABLE [Order] CHECK CONSTRAINT FK_Order_Customer
GO

ALTER TABLE OrderDetail  WITH CHECK ADD  CONSTRAINT FK_OrderDetail_Order FOREIGN KEY(OrderId)
REFERENCES [Order] (OrderId)
GO

ALTER TABLE OrderDetail CHECK CONSTRAINT FK_OrderDetail_Order
GO

ALTER TABLE OrderDetail  WITH CHECK ADD  CONSTRAINT FK_OrderDetail_Product FOREIGN KEY(ProductId)
REFERENCES Product (ProductId)
GO

ALTER TABLE OrderDetail CHECK CONSTRAINT FK_OrderDetail_Product
GO
