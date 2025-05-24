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

select
    o.OrderID as All_Orders,
    p.ProductName,
    od.Quantity,
    od.UnitPrice
from 
    [Order Details] od
join 
    Products p on od.ProductID = p.ProductID
join 
    Orders o on od.OrderID = o.OrderID;

--4) List employees who report to other employees (manager-subordinate relationship).

--(Self join on Employees)

select concat (FirstName ,' ',LastName) as FullName from Employees;


--5) Display each customer and their total order count.

--(Join Customers and Orders, then GROUP BY)


select 
    c.CustomerID,
    c.CompanyName,
    count(o.OrderID) as TotalOrders
from 
    Customers c
left join 
    Orders o on c.CustomerID = o.CustomerID
group by 
    c.CustomerID, c.CompanyName
order by 
    TotalOrders desc;

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
select 
    od.OrderID,
    sum(od.Quantity * od.UnitPrice) as TotalSalesAmount
from 
    [Order Details] od
group by 
    od.OrderID
order by 
    TotalSalesAmount desc;


--10) Create a stored procedure that returns all orders for a given customer ID.

--Input: @CustomerID
create procedure sp_GetOrdersByCustomer
    @CustomerID nvarchar(5)
as
begin
    select 
        o.OrderID,
        o.OrderDate,
        o.ShipName,
        o.ShipCity,
        o.ShipCountry
    from 
        Orders o
    where 
        o.CustomerID = @CustomerID
    order by 
        o.OrderDate desc;
end;

exec sp_GetOrdersByCustomer @CustomerID = 'ALFKI';


--11

create procedure sp_InsertProduct
    @ProductName nvarchar(100),
    @SupplierID int,
    @CategoryID int,
    @UnitPrice MONEY,
    @UnitsInStock int,
    @UnitsOnOrder int,
    @Discontinued BIT
as
begin
    insert into Products (ProductName, SupplierID, CategoryID, UnitPrice, UnitsInStock, UnitsOnOrder, Discontinued)
    values (@ProductName, @SupplierID, @CategoryID, @UnitPrice, @UnitsInStock, @UnitsOnOrder, @Discontinued);
end;


--12
create procedure sp_TotalSalesPerEmployee
as
begin
    select 
        e.EmployeeID,
        e.FirstName + ' ' + e.LastName as EmployeeName,
        sum(od.UnitPrice * od.Quantity) as TotalSales
    from 
        Employees e
        JOIN Orders o on e.EmployeeID = o.EmployeeID
        JOIN [Order Details] od on o.OrderID = od.OrderID
    group by 
        e.EmployeeID, e.FirstName, e.LastName
    order by 
        TotalSales desc;
end;

--13
with ProductRank as (
    select 
        ProductID,
        ProductName,
        CategoryID,
        UnitPrice,
        rank() over (partition by CategoryID order by UnitPrice desc) as PriceRank
    from 
        Products
)
select * from ProductRank;


--14
with ProductRevenue as (
    select 
        p.ProductID,
        p.ProductName,
        sum(od.UnitPrice * od.Quantity) as TotalRevenue
    from 
        Products p
        JOIN [Order Details] od on p.ProductID = od.ProductID
   group by
        p.ProductID, p.ProductName
)
select * from ProductRevenue where TotalRevenue > 10000;
--15

with EmployeeHierarchy as (
    select 
        EmployeeID,
        LastName,
        FirstName,
        ReportsTo,
        0 as Level
    from 
        Employees
    where 
        ReportsTo IS NULL

    union ALL

    select 
        e.EmployeeID,
        e.LastName,
        e.FirstName,
        e.ReportsTo,
        eh.Level + 1
    from 
        Employees e
        JOIN EmployeeHierarchy eh on e.ReportsTo = eh.EmployeeID
)
select * from EmployeeHierarchy order by Level,EmployeeID;