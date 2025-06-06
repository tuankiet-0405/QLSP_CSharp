-- Add diverse product categories and products for better AI testing
-- Insert new categories
INSERT INTO Categories (Name, Description) VALUES 
('Áo thun nam', 'Áo thun dành cho nam giới'),
('Áo thun nữ', 'Áo thun dành cho nữ giới'),
('Quần jeans', 'Quần jeans thời trang'),
('Giày thể thao', 'Giày thể thao các loại'),
('Túi xách', 'Túi xách thời trang'),
('Điện thoại', 'Điện thoại di động'),
('Laptop', 'Máy tính xách tay'),
('Tai nghe', 'Tai nghe âm thanh');

-- Insert new products for clothing
INSERT INTO Products (Name, Description, Price, CategoryId, StockQuantity, ImageUrl) VALUES 
-- Áo thun nam
('Áo thun nam cổ tròn Polo', 'Áo thun nam cotton 100%, form regular fit, màu trắng', 299000, (SELECT Id FROM Categories WHERE Name = 'Áo thun nam'), 50, '/images/ao-thun-nam-polo.jpg'),
('Áo thun nam Uniqlo', 'Áo thun cotton mềm mại, co giãn tốt, đa dạng màu sắc', 199000, (SELECT Id FROM Categories WHERE Name = 'Áo thun nam'), 75, '/images/ao-thun-uniqlo.jpg'),
('Áo thun nam Nike Dri-FIT', 'Áo thun thể thao chống ẩm, phù hợp tập luyện', 599000, (SELECT Id FROM Categories WHERE Name = 'Áo thun nam'), 30, '/images/ao-thun-nike.jpg'),

-- Áo thun nữ  
('Áo thun nữ crop top', 'Áo thun nữ form crop, chất liệu cotton blend', 249000, (SELECT Id FROM Categories WHERE Name = 'Áo thun nữ'), 40, '/images/ao-thun-nu-crop.jpg'),
('Áo thun nữ Zara basic', 'Áo thun basic dáng rộng, màu be trendy', 359000, (SELECT Id FROM Categories WHERE Name = 'Áo thun nữ'), 60, '/images/ao-thun-zara.jpg'),

-- Quần jeans
('Quần jeans nam slim fit', 'Quần jeans nam form slim, màu xanh đậm classic', 799000, (SELECT Id FROM Categories WHERE Name = 'Quần jeans'), 35, '/images/quan-jeans-nam.jpg'),
('Quần jeans nữ skinny', 'Quần jeans nữ ôm body, chất denim cao cấp', 899000, (SELECT Id FROM Categories WHERE Name = 'Quần jeans'), 45, '/images/quan-jeans-nu.jpg'),

-- Giày thể thao
('Giày Nike Air Force 1', 'Giày thể thao classic, da trắng tinh khôi', 2799000, (SELECT Id FROM Categories WHERE Name = 'Giày thể thao'), 25, '/images/nike-air-force.jpg'),
('Giày Adidas Ultraboost', 'Giày chạy bộ công nghệ Boost, êm ái tối đa', 4599000, (SELECT Id FROM Categories WHERE Name = 'Giày thể thao'), 20, '/images/adidas-ultraboost.jpg'),
('Giày Converse Chuck Taylor', 'Giày canvas cổ điển, phong cách vintage', 1299000, (SELECT Id FROM Categories WHERE Name = 'Giày thể thao'), 55, '/images/converse-chuck.jpg'),

-- Túi xách
('Túi xách nữ Charles & Keith', 'Túi xách da thật, thiết kế thanh lịch', 1899000, (SELECT Id FROM Categories WHERE Name = 'Túi xách'), 15, '/images/tui-charles-keith.jpg'),
('Balo laptop Samsonite', 'Balo đựng laptop 15.6 inch, chống nước', 2299000, (SELECT Id FROM Categories WHERE Name = 'Túi xách'), 30, '/images/balo-samsonite.jpg'),

