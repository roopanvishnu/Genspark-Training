-- 1) List all orders with the customer name and the employee who handled the order.



select FirstName , ContactName,ProductName from Customers c join Orders o on c.CustomerID = o.CustomerID 
join Employees e on e.EmployeeID = o.EmployeeID join [Order Details] od on o.OrderID = od.OrderID join Products p 
on p.ProductID = od.ProductID;



--2) Get a list of products along with their category and supplier name.

--(Join Products, Categories, and Suppliers)

select ProductName, CategoryName, ContactName from Products p join Categories c on p.CategoryID = c.CategoryID
join Suppliers s on p.SupplierID = s.SupplierID ; 

--3) Show all orders and the products included in each order with quantity and unit price.

--(Join Orders, Order Details, Products)

SELECT 
    o.OrderID AS All_Orders,
    p.ProductName,
    od.Quantity,
    od.UnitPrice
FROM 
    [Order Details] od
JOIN 
    Products p ON od.ProductID = p.ProductID
JOIN 
    Orders o ON od.OrderID = o.OrderID;

--4) List employees who report to other employees (manager-subordinate relationship).

--(Self join on Employees)

select concat (FirstName ,' ',LastName) as FullName from Employees;


--5) Display each customer and their total order count.

--(Join Customers and Orders, then GROUP BY)


SELECT 
    c.CustomerID,
    c.CompanyName,
    COUNT(o.OrderID) AS TotalOrders
FROM 
    Customers c
LEFT JOIN 
    Orders o ON c.CustomerID = o.CustomerID
GROUP BY 
    c.CustomerID, c.CompanyName
ORDER BY 
    TotalOrders DESC;

--6) Find the average unit price of products per category.

--Use AVG() with GROUP BY

select CategoryName, avg(UnitPrice) from Categories c join Products p on
p.CategoryID = c.CategoryID group by CategoryName;

-- 7) List customers where the contact title starts with 'Owner'
--Use LIKE or LEFT(ContactTitle, 5)

select ContactName,ContactTitle from Customers where ContactTitle like '%Owner';

--8) Show the top 5 most expensive products.

-- Use ORDER BY UnitPrice DESC and TOP 5

select top 5 ProductName, UnitPrice from Products order by UnitPrice desc ;

--9) Return the total sales amount (quantity × unit price) per order.

--Use SUM(OrderDetails.Quantity * OrderDetails.UnitPrice) and GROUP BY
SELECT 
    od.OrderID,
    SUM(od.Quantity * od.UnitPrice) AS TotalSalesAmount
FROM 
    [Order Details] od
GROUP BY 
    od.OrderID
ORDER BY 
    TotalSalesAmount DESC;


--10) Create a stored procedure that returns all orders for a given customer ID.

--Input: @CustomerID
CREATE PROCEDURE sp_GetOrdersByCustomer
    @CustomerID NVARCHAR(5)
AS
BEGIN
    SELECT 
        o.OrderID,
        o.OrderDate,
        o.ShipName,
        o.ShipCity,
        o.ShipCountry
    FROM 
        Orders o
    WHERE 
        o.CustomerID = @CustomerID
    ORDER BY 
        o.OrderDate DESC;
END;

EXEC sp_GetOrdersByCustomer @CustomerID = 'ALFKI';


--11

CREATE PROCEDURE sp_InsertProduct
    @ProductName NVARCHAR(100),
    @SupplierID INT,
    @CategoryID INT,
    @UnitPrice MONEY,
    @UnitsInStock INT,
    @UnitsOnOrder INT,
    @Discontinued BIT
AS
BEGIN
    INSERT INTO Products (ProductName, SupplierID, CategoryID, UnitPrice, UnitsInStock, UnitsOnOrder, Discontinued)
    VALUES (@ProductName, @SupplierID, @CategoryID, @UnitPrice, @UnitsInStock, @UnitsOnOrder, @Discontinued);
END;


--12
CREATE PROCEDURE sp_TotalSalesPerEmployee
AS
BEGIN
    SELECT 
        e.EmployeeID,
        e.FirstName + ' ' + e.LastName AS EmployeeName,
        SUM(od.UnitPrice * od.Quantity) AS TotalSales
    FROM 
        Employees e
        JOIN Orders o ON e.EmployeeID = o.EmployeeID
        JOIN [Order Details] od ON o.OrderID = od.OrderID
    GROUP BY 
        e.EmployeeID, e.FirstName, e.LastName
    ORDER BY 
        TotalSales DESC;
END;

--13
WITH ProductRank AS (
    SELECT 
        ProductID,
        ProductName,
        CategoryID,
        UnitPrice,
        RANK() OVER (PARTITION BY CategoryID ORDER BY UnitPrice DESC) AS PriceRank
    FROM 
        Products
)
SELECT * FROM ProductRank;


--14
WITH ProductRevenue AS (
    SELECT 
        p.ProductID,
        p.ProductName,
        SUM(od.UnitPrice * od.Quantity) AS TotalRevenue
    FROM 
        Products p
        JOIN [Order Details] od ON p.ProductID = od.ProductID
    GROUP BY 
        p.ProductID, p.ProductName
)
SELECT * FROM ProductRevenue WHERE TotalRevenue > 10000;
--15

WITH EmployeeHierarchy AS (
    SELECT 
        EmployeeID,
        LastName,
        FirstName,
        ReportsTo,
        0 AS Level
    FROM 
        Employees
    WHERE 
        ReportsTo IS NULL

    UNION ALL

    SELECT 
        e.EmployeeID,
        e.LastName,
        e.FirstName,
        e.ReportsTo,
        eh.Level + 1
    FROM 
        Employees e
        JOIN EmployeeHierarchy eh ON e.ReportsTo = eh.EmployeeID
)
SELECT * FROM EmployeeHierarchy ORDER BY Level, EmployeeID;
