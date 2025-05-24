-- cursor practice

use pubs;

create table cursorPractice (
	id int primary key,
	valueInTable int,
	runningCount int);

insert into cursorPractice (id, valueInTable, runningCount) values (1,10,null),(2,20,null),(3,30,null),(4,40,null);

select * from cursorPractice;

declare @id int
declare @value int
declare @runningTotal int = 0;

declare runningTotalCursor cursor for 
select  id,valueInTable from cursorPractice order by id
open  runningTotalCursor 
fetch next from runningTotalCursor into @id,@value
while @@FETCH_STATUS = 0
begin 
set @runningTotal = @runningTotal + @value
update cursorPractice  set runningCount  = @runningTotal  where id = @id
fetch next from runningTotalCursor into @id,@value
end
close runningTotalCursor
deallocate runningTotalCursor

select * from cursorPractice;