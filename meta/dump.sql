CREATE DATABASE  IF NOT EXISTS `db35` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `db35`;
-- MySQL dump 10.13  Distrib 8.0.36, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: db35
-- ------------------------------------------------------
-- Server version	8.3.0

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `category`
--

DROP TABLE IF EXISTS `category`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `category` (
  `CategoryId` int NOT NULL AUTO_INCREMENT,
  `CategoryName` varchar(100) NOT NULL,
  PRIMARY KEY (`CategoryId`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `category`
--

LOCK TABLES `category` WRITE;
/*!40000 ALTER TABLE `category` DISABLE KEYS */;
INSERT INTO `category` VALUES (1,'Хлеб'),(2,'Булки и багеты'),(3,'Круассаны'),(4,'Пироги'),(5,'Печенье'),(6,'Пирожные'),(7,'Конфеты'),(8,'Пряники'),(9,'Торты'),(10,'Напитки');
/*!40000 ALTER TABLE `category` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `order`
--

DROP TABLE IF EXISTS `order`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `order` (
  `OrderId` int NOT NULL AUTO_INCREMENT,
  `OrderWorkerId` int NOT NULL,
  `OrderDate` date NOT NULL,
  `OrderStatus` enum('Новый','Проведён') NOT NULL,
  PRIMARY KEY (`OrderId`),
  KEY `OrderFK1_idx` (`OrderWorkerId`),
  CONSTRAINT `OrderFK1` FOREIGN KEY (`OrderWorkerId`) REFERENCES `worker` (`WorkerId`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `order`
--

LOCK TABLES `order` WRITE;
/*!40000 ALTER TABLE `order` DISABLE KEYS */;
INSERT INTO `order` VALUES (1,1,'2025-02-21','Проведён'),(2,1,'2025-02-21','Новый'),(3,4,'2025-02-20','Новый'),(4,7,'2025-02-20','Новый'),(5,9,'2025-02-20','Новый'),(7,1,'2025-03-04','Проведён'),(8,7,'2025-03-13','Новый'),(9,7,'2025-03-13','Новый'),(10,7,'2025-03-13','Новый');
/*!40000 ALTER TABLE `order` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `orderitem`
--

DROP TABLE IF EXISTS `orderitem`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `orderitem` (
  `OrderItemOrderId` int NOT NULL,
  `OrderItemProductId` int NOT NULL,
  `OrderItemQuantity` int NOT NULL,
  `OrderItemCost` float NOT NULL,
  PRIMARY KEY (`OrderItemOrderId`,`OrderItemProductId`),
  KEY `OrderItemFK2_idx` (`OrderItemProductId`),
  CONSTRAINT `OrderItemFK1` FOREIGN KEY (`OrderItemOrderId`) REFERENCES `order` (`OrderId`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `OrderItemFK2` FOREIGN KEY (`OrderItemProductId`) REFERENCES `product` (`ProductId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `orderitem`
--

LOCK TABLES `orderitem` WRITE;
/*!40000 ALTER TABLE `orderitem` DISABLE KEYS */;
INSERT INTO `orderitem` VALUES (1,1,3,29),(1,3,3,27),(1,5,1,40),(2,2,2,25),(2,4,5,31),(3,15,2,41),(3,21,3,108),(4,1,7,37),(5,2,3,46),(5,7,1,35),(7,2,1,25),(7,3,1,27),(7,6,1,36.63),(8,1,1,30),(9,2,2,25),(10,3,1,27);
/*!40000 ALTER TABLE `orderitem` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `product`
--

DROP TABLE IF EXISTS `product`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `product` (
  `ProductId` int NOT NULL AUTO_INCREMENT,
  `ProductName` varchar(100) NOT NULL,
  `ProductQuantity` int NOT NULL,
  `ProductDiscount` tinyint NOT NULL,
  `ProductCost` float NOT NULL,
  `ProductCategoryId` int NOT NULL,
  `ProductSupplierId` int NOT NULL,
  `ProductExpirationDate` date NOT NULL,
  `ProductImage` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`ProductId`),
  KEY `fk1_idx` (`ProductCategoryId`),
  KEY `ProductFK2_idx` (`ProductSupplierId`),
  CONSTRAINT `ProductFK1` FOREIGN KEY (`ProductCategoryId`) REFERENCES `category` (`CategoryId`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `ProductFK2` FOREIGN KEY (`ProductSupplierId`) REFERENCES `supplier` (`SupplierId`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=51 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `product`
--

LOCK TABLES `product` WRITE;
/*!40000 ALTER TABLE `product` DISABLE KEYS */;
INSERT INTO `product` VALUES (1,'Белый хлеб',2,0,30,1,1,'2025-02-23',''),(2,'Черный хлеб',12,0,25,1,2,'2025-02-23',''),(3,'Багет',17,0,27,2,3,'2025-02-23','baguette.jpg'),(4,'Кукурузный хлеб',7,0,33,1,1,'2025-02-23',NULL),(5,'Круассан',7,0,37,3,2,'2025-02-23',''),(6,'Круассан с шоколадом',5,1,37,3,3,'2025-02-23',''),(7,'Круассан с вар. сгущёнкой',7,0,37,3,1,'2025-02-23',NULL),(8,'Булочка с шоколадом',3,1,40,2,2,'2025-02-23',NULL),(9,'Булочка с изюмом',5,3,38,2,3,'2025-02-23',NULL),(10,'Булочка с корицей',2,0,40,2,1,'2025-02-23',NULL),(11,'Багет чесночный ',6,0,27,2,2,'2025-02-23',NULL),(12,'Багетка',5,0,20,2,3,'2025-02-23',NULL),(13,'Багетка чесночная',4,0,20,2,1,'2025-02-23',NULL),(14,'Чесночный хлеб',7,0,30,1,2,'2025-02-23',NULL),(15,'Пирожок с творогом',9,0,34,4,3,'2025-02-23',NULL),(16,'Пирожок с луком и яйцом',12,0,37,4,1,'2025-02-23',NULL),(17,'Пирожок с вишней',6,0,34,4,2,'2025-02-23',NULL),(18,'Пирожок с капустой',10,0,37,4,3,'2025-02-23',NULL),(19,'Пирожок с ветчиной и сыром',8,0,37,4,1,'2025-02-23',NULL),(20,'Пирожок с мясом',8,0,37,4,2,'2025-02-23',NULL),(21,'Печенье овсяное',10,1,110,5,3,'2025-04-21',NULL),(22,'Печенье кукурузное',10,0,95,5,1,'2025-04-21',NULL),(23,'Печенье \"К кофе\"',12,0,100,5,2,'2025-04-21',NULL),(24,'Печенье \"Сормовское\"',8,0,100,5,3,'2025-04-21',NULL),(25,'Печенье \"К чаю\"',6,1,100,5,1,'2025-04-21',NULL),(26,'Заварной эклер',25,1,70,6,1,'2025-03-21',NULL),(27,'Медово-ореховое пирожное',14,1,85,6,1,'2025-03-21',NULL),(28,'Пирожное картошка',20,3,80,6,2,'2025-03-21',NULL),(29,'Суфле',15,1,65,6,2,'2025-03-21',NULL),(30,'Шоколадное пирожное',12,0,80,6,3,'2025-03-21',NULL),(31,'Макарони',8,1,60,6,1,'2025-05-21',NULL),(32,'Конфеты \"Коровка\"',2700,1,2.5,7,2,'2025-08-21',NULL),(33,'Конфеты \"Птичье молоко\"',2570,0,2.7,7,2,'2025-08-21',NULL),(34,'Драже',500,0,3,7,2,'2025-08-21',NULL),(35,'Конфеты \"Твикс\"',800,0,5.2,7,1,'2025-08-21',NULL),(36,'Городецкий пряник',10,0,55,8,1,'2025-08-21',NULL),(37,'Имбирный пряник',10,0,57,8,1,'2025-08-21',NULL),(38,'Пряничный человечек',8,0,52,8,2,'2025-08-21',NULL),(39,'Торт \"Наполеон\"',0,1,325,9,2,'2025-08-21',NULL),(40,'Торт \"Медовик\"',3,0,335,9,3,'2025-08-21',NULL),(41,'Торт \"Муравейник\"',1,0,315,9,3,'2025-08-21',NULL),(42,'Торт \"Тирамису\"',0,1,300,9,2,'2025-08-21',NULL),(43,'Торт \"Чизкейк\"',0,0,270,9,1,'2025-08-21',NULL),(44,'Торт \"Красный бархат\"',1,1,380,9,1,'2025-08-21',NULL),(45,'Торт \"Фруктовое чудо\"',1,2,405,9,2,'2025-08-21',NULL),(46,'Добрый Кола',8,0,45,10,3,'2025-08-21',NULL),(47,'Кола из Черноголовки',6,1,43,10,3,'2025-08-21',NULL),(48,'Сок Добрый яблоко',8,0,40,10,2,'2025-08-21',NULL),(49,'Вода негаз. \"Святой источник\"',11,0,39,10,1,'2025-08-21',NULL),(50,'Сок Добрый мультифрукт',8,0,40,10,1,'2025-08-21',NULL);
/*!40000 ALTER TABLE `product` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `role`
--

DROP TABLE IF EXISTS `role`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `role` (
  `RoleId` int NOT NULL AUTO_INCREMENT,
  `RoleName` varchar(30) NOT NULL,
  PRIMARY KEY (`RoleId`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `role`
--

LOCK TABLES `role` WRITE;
/*!40000 ALTER TABLE `role` DISABLE KEYS */;
INSERT INTO `role` VALUES (1,'Сотрудник'),(2,'Менеджер'),(3,'Администратор');
/*!40000 ALTER TABLE `role` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `supplier`
--

DROP TABLE IF EXISTS `supplier`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `supplier` (
  `SupplierId` int NOT NULL AUTO_INCREMENT,
  `SupplierName` varchar(100) NOT NULL,
  PRIMARY KEY (`SupplierId`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `supplier`
--

LOCK TABLES `supplier` WRITE;
/*!40000 ALTER TABLE `supplier` DISABLE KEYS */;
INSERT INTO `supplier` VALUES (1,'СЕЙМОВСКИЕ МЕЛЬНИЦЫ'),(2,'Агроплемкомбинат МИР'),(3,'Выкса-Опт');
/*!40000 ALTER TABLE `supplier` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user`
--

DROP TABLE IF EXISTS `user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user` (
  `UserId` int NOT NULL AUTO_INCREMENT,
  `UserLogin` varchar(100) NOT NULL,
  `UserPassword` varchar(64) NOT NULL,
  `UserRoleId` int NOT NULL,
  `UserWorkerId` int NOT NULL,
  PRIMARY KEY (`UserId`),
  KEY `fk1_idx` (`UserRoleId`),
  KEY `UserFK2_idx` (`UserWorkerId`),
  CONSTRAINT `UserFK1` FOREIGN KEY (`UserRoleId`) REFERENCES `role` (`RoleId`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `UserFK2` FOREIGN KEY (`UserWorkerId`) REFERENCES `worker` (`WorkerId`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user`
--

LOCK TABLES `user` WRITE;
/*!40000 ALTER TABLE `user` DISABLE KEYS */;
INSERT INTO `user` VALUES (1,'c','2e7d2c03a9507ae265ecf5b5356885a53393a2029d241394997265a1a25aefc6',1,4),(2,'m','62c66a7a5dd70c3146618063c344e531e6d4b59e379808443ce962b3abd63c5a',2,7),(3,'a','ca978112ca1bbdcafac231b39a23dc4da786eff8147c4e72b9807785afee48bb',3,1),(4,'cooluser123','a1159e9df3670d549d04524532629f5477ceb7deec9b45e47e8c009506ecb2c8',1,3),(5,'man52','a1159e9df3670d549d04524532629f5477ceb7deec9b45e47e8c009506ecb2c8',2,5),(6,'ihaveverybadpwd','a1159e9df3670d549d04524532629f5477ceb7deec9b45e47e8c009506ecb2c8',1,2),(7,'genius','a1159e9df3670d549d04524532629f5477ceb7deec9b45e47e8c009506ecb2c8',1,9),(8,'master','a1159e9df3670d549d04524532629f5477ceb7deec9b45e47e8c009506ecb2c8',3,6),(9,'user35','a1159e9df3670d549d04524532629f5477ceb7deec9b45e47e8c009506ecb2c8',1,8),(10,'aaa','a1159e9df3670d549d04524532629f5477ceb7deec9b45e47e8c009506ecb2c8',2,10);
/*!40000 ALTER TABLE `user` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `worker`
--

DROP TABLE IF EXISTS `worker`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `worker` (
  `WorkerId` int NOT NULL AUTO_INCREMENT,
  `WorkerSurname` varchar(30) NOT NULL,
  `WorkerName` varchar(30) NOT NULL,
  `WorkerPatronymic` varchar(30) DEFAULT NULL,
  `WorkerPhone` varchar(11) NOT NULL,
  PRIMARY KEY (`WorkerId`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `worker`
--

LOCK TABLES `worker` WRITE;
/*!40000 ALTER TABLE `worker` DISABLE KEYS */;
INSERT INTO `worker` VALUES (1,'Родионова','Тамара','Витальевна','89300100020'),(2,'Щербаков','Владимир','Матвеевич','89012345678'),(3,'Третьяков','Фёдор','Вадимович','89103254769'),(4,'Суворова','Валерия','Борисовна','89996667775'),(5,'Маслов','Дмитрий','Иванович','89001112233'),(6,'Виноградов','Созон','Арсеньевич','89907856341'),(7,'Бурова','Наина','Брониславовна','89753186420'),(8,'Игнатьев','Игнатий','Антонович','89774552171'),(9,'Давыдов','Яков','Антонович','89020158686'),(10,'Блинова','Ангелина','Владленовна','89525252525');
/*!40000 ALTER TABLE `worker` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-03-31 13:49:15
