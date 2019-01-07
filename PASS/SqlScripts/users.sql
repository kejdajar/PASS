CREATE TABLE Users (id INT PRIMARY KEY IDENTITY(1,1),username NCHAR(40) not null UNIQUE, pswd NVARCHAR(MAX) not null, salt NVARCHAR(MAX) not null, userRole INT not null)
CREATE TABLE UserRoles (id INT PRIMARY KEY IDENTITY(1,1),name NCHAR(40) not null UNIQUE)
ALTER TABLE Users ADD CONSTRAINT fk_role FOREIGN KEY (userRole) REFERENCES UserRoles(id)