-- Điện thoại
('iPhone 15 Pro Max', 'Điện thoại Apple mới nhất, chip A17 Pro', 34990000, (SELECT Id FROM Categories WHERE Name = 'Điện thoại'), 10, '/images/iphone-15-pro.jpg'),
('Samsung Galaxy S24 Ultra', 'Flagship Android với bút S Pen, camera 200MP', 31990000, (SELECT Id FROM Categories WHERE Name = 'Điện thoại'), 12, '/images/galaxy-s24.jpg'),
('Xiaomi 14 Pro', 'Điện thoại Xiaomi cao cấp, chip Snapdragon 8 Gen 3', 18990000, (SELECT Id FROM Categories WHERE Name = 'Điện thoại'), 20, '/images/xiaomi-14.jpg'),

-- Laptop
('MacBook Air M3', 'Laptop Apple chip M3, 13 inch, 256GB SSD', 28990000, (SELECT Id FROM Categories WHERE Name = 'Laptop'), 8, '/images/macbook-air-m3.jpg'),
('Dell XPS 13', 'Laptop Windows premium, InfinityEdge display', 24990000, (SELECT Id FROM Categories WHERE Name = 'Laptop'), 15, '/images/dell-xps13.jpg'),
('Asus ROG Gaming', 'Laptop gaming RTX 4060, Intel Core i7', 32990000, (SELECT Id FROM Categories WHERE Name = 'Laptop'), 6, '/images/asus-rog.jpg'),

-- Tai nghe
('AirPods Pro 2', 'Tai nghe Apple không dây, chống ồn chủ động', 6990000, (SELECT Id FROM Categories WHERE Name = 'Tai nghe'), 25, '/images/airpods-pro.jpg'),
('Sony WH-1000XM5', 'Tai nghe chụp tai cao cấp, chống ồn tuyệt vời', 8990000, (SELECT Id FROM Categories WHERE Name = 'Tai nghe'), 18, '/images/sony-wh1000xm5.jpg'),
('JBL Tune 510BT', 'Tai nghe Bluetooth giá rẻ, âm thanh JBL', 1299000, (SELECT Id FROM Categories WHERE Name = 'Tai nghe'), 40, '/images/jbl-tune510.jpg');

-- Add some product reviews for the new products
INSERT INTO ProductReviews (ProductId, UserId, Rating, Comment, CreatedAt) VALUES
-- Reviews for clothing
((SELECT Id FROM Products WHERE Name = 'Áo thun nam cổ tròn Polo'), (SELECT TOP 1 Id FROM AspNetUsers), 5, 'Chất liệu cotton rất mềm mại, mặc rất thoải mái', GETDATE()),
((SELECT Id FROM Products WHERE Name = 'Áo thun nam Nike Dri-FIT'), (SELECT TOP 1 Id FROM AspNetUsers), 4, 'Tốt cho tập gym, thấm hút mồ hôi tốt', GETDATE()),
((SELECT Id FROM Products WHERE Name = 'Quần jeans nam slim fit'), (SELECT TOP 1 Id FROM AspNetUsers), 4, 'Form dáng đẹp, chất denim bền', GETDATE()),

-- Reviews for electronics  
((SELECT Id FROM Products WHERE Name = 'iPhone 15 Pro Max'), (SELECT TOP 1 Id FROM AspNetUsers), 5, 'Camera tuyệt vời, performance mượt mà', GETDATE()),
((SELECT Id FROM Products WHERE Name = 'MacBook Air M3'), (SELECT TOP 1 Id FROM AspNetUsers), 5, 'Nhanh, nhẹ, pin trâu, rất hài lòng', GETDATE()),
((SELECT Id FROM Products WHERE Name = 'AirPods Pro 2'), (SELECT TOP 1 Id FROM AspNetUsers), 4, 'Chất lượng âm thanh tốt, chống ồn hiệu quả', GETDATE()),

-- Reviews for footwear
((SELECT Id FROM Products WHERE Name = 'Giày Nike Air Force 1'), (SELECT TOP 1 Id FROM AspNetUsers), 5, 'Giày classic, đi được mọi outfit', GETDATE()),
((SELECT Id FROM Products WHERE Name = 'Giày Adidas Ultraboost'), (SELECT TOP 1 Id FROM AspNetUsers), 5, 'Êm ái nhất từng đi, rất tốt cho chạy bộ', GETDATE());

GO
