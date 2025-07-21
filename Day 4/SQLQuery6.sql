use pubs;

select title,pub_name from titles join publishers on titles.pub_id = publishers.pub_id;

select title ,pub_name from titles t join publishers p on t.pub_id = p.pub_id;

select title, pub_name from titles t join publishers p on t.pub_id = p.pub_id where title is null ;

SELECT title,pub_name
FROM publishers p
LEFT JOIN titles t ON p.pub_id = t.pub_id
WHERE t.pub_id IS NULL;


select * from publishers where pub_id not in 
(select distinct pub_id from titles);

select au_id, title from titles t join titleauthor ta on t.title_id = ta.title_id;

select au_fname as Author_Name , title Book_Name from authors a join titleauthor ta on a.au_id = ta.au_id
join titles t on ta.title_id = t.title_id;


select pub_name, title,pubdate from publishers p join titles t on p.pub_id = t.pub_id order by t.pubdate;

SELECT pub_name, title, ord_date 
FROM publishers p JOIN titles t ON p.pub_id = t.pub_id
JOIN sales s ON s.title_id = t.title_id;


-- print the publisher name and the first book sale date for all the publishers 

select pub_name as Publisher_Name , title as Book_Titles, min(t.pubdate) as First_Published_Date from publishers p join titles t on p.pub_id = t.pub_id 
group by p.pub_name order by p.pub_name;
select p.pub_name as Publisher_Name, min(t.pubdate) as First_published_Date from publishers p join titles t on p.pub_id = t.pub_id group by p.pub_name order by 2 desc;

SELECT 
    p.pub_name AS Publisher_Name,
    MIN(t.pubdate) AS First_Published_Date
FROM 
    publishers p
JOIN 
    titles t ON p.pub_id = t.pub_id
GROUP BY 
    p.pub_name
ORDER BY 
    p.pub_name;


-- print the book name and the store address of the sale
select title as Book_Name , stor_address as Store_Address from titles t join sales s on s.title_id = t.title_id join stores on s.stor_id = stores.stor_id;

-- First batch: Create the procedure
CREATE PROCEDURE sp_FirstProcedure
AS 
BEGIN
    PRINT 'Hello World';
END;
GO  

EXEC sp_FirstProcedure;
go

create table Productss
(id int identity(1,1) constraint pk_productId primary key,
name nvarchar(100) not null,
details nvarchar(max))
Go
create proc proc_InsertProduct(@pname nvarchar(100),@pdetails nvarchar(max))
as
begin
    insert into Productss(name,details) values(@pname,@pdetails)
end
go
proc_InsertProduct 'Laptop','{"brand":"Dell","spec":{"ram":"16GB","cpu":"i5"}}'
go
select * from Productss

create or alter proc proc_InsertProduct(@pname nvarchar(100),@pdetails nvarchar(max))
as
begin
    insert into Productss(name,details) values(@pname,@pdetails)
end

select JSON_QUERY(details, '$.spec') Product_Specification from productss

create proc proc_UpdateProductSpec(@pid int,@newvalue varchar(20))
as
begin
   update productss set details = JSON_MODIFY(details, '$.spec.ram',@newvalue) where id = @pid
end

proc_UpdateProductSpec 1, '24GB'

select id, name, JSON_VALUE (details, '$.brand') Brand_name from productss;









create table Posts
  (id int primary key,
  title nvarchar(100),
  user_id int,
  body nvarchar(max))
Go;

  select * from Posts;
  select * from productss

  create proc proc_BulInsertPosts(@jsondata nvarchar(max))
  as
  begin
		insert into Posts(user_id,id,title,body)
	  select userId,id,title,body from openjson(@jsondata)
	  with (userId int,id int, title varchar(100), body varchar(max))
  end

  delete from Posts

  proc_BulInsertPosts '
[
  {
    "userId": 1,
    "id": 1,
    "title": "sunt aut facere repellat provident occaecati excepturi optio reprehenderit",
    "body": "quia et suscipit\nsuscipit recusandae consequuntur expedita et cum\nreprehenderit molestiae ut ut quas totam\nnostrum rerum est autem sunt rem eveniet architecto"
  },
  {
    "userId": 1,
    "id": 2,
    "title": "qui est esse",
    "body": "est rerum tempore vitae\nsequi sint nihil reprehenderit dolor beatae ea dolores neque\nfugiat blanditiis voluptate porro vel nihil molestiae ut reiciendis\nqui aperiam non debitis possimus qui neque nisi nulla"
  }]'

select * from productss where 
try_cast (json_value(details, '$.spec.cpu') as nvarchar(20)) = 'i7';

select * from productss;




create table people
(id int primary key,
name nvarchar(20),
age int)

create or alter proc proc_BulkInsert(@filepath nvarchar(500))
as
begin
   declare @insertQuery nvarchar(max)

   set @insertQuery = 'BULK INSERT people from '''+ @filepath +'''
   with(
   FIRSTROW =2,
   FIELDTERMINATOR='','',
   ROWTERMINATOR = ''\n'')'
   exec sp_executesql @insertQuery
end

proc_BulkInsert 'D:\sql\data.csv'

select * from people;

