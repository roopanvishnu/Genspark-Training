-- 1. Create Customer table with encrypted and masked data fields

CREATE TABLE Customer (
    CustomerID SERIAL PRIMARY KEY,
    FullName TEXT,
    MaskedName TEXT,
    EncryptedEmail BYTEA
);

-- 2. Create function to encrypt text using pgcrypto
CREATE OR REPLACE FUNCTION EncryptText(input TEXT, key TEXT)
RETURNS BYTEA AS $$
BEGIN
    RETURN pgp_sym_encrypt(input, key);
END;
$$ LANGUAGE plpgsql;

-- 3. Create function to decrypt text using pgcrypto
CREATE OR REPLACE FUNCTION DecryptText(input BYTEA, key TEXT)
RETURNS TEXT AS $$
BEGIN
    RETURN pgp_sym_decrypt(input, key);
END;
$$ LANGUAGE plpgsql;

-- 4. Create function to mask full name (first letter + asterisks)
CREATE OR REPLACE FUNCTION MaskName(input TEXT)
RETURNS TEXT AS $$
BEGIN
    RETURN SUBSTRING(input, 1, 1) || REPEAT('*', LENGTH(input) - 1);
END;
$$ LANGUAGE plpgsql;
-- 5. Create procedure to insert masked and encrypted data into Customer
CREATE OR REPLACE PROCEDURE InsertCustomerData(
    p_fullname TEXT,
    p_email TEXT,
    p_key TEXT
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO Customer (FullName, MaskedName, EncryptedEmail)
    VALUES (
        p_fullname,
        MaskName(p_fullname),
        EncryptText(p_email, p_key)
    );
END;
$$;
-- 6. Create procedure to display masked names and decrypted emails
CREATE OR REPLACE PROCEDURE ShowMaskedDecrypted(key TEXT)
LANGUAGE plpgsql
AS $$
DECLARE
    rec RECORD;
BEGIN
    FOR rec IN SELECT MaskedName, EncryptedEmail FROM Customer LOOP
        RAISE NOTICE 'Masked Name: %, Email: %', rec.MaskedName, DecryptText(rec.EncryptedEmail, key);
    END LOOP;
END;
$$;

