USE ViaEuropeDB;
GO

CREATE OR ALTER PROCEDURE sp_InsertCategory
    @Name NVARCHAR(100),
    @Description NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Categories (Name, Description)
    VALUES (@Name, @Description);
    SELECT SCOPE_IDENTITY() AS NewId;
END
GO

CREATE OR ALTER PROCEDURE sp_InsertDish
    @Name NVARCHAR(150),
    @CountryOfOrigin NVARCHAR(100) = NULL,
    @Description NVARCHAR(1000) = NULL,
    @Price DECIMAL(10,2),
    @PortionQuantity DECIMAL(10,2),
    @PortionUnit NVARCHAR(20),
    @TotalQuantity DECIMAL(10,2),
    @TotalUnit NVARCHAR(20),
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Dishes 
        (Name, CountryOfOrigin, Description, Price, 
         PortionQuantity, PortionUnit, TotalQuantity, TotalUnit, CategoryId)
    VALUES 
        (@Name, @CountryOfOrigin, @Description, @Price,
         @PortionQuantity, @PortionUnit, @TotalQuantity, @TotalUnit, @CategoryId);
    SELECT SCOPE_IDENTITY() AS NewId;
END
GO

CREATE OR ALTER PROCEDURE sp_InsertOrder
    @UserId INT,
    @OrderCode NVARCHAR(50),
    @FoodCost DECIMAL(10,2),
    @TransportCost DECIMAL(10,2),
    @DiscountAmount DECIMAL(10,2),
    @TotalCost DECIMAL(10,2),
    @EstimatedDelivery DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Orders
        (UserId, OrderCode, OrderDate, Status,
         FoodCost, TransportCost, DiscountAmount, TotalCost, EstimatedDelivery)
    VALUES
        (@UserId, @OrderCode, GETDATE(), 'Inregistrata',
         @FoodCost, @TransportCost, @DiscountAmount, @TotalCost, @EstimatedDelivery);
    SELECT SCOPE_IDENTITY() AS NewId;
END
GO


CREATE OR ALTER PROCEDURE sp_InsertUser
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @Email NVARCHAR(200),
    @Phone NVARCHAR(20) = NULL,
    @DeliveryAddress NVARCHAR(500) = NULL,
    @PasswordHash NVARCHAR(256),
    @Role NVARCHAR(20) = 'Client'
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM Users WHERE Email = @Email)
    BEGIN
        RAISERROR('Email already exists', 16, 1);
        RETURN;
    END
    INSERT INTO Users
        (FirstName, LastName, Email, Phone, DeliveryAddress, PasswordHash, Role)
    VALUES
        (@FirstName, @LastName, @Email, @Phone, @DeliveryAddress, @PasswordHash, @Role);
    SELECT SCOPE_IDENTITY() AS NewId;
END
GO

CREATE OR ALTER PROCEDURE sp_UpdateOrderStatus
    @OrderId INT,
    @NewStatus NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Orders
    SET Status = @NewStatus
    WHERE OrderId = @OrderId;
END
GO


CREATE OR ALTER PROCEDURE sp_UpdateDishQuantityAfterDelivery
    @OrderId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE d
    SET d.TotalQuantity = d.TotalQuantity - (oi.Quantity * d.PortionQuantity)
    FROM Dishes d
    INNER JOIN OrderItems oi ON oi.DishId = d.DishId
    WHERE oi.OrderId = @OrderId;

    UPDATE d
    SET d.TotalQuantity = d.TotalQuantity - (oi.Quantity * md.PortionQuantity)
    FROM Dishes d
    INNER JOIN MenuDishes md ON md.DishId = d.DishId
    INNER JOIN OrderItems oi ON oi.MenuId = md.MenuId
    WHERE oi.OrderId = @OrderId;
END
GO


CREATE OR ALTER PROCEDURE sp_UpdateDish
    @DishId INT,
    @Name NVARCHAR(150),
    @Price DECIMAL(10,2),
    @PortionQuantity DECIMAL(10,2),
    @PortionUnit NVARCHAR(20),
    @TotalQuantity DECIMAL(10,2),
    @TotalUnit NVARCHAR(20),
    @CategoryId INT,
    @CountryOfOrigin NVARCHAR(100) = NULL,
    @Description NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Dishes
    SET Name = @Name,
        Price = @Price,
        PortionQuantity = @PortionQuantity,
        PortionUnit = @PortionUnit,
        TotalQuantity = @TotalQuantity,
        TotalUnit = @TotalUnit,
        CategoryId = @CategoryId,
        CountryOfOrigin = @CountryOfOrigin,
        Description = @Description
    WHERE DishId = @DishId;
END
GO


CREATE OR ALTER PROCEDURE sp_GetFullMenu
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        c.CategoryId, c.Name AS CategoryName,
        d.DishId, d.Name AS DishName, d.CountryOfOrigin,
        d.Price, d.PortionQuantity, d.PortionUnit,
        d.TotalQuantity, d.TotalUnit,
        CASE WHEN d.TotalQuantity <= 0 THEN 1 ELSE 0 END AS IsUnavailable
    FROM Categories c
    LEFT JOIN Dishes d ON d.CategoryId = c.CategoryId
    ORDER BY c.Name, d.Name;
END
GO


