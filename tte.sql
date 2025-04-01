CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `Categories` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Approved` tinyint(1) NOT NULL,
    CONSTRAINT `PK_Categories` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Coupons` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Code` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Discount` decimal(65,30) NOT NULL,
    CONSTRAINT `PK_Coupons` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Jobs` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Item_id` int NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `Type` int NOT NULL,
    `Operation` int NOT NULL,
    `Status` int NOT NULL,
    CONSTRAINT `PK_Jobs` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Roles` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Roles` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `SecurityQuestions` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Question` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_SecurityQuestions` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Products` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Title` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Price` decimal(65,30) NOT NULL,
    `Description` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Image` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Approved` tinyint(1) NOT NULL,
    `CategoryId` int NOT NULL,
    CONSTRAINT `PK_Products` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Products_Categories_CategoryId` FOREIGN KEY (`CategoryId`) REFERENCES `Categories` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `Users` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` longtext CHARACTER SET utf8mb4 NOT NULL,
    `UserName` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Email` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Password` longtext CHARACTER SET utf8mb4 NOT NULL,
    `SecurityAnswer` longtext CHARACTER SET utf8mb4 NULL,
    `RoleId` int NOT NULL,
    `SecurityQuestionId` int NULL,
    CONSTRAINT `PK_Users` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Users_Roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `Roles` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Users_SecurityQuestions_SecurityQuestionId` FOREIGN KEY (`SecurityQuestionId`) REFERENCES `SecurityQuestions` (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Inventory` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Total` int NOT NULL,
    `Available` int NOT NULL,
    `ProductId` int NOT NULL,
    CONSTRAINT `PK_Inventory` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Inventory_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `Carts` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Total_before_discount` decimal(65,30) NOT NULL,
    `Total_after_discount` decimal(65,30) NOT NULL,
    `ShippingCost` decimal(65,30) NOT NULL,
    `UserId` int NOT NULL,
    `CouponId` int NOT NULL,
    CONSTRAINT `PK_Carts` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Carts_Coupons_CouponId` FOREIGN KEY (`CouponId`) REFERENCES `Coupons` (`Id`),
    CONSTRAINT `FK_Carts_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `Orders` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Total_before_discount` decimal(65,30) NOT NULL,
    `Total_after_discount` decimal(65,30) NOT NULL,
    `ShippingCost` decimal(65,30) NOT NULL,
    `FinalTotal` decimal(65,30) NOT NULL,
    `Status` longtext CHARACTER SET utf8mb4 NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `UserId` int NOT NULL,
    `CouponId` int NOT NULL,
    CONSTRAINT `PK_Orders` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Orders_Coupons_CouponId` FOREIGN KEY (`CouponId`) REFERENCES `Coupons` (`Id`),
    CONSTRAINT `FK_Orders_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `Ratings` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Rate` int NOT NULL,
    `ProductId` int NOT NULL,
    `UserId` int NOT NULL,
    CONSTRAINT `PK_Ratings` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Ratings_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Ratings_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `Reviews` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Comment` longtext CHARACTER SET utf8mb4 NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `ProductId` int NOT NULL,
    `UserId` int NOT NULL,
    CONSTRAINT `PK_Reviews` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Reviews_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Reviews_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `Wishlists` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `UserId` int NOT NULL,
    `ProductId` int NOT NULL,
    CONSTRAINT `PK_Wishlists` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Wishlists_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Wishlists_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `Cart_Items` (
    `ProductId` int NOT NULL,
    `CartId` int NOT NULL,
    `Quantity` int NOT NULL,
    CONSTRAINT `PK_Cart_Items` PRIMARY KEY (`CartId`, `ProductId`),
    CONSTRAINT `FK_Cart_Items_Carts_CartId` FOREIGN KEY (`CartId`) REFERENCES `Carts` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Cart_Items_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `Order_Items` (
    `OrderId` int NOT NULL,
    `ProductId` int NOT NULL,
    `Quantity` int NOT NULL,
    `Price` decimal(65,30) NOT NULL,
    CONSTRAINT `PK_Order_Items` PRIMARY KEY (`OrderId`, `ProductId`),
    CONSTRAINT `FK_Order_Items_Orders_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `Orders` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Order_Items_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

INSERT INTO `Coupons` (`Id`, `Code`, `Discount`)
VALUES (1, '10OFF', 10.0),
(2, '20OFF', 20.0),
(3, '30OFF', 30.0);

INSERT INTO `Roles` (`Id`, `Name`)
VALUES (1, 'Admin'),
(2, 'Employee'),
(3, 'Shopper');

INSERT INTO `SecurityQuestions` (`Id`, `Question`)
VALUES (1, 'What is your favorite color?'),
(2, 'What is your favorite food?'),
(3, 'What is your favorite movie?');

CREATE INDEX `IX_Cart_Items_ProductId` ON `Cart_Items` (`ProductId`);

CREATE INDEX `IX_Carts_CouponId` ON `Carts` (`CouponId`);

CREATE INDEX `IX_Carts_UserId` ON `Carts` (`UserId`);

CREATE UNIQUE INDEX `IX_Inventory_ProductId` ON `Inventory` (`ProductId`);

CREATE INDEX `IX_Order_Items_ProductId` ON `Order_Items` (`ProductId`);

CREATE INDEX `IX_Orders_CouponId` ON `Orders` (`CouponId`);

CREATE INDEX `IX_Orders_UserId` ON `Orders` (`UserId`);

CREATE INDEX `IX_Products_CategoryId` ON `Products` (`CategoryId`);

CREATE INDEX `IX_Ratings_ProductId` ON `Ratings` (`ProductId`);

CREATE INDEX `IX_Ratings_UserId` ON `Ratings` (`UserId`);

CREATE INDEX `IX_Reviews_ProductId` ON `Reviews` (`ProductId`);

CREATE INDEX `IX_Reviews_UserId` ON `Reviews` (`UserId`);

CREATE INDEX `IX_Users_RoleId` ON `Users` (`RoleId`);

CREATE INDEX `IX_Users_SecurityQuestionId` ON `Users` (`SecurityQuestionId`);

CREATE INDEX `IX_Wishlists_ProductId` ON `Wishlists` (`ProductId`);

CREATE INDEX `IX_Wishlists_UserId` ON `Wishlists` (`UserId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250323053825_wishlistFix', '8.0.13');

COMMIT;

START TRANSACTION;

ALTER TABLE `Carts` MODIFY COLUMN `CouponId` int NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250331030021_MakeCouponOptionalInCart', '8.0.13');

COMMIT;

START TRANSACTION;

ALTER TABLE `Orders` MODIFY COLUMN `CouponId` int NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250331055632_MakeCouponOptionalInOrder', '8.0.13');

COMMIT;

START TRANSACTION;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250331061329_MakeCouponNavigatonNullable', '8.0.13');

COMMIT;

