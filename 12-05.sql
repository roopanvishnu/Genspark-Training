1️⃣ Question:
In a transaction, if I perform multiple updates and an error happens in the third statement, but I have not used SAVEPOINT, what will happen if I issue a ROLLBACK?
Will my first two updates persist?
--No the 1st 2 updates will not persist as there was no savepoint or commit and inside begin transaction, there is no autocommit

2️⃣ Question:note
Suppose Transaction A updates Alice’s balance but does not commit. Can Transaction B read the new balance if the isolation level is set to READ COMMITTED?
--No ,with read commited only the commited changes can be read and hence the uncommit change by alice cannot be read by bob.

3️⃣ Question:
What will happen if two concurrent transactions both execute:
UPDATE tbl_bank_accounts SET balance = balance - 100 WHERE account_name = 'Alice';
at the same time? Will one overwrite the other?
-- No it wont be overwritten , one transaction will start execution and the other will wait till it gets completed and applies the command on the newly updated row


4️⃣ Question:
If I issue ROLLBACK TO SAVEPOINT after_alice;, will it only undo changes made after the savepoint or everything?
--Changes after the savepoint will be lost.

5️⃣ Question:
Which isolation level in PostgreSQL prevents phantom reads?
--Repeatable read and serializable

6️⃣ Question:
Can Postgres perform a dirty read (reading uncommitted data from another transaction)?
--No, it is not allowed in postgres

7️⃣ Question:
If autocommit is ON (default in Postgres), and I execute an UPDATE, is it safe to assume the change is immediately committed?
--Yes

8️⃣ Question:
If I do this:

BEGIN;
UPDATE accounts SET balance = balance - 500 WHERE id = 1;
-- (No COMMIT yet)
And from another session, I run:

SELECT balance FROM accounts WHERE id = 1;
Will the second session see the deducted balance?
--No , if we use begin and unless we explicitly mention commit, the new changes wont be commited. So we will be seeing the old data itself.