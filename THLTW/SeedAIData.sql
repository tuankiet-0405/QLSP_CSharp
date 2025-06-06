-- Sample data for AI training
-- This script adds sample reviews, orders, and user activities for testing AI features

-- Sample user activities (anonymous and logged in)
INSERT INTO UserActivities (UserId, ProductId, ActivityType, ActivityData, Timestamp, SessionId, IpAddress) VALUES
('user1@example.com', 1, 'View', '{"duration": 45, "source": "search"}', DATEADD(day, -30, GETDATE()), 'session1', '192.168.1.1'),
('user1@example.com', 1, 'AddToCart', '{"quantity": 1}', DATEADD(day, -30, GETDATE()), 'session1', '192.168.1.1'),
('user1@example.com', 2, 'View', '{"duration": 32, "source": "recommendation"}', DATEADD(day, -29, GETDATE()), 'session2', '192.168.1.1'),
('user2@example.com', 3, 'View', '{"duration": 78, "source": "category"}', DATEADD(day, -28, GETDATE()), 'session3', '192.168.1.2'),
('user2@example.com', 3, 'AddToCart', '{"quantity": 2}', DATEADD(day, -28, GETDATE()), 'session3', '192.168.1.2'),
('user2@example.com', 4, 'Search', '{"term": "luxury watch", "results": 15}', DATEADD(day, -27, GETDATE()), 'session4', '192.168.1.2');

-- Sample product view history
INSERT INTO ProductViewHistories (UserId, ProductId, ViewedAt, SessionId, IpAddress, ViewDurationSeconds) VALUES
('user1@example.com', 1, DATEADD(day, -30, GETDATE()), 'session1', '192.168.1.1', 45),
('user1@example.com', 2, DATEADD(day, -29, GETDATE()), 'session2', '192.168.1.1', 32),
('user2@example.com', 3, DATEADD(day, -28, GETDATE()), 'session3', '192.168.1.2', 78),
('user2@example.com', 1, DATEADD(day, -27, GETDATE()), 'session4', '192.168.1.2', 52),
(NULL, 4, DATEADD(day, -26, GETDATE()), 'anonymous1', '192.168.1.3', 25),
(NULL, 5, DATEADD(day, -25, GETDATE()), 'anonymous2', '192.168.1.4', 67);

-- Sample orders
INSERT INTO Orders (UserId, OrderDate, TotalAmount, Status, ShippingAddress, CustomerName, CustomerPhone) VALUES
('user1@example.com', DATEADD(day, -25, GETDATE()), 15000000.00, 'Delivered', '123 Nguyen Trai, Q1, HCM', 'Nguyen Van A', '0901234567'),
('user2@example.com', DATEADD(day, -20, GETDATE()), 8500000.00, 'Delivered', '456 Le Loi, Q3, HCM', 'Tran Thi B', '0907654321'),
('user1@example.com', DATEADD(day, -15, GETDATE()), 25000000.00, 'Shipped', '123 Nguyen Trai, Q1, HCM', 'Nguyen Van A', '0901234567'),
('user2@example.com', DATEADD(day, -10, GETDATE()), 12000000.00, 'Processing', '456 Le Loi, Q3, HCM', 'Tran Thi B', '0907654321');

-- Sample order items (assuming Orders start with ID 1)
INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES
-- Order 1: user1 bought Rolex and Omega
(1, 1, 1, 15000000.00), -- Rolex Submariner
-- Order 2: user2 bought Casio and Citizen  
(2, 2, 2, 4250000.00),  -- Casio G-Shock (2 units)
-- Order 3: user1 bought expensive Patek Philippe
(3, 8, 1, 25000000.00), -- Patek Philippe
-- Order 4: user2 bought TAG Heuer
(4, 6, 1, 12000000.00); -- TAG Heuer

-- Sample product reviews
INSERT INTO ProductReviews (UserId, ProductId, Rating, Comment, CreatedAt) VALUES
('user1@example.com', 1, 5, 'Excellent watch! Very satisfied with the quality and design.', DATEADD(day, -20, GETDATE())),
('user1@example.com', 8, 5, 'Luxury at its finest. Worth every penny!', DATEADD(day, -10, GETDATE())),
('user2@example.com', 2, 4, 'Good sports watch, durable and water resistant.', DATEADD(day, -15, GETDATE())),
('user2@example.com', 6, 4, 'Nice design but a bit pricey. Good build quality though.', DATEADD(day, -5, GETDATE())),
('user1@example.com', 2, 3, 'Decent watch but expected better for the price.', DATEADD(day, -18, GETDATE())),
('user2@example.com', 1, 5, 'Amazing craftsmanship! Highly recommended.', DATEADD(day, -12, GETDATE()));

-- Sample users in AspNetUsers (if not exist)
-- Note: You may need to run this manually after creating actual users through the Identity system
/*
INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, FirstName, LastName, CreatedAt) VALUES
('user1@example.com', 'user1@example.com', 'USER1@EXAMPLE.COM', 'user1@example.com', 'USER1@EXAMPLE.COM', 1, 'Nguyen', 'Van A', DATEADD(day, -60, GETDATE())),
('user2@example.com', 'user2@example.com', 'USER2@EXAMPLE.COM', 'user2@example.com', 'USER2@EXAMPLE.COM', 1, 'Tran', 'Thi B', DATEADD(day, -45, GETDATE()));
*/

PRINT 'AI Training sample data inserted successfully!';
