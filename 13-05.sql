--13/05/2025
--1 locking 
update bank_accounts set balance=balance-1000 where account_id=1;
--session2
update bank_accounts set balance=balance+1000 where account_id=1;

--2
update bank_accounts set balance=balance-1000 where account_id=1;

--3 deadlock
rollback;
--session1
begin;
select * from bank_accounts where account_id=2 for update;

begin;
select * from bank_accounts where account_id=1 for update;

select * from bank_accounts where account_id=1 for update;

--session2
select * from bank_accounts where account_id=2 for update;

--AFTERNOON
-- Cursors
--1. Write a cursor to list all customers and how many rentals each made. Insert these into a summary table.
create table summary_table (
    customer_id int primary key,
    rental_count int
);
 
do $$ 
declare 
	cr cursor for select customer_id, count(*) as rental_count from rental group by customer_id;
	cust_rec record;

begin
    for cust_rec in cr
    loop
        insert into summary_table (customer_id, rental_count) 
        values (cust_rec.customer_id, cust_rec.rental_count);
    end loop;
end $$;

select * from summary_table;
 
--2.Using a cursor, print the titles of films in the 'Comedy' category rented more than 10 times.
do $$ 
declare 
	cr cursor for select f.title from film f
        join film_category fc on f.film_id = fc.film_id
        join category c on fc.category_id = c.category_id
        join inventory i on f.film_id = i.film_id
        join rental r on i.inventory_id = r.inventory_id
        where c.name = 'Comedy'
        group by f.title
        having count(r.rental_id) > 10;
	film_rec record;
begin
    for film_rec in cr
    loop
        raise notice '%', film_rec.title;
    end loop;
end $$;
 
--3.Create a cursor to go through each store and count the number of distinct films available, and insert results into a report table.
create table report_table (
    store_id int primary key,
    film_count int
);
 
do $$ 
declare 
	cr cursor for select s.store_id, count(distinct i.film_id) as film_count
        from store s
        join inventory i on s.store_id = i.store_id
        group by s.store_id;
	store_rec record;
begin
    for store_rec in cr
    loop
        insert into report_table (store_id, film_count) 
        values (store_rec.store_id, store_rec.film_count);
    end loop;
end $$;
 
select * from report_table;
 
--4.Loop through all customers who haven't rented in the last 6 months and insert their details into an inactive_customers table.
create table inactive_customers (
    customer_id int primary key,
    first_name varchar(50),
    last_name varchar(50),
    email varchar(100)
);
 
do $$ 
declare 
	cr cursor for select customer_id, first_name, last_name, email 
        from customer
        where customer_id not in 
            (select distinct customer_id from rental where rental_date >= now() - interval '6 months');
	cust_rec record;
begin
    for cust_rec in cr
    loop
        insert into inactive_customers (customer_id, first_name, last_name, email) 
        values (cust_rec.customer_id, cust_rec.first_name, cust_rec.last_name, cust_rec.email);
    end loop;
end $$;
 
select * from inactive_customers;

--Transactions

--1.Write a transaction that inserts a new customer, adds their rental, and logs the payment – all atomically.

do $$
declare 
	new_customer_id int;
	new_rental_id int;
begin
    insert into customer (store_id, address_id, first_name, last_name, email, create_date)
    values (1, 1, 'Mark', 'A', 'Mark@mail.com', now())
    returning customer_id into new_customer_id;
    insert into public.rental (rental_date, inventory_id, customer_id, staff_id, return_date)
    values (now(), 1, new_customer_id, 1, null)
	returning rental_id into new_rental_id;
 
	insert into payment (customer_id, staff_id, rental_id, amount, payment_date)
	values (new_customer_id, 1, new_rental_id, 10.99, now());
	commit;	
end;
$$;
 
select * from payment where rental_id=16073;


--2. Simulate a transaction where one update fails (e.g., invalid rental ID), and ensure the entire transaction rolls back.
do $$
begin
    update film 
    set rental_rate = 4.99
    WHERE film_id = 1;
    insert into inventory (film_id, store_id) 
    VALUES (10000, 1);
