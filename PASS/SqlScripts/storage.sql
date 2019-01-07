CREATE TABLE Products (id INT PRIMARY KEY IDENTITY(1,1),name NCHAR(100) not null,quantity INT not null,unit INT,unitQuantity DECIMAL(18,2),expirationDate DATE,code INT not null UNIQUE, price DECIMAL(18,2), priceForUnit BIT not null,vatId NCHAR(1) not null)
CREATE TABLE Units (id INT PRIMARY KEY IDENTITY(1,1),name NCHAR(40) not null UNIQUE)
CREATE TABLE Vat (id NCHAR(1) PRIMARY KEY,rate INT UNIQUE not null)
ALTER TABLE Products ADD CONSTRAINT fk_units FOREIGN KEY (unit) REFERENCES Units(id)
ALTER TABLE Products ADD CONSTRAINT fk_vat FOREIGN KEY (vatId) REFERENCES Vat(id)
CREATE TABLE Orders (id INT PRIMARY KEY IDENTITY(1,1), paid INT, staff NCHAR(100),timeOfTransaction DATETIME,companyName NCHAR(100),companyAdress NCHAR(100),companyCity NCHAR(100),companyPostalCode NCHAR(100),companyPhone NCHAR(100),companyWeb NCHAR(100),billText NCHAR(100), vatSum DECIMAL(18,2), vatSumSingle DECIMAL(18,2), totalShoppingCartPrice DECIMAL(18,2), change DECIMAL(18,2) )
CREATE TABLE OrderItems(id INT PRIMARY KEY IDENTITY(1,1),orderId INT not null,name NCHAR(100) not null,quantity INT not null,unit NCHAR(40),unitQuantity DECIMAL(18,2),expirationDate DATE,code INT not null, price DECIMAL(18,2), priceForUnit BIT not null,vatId NCHAR(1) not null,totalPrice DECIMAL(18,2))
ALTER TABLE OrderItems ADD CONSTRAINT fk_orderId FOREIGN KEY (orderId) REFERENCES Orders(id) ON DELETE CASCADE
CREATE TABLE OrderItemsVat (orderId INT, vatId NCHAR(1), percentageLabel NCHAR(100), vatValue DECIMAL(18,2),vatValueProducts DECIMAL(18,2), primary key (orderId, vatId))
ALTER TABLE OrderItemsVat ADD CONSTRAINT fk_compositeKey1 FOREIGN KEY (orderId) REFERENCES Orders(id) ON DELETE CASCADE