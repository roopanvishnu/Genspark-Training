-- Suppliers table
CREATE TABLE suppliers( 
    supplierId INT PRIMARY KEY IDENTITY(1,1),
    supplierName VARCHAR(100),
    contactInfo VARCHAR(200)
);

-- Product table
CREATE TABLE product (
    productId INT PRIMARY KEY IDENTITY(1,1),
    productName VARCHAR(200),
    price DECIMAL(10,2),
    stock INT
);

-- Supplier-Product junction table (many-to-many)
CREATE TABLE supplierProduct (
    supplierId INT,
    productId INT,
    PRIMARY KEY (supplierId, productId),
    FOREIGN KEY (supplierId) REFERENCES suppliers(supplierId),
    FOREIGN KEY (productId) REFERENCES product(productId)
);

-- Customers table
CREATE TABLE customers (
    customerId INT PRIMARY KEY IDENTITY(1,1),
    customerName VARCHAR(100),
    customerPhone VARCHAR(20),
    customerEmail VARCHAR(100),
    customerAddress VARCHAR(255)
);

-- Purchases (Bill) table
CREATE TABLE purchases (
    purchaseId INT PRIMARY KEY IDENTITY(1,1),
    customerId INT,
    purchaseDate DATETIME DEFAULT GETDATE(),
    grandTotal DECIMAL(10,2),
    FOREIGN KEY (customerId) REFERENCES customers(customerId)
);

-- Purchase details table
CREATE TABLE purchaseDetails (
    purchaseId INT,
    productId INT,
    quantity INT,
    priceAtPurchase DECIMAL(10,2),
    PRIMARY KEY (purchaseId, productId),
    FOREIGN KEY (purchaseId) REFERENCES purchases(purchaseId),
    FOREIGN KEY (productId) REFERENCES product(productId)
);
