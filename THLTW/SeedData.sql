-- Thêm dữ liệu mẫu cho database QLSP_THW
USE QLSP_THW;

-- Thêm Categories
INSERT INTO Categories (Name) VALUES 
(N'Đồng hồ nam'),
(N'Đồng hồ nữ'),
(N'Đồng hồ thể thao'),
(N'Đồng hồ cao cấp'),
(N'Phụ kiện đồng hồ');

-- Thêm Products
INSERT INTO Products (Name, Price, Description, ImageUrl, CategoryId) VALUES 
(N'Rolex Submariner', 250000000, N'Đồng hồ lặn cao cấp với khả năng chống nước 300m', '/images/Products/rolex-submariner.jpg', 4),
(N'Casio G-Shock', 3500000, N'Đồng hồ thể thao chống sốc, chống nước', '/images/Products/casio-gshock.jpg', 3),
(N'Omega Speedmaster', 180000000, N'Đồng hồ chronograph nổi tiếng, từng lên mặt trăng', '/images/Products/omega-speedmaster.jpg', 1),
(N'Citizen Eco-Drive', 4500000, N'Đồng hồ năng lượng mặt trời cho nữ', '/images/Products/citizen-ecodrive.jpg', 2),
(N'Seiko Automatic', 8500000, N'Đồng hồ cơ tự động Nhật Bản', '/images/Products/seiko-automatic.jpg', 1),
(N'Apple Watch Series 9', 12000000, N'Smartwatch thông minh với nhiều tính năng sức khỏe', '/images/Products/apple-watch.jpg', 3),
(N'Tag Heuer Formula 1', 25000000, N'Đồng hồ thể thao cao cấp phong cách đua xe', '/images/Products/tagheuer-formula1.jpg', 3),
(N'Tissot PRC 200', 7500000, N'Đồng hồ Swiss Made chống nước 200m', '/images/Products/tissot-prc200.jpg', 1),
(N'Fossil Grant', 3200000, N'Đồng hồ vintage với thiết kế cổ điển', '/images/Products/fossil-grant.jpg', 1),
(N'Michael Kors Runway', 4800000, N'Đồng hồ thời trang nữ với đá pha lê', '/images/Products/mk-runway.jpg', 2);

-- Thêm ProductAdditionalImages
INSERT INTO ProductAdditionalImages (ProductId, ImageUrl) VALUES 
(1, '/images/Products/rolex-submariner-side.jpg'),
(1, '/images/Products/rolex-submariner-back.jpg'),
(2, '/images/Products/casio-gshock-side.jpg'),
(3, '/images/Products/omega-speedmaster-detail.jpg'),
(4, '/images/Products/citizen-ecodrive-side.jpg'),
(5, '/images/Products/seiko-automatic-movement.jpg'),
(6, '/images/Products/apple-watch-bands.jpg'),
(7, '/images/Products/tagheuer-formula1-detail.jpg'),
(8, '/images/Products/tissot-prc200-side.jpg'),
(9, '/images/Products/fossil-grant-back.jpg'),
(10, '/images/Products/mk-runway-detail.jpg');
