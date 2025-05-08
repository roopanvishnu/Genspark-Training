use pubs;

select title from titles;

select title from titles where pub_id = 1389;

select title,price from titles where price between 10 and 15;

select title,price from titles where price is null;

select title from titles where title like 'the%';

select title from titles where title not like '%v%';

select title from titles order by royalty;

select t.* from titles t join publishers p on t.pub_id = p.pub_id order by p.pub_name desc, t.type asc, t.price desc;

select avg(price) as avg_price from titles group by type;

select distinct type from titles;

select top 2 title,price from titles order by price desc;

select * from titles where type ='business' and price <20 and advance >7000;

select pub_id , count(*) as book_count from titles where price between 15 and 25 and title like '%It%' group by pub_id having count (*) > 2 order by book_count asc;

select * from authors where state = 'CA';

select state , count (*) as author_count from authors group by state;


-- 2nd task

create table suppliers( 
	supplierId int Primary key ,
	supplierName varchar(100),
	contanctInfo varchar(200)
);

--product
create table products (
	productId int primary key,
	productName varchar(200),
	price decimal(10,2),
	stock int
);

--supplier product junction table (m to m);
create table supplierProduct(
	supplierId int,
	productId int ,
	primary key (supplierId, productId),
	foreign key (supplierId) references suppliers(supplierId),
	foreign key (productId) references products(productId)
);

--customer
create table customers(
	customerId int primary key,
	customerName varchar(100),
	customerPhone varchar (20),
	customerEmail varchar(100),
	customerAddress varchar(255)
);

--bill
create table purchases(
	purchaseId int primary key,
	customerId int ,
	purchaseDate datetime default getDate(),
	grandTotal decimal(10,2),
	foreign key (customerId) references customers (customerId)
);
--purchase details
create table purchaseDetails (
	purchaseId int,
	productId int,
	quantity int,
	priceAtPurchase decimal(10,2),
	primary key (purchaseId,productId),
	foreign key (purchaseId) references purchases (purchaseId),
	foreign key (productId) references products(productId)
);