-- Seed AI Training Data for THLTW E-commerce Platform
-- This script creates sample data for machine learning model training

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

-- First, let's create some sample users (using ASP.NET Identity structure)
INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, SecurityStamp, ConcurrencyStamp, AccessFailedCount, LockoutEnabled, TwoFactorEnabled, PhoneNumberConfirmed, CreatedAt)
VALUES 
('11111111-1111-1111-1111-111111111111', 'customer1@test.com', 'CUSTOMER1@TEST.COM', 'customer1@test.com', 'CUSTOMER1@TEST.COM', 1, NEWID(), NEWID(), 0, 1, 0, 0, GETDATE()),
('22222222-2222-2222-2222-222222222222', 'customer2@test.com', 'CUSTOMER2@TEST.COM', 'customer2@test.com', 'CUSTOMER2@TEST.COM', 1, NEWID(), NEWID(), 0, 1, 0, 0, GETDATE()),
('33333333-3333-3333-3333-333333333333', 'customer3@test.com', 'CUSTOMER3@TEST.COM', 'customer3@test.com', 'CUSTOMER3@TEST.COM', 1, NEWID(), NEWID(), 0, 1, 0, 0, GETDATE());

-- Sample User Activities for recommendation system training
INSERT INTO UserActivities (UserId, ActivityType, ProductId, ActivityData, Timestamp) VALUES
('11111111-1111-1111-1111-111111111111', 'View', 1, '{"page":"ProductDetails","duration":45}', DATEADD(day, -30, GETDATE())),
('11111111-1111-1111-1111-111111111111', 'AddToCart', 1, '{"quantity":1,"price":25000000}', DATEADD(day, -29, GETDATE())),
('11111111-1111-1111-1111-111111111111', 'View', 2, '{"page":"ProductDetails","duration":32}', DATEADD(day, -28, GETDATE())),
('22222222-2222-2222-2222-222222222222', 'View', 1, '{"page":"ProductDetails","duration":67}', DATEADD(day, -25, GETDATE())),
('22222222-2222-2222-2222-222222222222', 'View', 3, '{"page":"ProductDetails","duration":23}', DATEADD(day, -24, GETDATE())),
('33333333-3333-3333-3333-333333333333', 'Search', NULL, '{"query":"luxury watch","results":5}', DATEADD(day, -20, GETDATE()));

-- Sample Product View History for analytics
INSERT INTO ProductViewHistories (UserId, ProductId, ViewedAt, SessionId, IpAddress, ViewDurationSeconds) VALUES
('11111111-1111-1111-1111-111111111111', 1, DATEADD(day, -30, GETDATE()), 'sess_001', '192.168.1.100', 45),
('11111111-1111-1111-1111-111111111111', 2, DATEADD(day, -28, GETDATE()), 'sess_001', '192.168.1.100', 32),
('22222222-2222-2222-2222-222222222222', 1, DATEADD(day, -25, GETDATE()), 'sess_002', '192.168.1.101', 67),
('22222222-2222-2222-2222-222222222222', 3, DATEADD(day, -24, GETDATE()), 'sess_002', '192.168.1.101', 23),
(NULL, 1, DATEADD(day, -15, GETDATE()), 'sess_003', '192.168.1.102', 12), -- Anonymous user
(NULL, 2, DATEADD(day, -14, GETDATE()), 'sess_003', '192.168.1.102', 8); -- Anonymous user

-- Sample Orders for purchase history analysis
INSERT INTO Orders (UserId, OrderDate, Status, TotalAmount, ShippingAddress, CustomerName, CustomerPhone) VALUES
('11111111-1111-1111-1111-111111111111', DATEADD(day, -29, GETDATE()), 'Completed', 25000000.00, '123 Main St, City, Country', 'Customer One', '123-456-7890'),
('22222222-2222-2222-2222-222222222222', DATEADD(day, -20, GETDATE()), 'Completed', 45000000.00, '456 Oak Ave, City, Country', 'Customer Two', '234-567-8901'),
('33333333-3333-3333-3333-333333333333', DATEADD(day, -10, GETDATE()), 'Pending', 35000000.00, '789 Pine St, City, Country', 'Customer Three', '345-678-9012');

-- Sample Order Items for detailed purchase analysis
INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES
(1, 1, 1, 25000000.00),
(2, 2, 1, 35000000.00),
(2, 3, 1, 10000000.00),
(3, 1, 1, 25000000.00),
(3, 3, 1, 10000000.00);

-- Sample Product Reviews for sentiment analysis and rating prediction
INSERT INTO ProductReviews (UserId, ProductId, Rating, Comment, CreatedAt) VALUES
('11111111-1111-1111-1111-111111111111', 1, 5, 'Excellent watch! Great quality and beautiful design. Highly recommended!', DATEADD(day, -25, GETDATE())),
('22222222-2222-2222-2222-222222222222', 2, 4, 'Good watch, but delivery was a bit slow. Overall satisfied with the purchase.', DATEADD(day, -15, GETDATE())),
('22222222-2222-2222-2222-222222222222', 3, 5, 'Amazing value for money. The watch looks much more expensive than it is.', DATEADD(day, -12, GETDATE())),
('33333333-3333-3333-3333-333333333333', 1, 4, 'Nice watch, but could use better packaging. Product itself is great.', DATEADD(day, -8, GETDATE()));

PRINT 'AI Training sample data with users inserted successfully!';