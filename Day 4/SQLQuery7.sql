use pubs;

WITH cteAuthors AS (
    SELECT au_id, CONCAT(au_fname, ' ', au_lname) AS author_name
    FROM authors
)
SELECT * FROM cteAuthors;


declare @page int =2, @pageSize int=10;
with PaginatedBooks as
( select  title_id,title, price, ROW_Number() over (order by price desc) as RowNum
  from titles
)
select * from PaginatedBooks where rowNUm between((@page-1)*@pageSize) and (@page*@pageSize)