exception
    when others then
        ROLLBACK;
        raise notice 'Transaction rolled back due to error.';
end;
$$;
 
select * from film where film_id=1

--3. Use SAVEPOINT to update multiple payment amounts. Roll back only one payment update using ROLLBACK TO SAVEPOINT.

select * from payment;
begin
 
update payment
set amount = amount + 10
where payment_id = 17512;
savepoint payment_update;
 
select * from payment where payment_id=17512
 
update payment
set amount = amount + 100
where payment_id = 17512;
 
select * from payment where payment_id=17512
 
rollback to savepoint payment_update;
 
select * from payment where payment_id=17512
 
commit;

-- 4 Perform a transaction that transfers inventory from one store to another (delete + insert) safely.

select * from inventory where inventory_id = 1
 
do $$
declare 
	v_inventory_id INT := 1;
	v_from_store_id INT := 2;
	v_to_store_id INT := 1;
Begin
    update inventory
    set store_id = v_to_store_id
    where inventory_id = v_inventory_id
    and store_id = v_from_store_id;
    if not found then
        raise exception 'Inventory item % not found in store %', v_inventory_id, v_from_store_id;
    end if;
    raise notice 'Inventory item % moved from store % to store %', v_inventory_id, v_from_store_id, v_to_store_id;
end;
$$;
 
select * from inventory where inventory_id = 1

-- 5 Create a transaction that deletes a customer and all associated records (rental, payment), ensuring referential integrity.


select * from customer where customer_id=607
select * from rental where customer_id=607
select * from payment where rental_id=16074
 
do $$
declare v_customer_id INT := 607;
begin
    delete from payment
    where customer_id = v_customer_id;
    delete from rental
    where customer_id = v_customer_id;
    delete from customer
    where customer_id = v_customer_id;
 
	commit;
end;
$$;
 
select * from customer where customer_id=607
select * from rental where customer_id=607
select * from payment where rental_id=16074

--triggers
--1.Create a trigger to prevent inserting payments of zero or negative amount.
create or replace function payment_trigger_func()
returns trigger as $$
begin
	if New.amount<=0.00 then 
		raise exception 'Amount cannot be <= 0';
	end if;
	return New;
end;
$$ language plpgsql;

create or replace trigger paymentinsert 
after insert on payment
for each row
execute function payment_trigger_func();

insert into payment (payment_id, customer_id, staff_id, rental_id, amount, payment_date)
VALUES (1, 103, 1, 202, -10.00, '2025-05-13');
select * from staff;

--2.Set up a trigger that automatically updates last_update on the film table when the title or rental rate is changed.
create or replace function last_update_function()
returns trigger as $$
begin
	New.last_update=NOW();
	Raise notice 'Last updated set using trigger';
	return New;
end;
$$ language plpgsql;

create or replace trigger on_film_update
after update on film
for each row
execute function last_update_function();

select * from film where film_id=2;
update film set title='Hmm' where film_id=2;

--3. Write a trigger that inserts a log into rental_log whenever a film is rented more than 3 times in a week.
create table rental_log(
 id serial primary key,
 film_id integer,
 message varchar(100)
)

create or replace function log_freq_rental()
returns trigger as $$
declare 
	filmid integer;
	rentalcount integer;
begin
	select film_id into filmid from inventory where inventory_id=New.inventory_id;
	select count(*) into rentalcount from rental a join inventory b on a.inventory_id=b.inventory_id where b.film_id=filmid and a.rental_date>=NOW() - INTERVAL '7 days';
	if rentalcount>3 then 
		insert into rental_log(film_id,message) values(filmid,format('This movie is rented for %s time in last week',rentalcount));
	end if;
	return null;
end;
$$ language plpgsql;

create or replace trigger log_trgr
after insert on rental
for each row
execute function log_freq_rental()

select * from rental where inventory_id=2;
select * from inventory;
insert into rental(rental_date,inventory_id,customer_id,return_date,staff_id,last_update)
values(current_timestamp,2,411,null,1,current_timestamp);

select * from rental_log;

