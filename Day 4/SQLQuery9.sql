-- transaction practice

use pubs;

create table bankAccount (
	accountId int primary key,
	accountNumber int ,
	balance decimal (10,2)
);

INSERT INTO bankAccount (accountId, accountNumber, balance) VALUES 
(1, 100001, 1500.75),
(2, 100002, 2450.00),
(3, 100003, 387.60),
(4, 100004, 5600.00),
(5, 100005, 120.25),
(6, 100006, 9800.00),
(7, 100007, 3025.50),
(8, 100008, 0.00),
(9, 100009, 199.99),
(10, 100010, 725.10);

select * from bankAccount;

begin transaction 
update bankAccount set balance = balance -1000 where accountNumber = '100001'
update bankAccount set balance = balance +1000 where accountNumber = '100002'
select 1/0

if @@error <> rollback transaction 
else 
commit transaction