CREATE OR ALTER PROCEDURE sp_SearchDishesByKeyword
    @Keyword NVARCHAR(150),
    @MustContain BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    IF @MustContain = 1
        SELECT d.*, c.Name AS CategoryName
        FROM Dishes d
        INNER JOIN Categories c ON c.CategoryId = d.CategoryId
        WHERE d.Name LIKE '%' + @Keyword + '%';
    ELSE
        SELECT d.*, c.Name AS CategoryName
        FROM Dishes d
        INNER JOIN Categories c ON c.CategoryId = d.CategoryId
        WHERE d.Name NOT LIKE '%' + @Keyword + '%';
END
GO


CREATE OR ALTER PROCEDURE sp_SearchDishesByAllergen
    @AllergenName NVARCHAR(100),
    @MustContain BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    IF @MustContain = 1
        SELECT DISTINCT d.*, c.Name AS CategoryName
        FROM Dishes d
        INNER JOIN Categories c ON c.CategoryId = d.CategoryId
        INNER JOIN DishAllergens da ON da.DishesId = d.DishId
        INNER JOIN Allergens a ON a.AllergenId = da.AllergensAllergenId
        WHERE a.Name LIKE '%' + @AllergenName + '%';
    ELSE
        SELECT d.*, c.Name AS CategoryName
        FROM Dishes d
        INNER JOIN Categories c ON c.CategoryId = d.CategoryId
        WHERE d.DishId NOT IN (
            SELECT da.DishesId
            FROM DishAllergens da
            INNER JOIN Allergens a ON a.AllergenId = da.AllergensAllergenId
            WHERE a.Name LIKE '%' + @AllergenName + '%'
        );
END
GO


CREATE OR ALTER PROCEDURE sp_GetOrdersByUser
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        o.OrderId, o.OrderCode, o.OrderDate, o.Status,
        o.FoodCost, o.TransportCost, o.DiscountAmount, o.TotalCost,
        o.EstimatedDelivery
    FROM Orders o
    WHERE o.UserId = @UserId
    ORDER BY o.OrderDate DESC;
END
GO


CREATE OR ALTER PROCEDURE sp_GetLowStockDishes
    @Threshold DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT DishId, Name, TotalQuantity, TotalUnit
    FROM Dishes
    WHERE TotalQuantity <= @Threshold
    ORDER BY TotalQuantity ASC;
END
GO


INSERT INTO Allergens (Name, Description) VALUES
('Gluten', 'Cereale care contin gluten'),
('Lactoza', 'Lapte si produse lactate'),
('Oua', 'Oua si produse din oua'),
('Peste', 'Peste si produse din peste'),
('Arahide', 'Arahide si produse derivate'),
('Telina', 'Telina si produse derivate'),
('Mustar', 'Mustar si produse derivate'),
('Susan', 'Susan si produse derivate');
GO

INSERT INTO Categories (Name, Description) VALUES
('Aperitive Europene', 'Gustari si aperitive din toata Europa'),
('Supe & Ciorbe', 'Supe traditionale europene'),
('Feluri Principale', 'Preparate principale din bucatariile Europei'),
('Deserturi Continentale', 'Dulciuri si deserturi europene'),
('Bauturi', 'Bauturi traditionale si moderne');
GO

INSERT INTO Dishes (Name, CountryOfOrigin, Description, Price, PortionQuantity, PortionUnit, TotalQuantity, TotalUnit, CategoryId) VALUES
('Bouillabaisse', 'Franta', 'Supa traditionala de peste din Marsilia', 45.00, 400, 'ml', 8000, 'ml', 2),
('Goulash', 'Ungaria', 'Tocana consistenta de vita cu paprika', 38.00, 350, 'g', 7000, 'g', 2),
('Borscht', 'Ucraina', 'Ciorba de sfecla rosie cu smantana', 28.00, 300, 'ml', 6000, 'ml', 2),
('Wiener Schnitzel', 'Austria', 'Snitel de vitel panat, servit cu lamaie', 52.00, 300, 'g', 6000, 'g', 3),
('Paella Valenciana', 'Spania', 'Orez cu fructe de mare, sofran si legume', 58.00, 400, 'g', 8000, 'g', 3),
('Moussaka', 'Grecia', 'Gratinata de vinete cu carne tocata si bechamel', 42.00, 350, 'g', 7000, 'g', 3),
('Fish & Chips', 'Marea Britanie', 'Cod prajit in aluat cu cartofi pai', 44.00, 400, 'g', 8000, 'g', 3),
('Pierogi', 'Polonia', 'Coltunasi umpluti cu cartofi si branza', 32.00, 250, 'g', 5000, 'g', 1),
('Bruschetta al Pomodoro', 'Italia', 'Paine crocanta cu rosii, usturoi si busuioc', 22.00, 150, 'g', 3000, 'g', 1),
('Tiramisu', 'Italia', 'Desert clasic cu mascarpone, cafea si piscoturi', 28.00, 150, 'g', 3000, 'g', 4),
('Creme Brulee', 'Franta', 'Crema de vanilie cu crusta de zahar caramelizat', 26.00, 120, 'g', 2400, 'g', 4),
('Strudel cu Mere', 'Austria', 'Placinta subtire cu mere, scortisoara si stafide', 22.00, 180, 'g', 3600, 'g', 4),
('Sangria', 'Spania', 'Vin rosu cu fructe proaspete si suc de portocale', 18.00, 300, 'ml', 6000, 'ml', 5),
('Gluhwein', 'Germania', 'Vin fiert cu condimente, specific sarbatorilor de iarna', 16.00, 250, 'ml', 5000, 'ml', 5),
('Currywurst', 'Germania', 'Carnati cu sos de ketchup si curry', 30.00, 250, 'g', 5000, 'g', 1);
GO