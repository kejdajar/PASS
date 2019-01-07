﻿CREATE TABLE Company (id INT PRIMARY KEY IDENTITY(1,1), name NCHAR(100) NOT NULL UNIQUE, adress NCHAR(100),city NCHAR(100),postalCode INT,phone NCHAR(20),web NCHAR(100))
CREATE TABLE Bill (id INT PRIMARY KEY, billText NVARCHAR(200))
ALTER TABLE Bill ADD CONSTRAINT fk_bill FOREIGN KEY (id) REFERENCES Company(id)