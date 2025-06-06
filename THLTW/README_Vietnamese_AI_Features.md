# THLTW E-commerce - Vietnamese AI Interface Features

## Tổng quan (Overview)

THLTW (Tích Hợp Học Tập Thông Minh) là một ứng dụng thương mại điện tử với các tính năng AI tiên tiến, được tối ưu hóa cho người dùng Việt Nam với giao diện hoàn toàn bằng tiếng Việt.

## 🎯 Tính năng AI đã triển khai (Implemented AI Features)

### 1. 🔍 Tìm kiếm AI thông minh (Smart AI Search)
- **Tìm kiếm ngữ nghĩa**: Hiểu ngữ cảnh và ý nghĩa câu truy vấn tiếng Việt
- **Tìm kiếm mờ**: Xử lý lỗi chính tả và từ đồng nghĩa
- **Gợi ý tự động**: Đề xuất từ khóa và sản phẩm liên quan
- **Lịch sử tìm kiếm**: Lưu trữ và phân tích hành vi tìm kiếm

### 2. 🎙️ Tìm kiếm bằng giọng nói (Voice Search)
- **Nhận dạng giọng nói tiếng Việt**: Hỗ trợ nhận dạng giọng nói với độ chính xác cao
- **Chuyển đổi Speech-to-Text**: Tích hợp Web Speech API
- **Giao diện trực quan**: Hiệu ứng sóng âm và phản hồi real-time
- **Đa ngôn ngữ**: Hỗ trợ chuyển đổi giữa tiếng Việt và tiếng Anh

### 3. 🖼️ Tìm kiếm bằng hình ảnh (Image Search)
- **AI Vision**: Phân tích nội dung hình ảnh để tìm sản phẩm tương tự
- **Nhận dạng đối tượng**: Xác định sản phẩm từ hình ảnh tải lên
- **Tìm kiếm tương tự**: Gợi ý sản phẩm có đặc điểm giống nhau

### 4. 💬 Trợ lý AI Chat (AI Chat Assistant)
- **Chatbot thông minh**: Trả lời câu hỏi về sản phẩm bằng tiếng Việt
- **Tư vấn mua sắm**: Đưa ra gợi ý dựa trên nhu cầu khách hàng
- **Hỗ trợ 24/7**: Luôn sẵn sàng hỗ trợ khách hàng
- **Ngữ cảnh đối thoại**: Ghi nhớ lịch sử cuộc trò chuyện

### 5. 📈 Phân tích xu hướng AI (AI Trending Analytics)
- **Theo dõi real-time**: Cập nhật dữ liệu mỗi 30 giây
- **Biểu đồ tương tác**: Sử dụng Chart.js cho visualization
- **Xuất dữ liệu**: Tính năng export báo cáo JSON
- **Dashboard tổng quan**: Hiển thị KPIs và metrics quan trọng

### 6. 🌐 Đa ngôn ngữ (Internationalization)
- **Tiếng Việt mặc định**: Giao diện hoàn toàn tiếng Việt
- **Chuyển đổi ngôn ngữ**: Toggle giữa tiếng Việt và tiếng Anh
- **Localization**: Sử dụng .resx files cho đa ngôn ngữ
- **Culture-aware**: Tự động phát hiện ngôn ngữ của trình duyệt

## 🏗️ Kiến trúc hệ thống (System Architecture)

### Backend Components

#### 1. AISearchService (`Services/AISearchService.cs`)
```csharp
public interface IAISearchService
{
    Task<List<Product>> SmartSearchAsync(string query, int maxResults = 10);
    Task<List<Product>> SearchByImageAsync(string imageUrl, int maxResults = 10);
    Task<AISearchSuggestion> GetSearchSuggestionsAsync(string partialQuery);
    Task<string> GenerateChatResponseAsync(string userMessage, string? userId = null);
    Task<AIAnalytics> GetAIAnalyticsAsync();
    Task LogSearchQueryAsync(string query, string? userId, int resultCount);
}
```

#### 2. AITrendingController (`Controllers/AITrendingController.cs`)
- **POST /AITrending/SmartSearch**: Tìm kiếm thông minh
- **POST /AITrending/Chat**: Trò chuyện với AI
- **POST /AITrending/VoiceSearch**: Tìm kiếm bằng giọng nói
- **GET /AITrending/TrendingProducts**: Sản phẩm thịnh hành
- **GET /AITrending/GetAIAnalytics**: Phân tích AI

#### 3. Database Models
```csharp
public class SearchLog
{
    public int Id { get; set; }
    public string Query { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public int ResultCount { get; set; }
    public DateTime Timestamp { get; set; }
}
```

### Frontend Components

#### 1. Voice Search (`wwwroot/js/voice-search.js`)
- Class `VoiceSearchManager`
- Speech Recognition API integration
- Real-time voice indicator
- Multi-language support

#### 2. AI Analytics Dashboard (`wwwroot/js/ai-analytics.js`)
- Class `AIAnalyticsDashboard`
- Chart.js integration
- Real-time data updates
- Export functionality

#### 3. Language Switch (`Views/Shared/_LanguageSwitch.cshtml`)
- Dropdown với flag icons
- Cookie-based language persistence
- Seamless switching experience

## 📊 Database Schema

### SearchLogs Table
```sql
CREATE TABLE [SearchLogs] (
    [Id] int NOT NULL IDENTITY,
    [Query] nvarchar(max) NOT NULL,
    [UserId] nvarchar(max) NULL,
    [ResultCount] int NOT NULL,
    [Timestamp] datetime2 NOT NULL,
    CONSTRAINT [PK_SearchLogs] PRIMARY KEY ([Id])
);
```

## 🎨 UI/UX Features

### 1. Modern Design
- **Gradient backgrounds**: AI-themed color schemes
- **Responsive layout**: Bootstrap 5 integration
- **Smooth animations**: CSS transitions và transforms
- **Vietnamese typography**: Tối ưu cho tiếng Việt

### 2. Interactive Elements
- **Voice indicator**: Animated sound waves
- **Search suggestions**: Dropdown với auto-complete
- **Loading states**: Progress indicators và spinners
- **Toast notifications**: Success/error feedback

### 3. Accessibility
- **ARIA labels**: Screen reader support
- **Keyboard navigation**: Tab-friendly interface
- **High contrast**: Color accessibility
- **Font scaling**: Responsive typography

## 🚀 Cách sử dụng (Usage Instructions)

### 1. Khởi động ứng dụng
```bash
cd THLTW
dotnet run
```

### 2. Truy cập các tính năng AI
- **Trang chủ**: http://localhost:5264
- **AI Dashboard**: Menu "AI" → "Phân tích AI"
- **AI Trending**: Menu "AI" → "AI Trending"

### 3. Sử dụng Voice Search
1. Click vào icon microphone bên cạnh ô tìm kiếm
2. Cho phép truy cập microphone khi được yêu cầu
3. Nói rõ ràng tên sản phẩm cần tìm
4. Kết quả sẽ hiển thị tự động

### 4. Chat với AI
1. Vào phần "AI Dashboard"
2. Nhập câu hỏi trong ô chat
3. Nhận phản hồi tức thì từ AI assistant

## 🔧 Cấu hình (Configuration)

### 1. Localization Settings (`Program.cs`)
```csharp
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en-US"),
        new CultureInfo("vi-VN")
    };
    
    options.DefaultRequestCulture = new RequestCulture("vi-VN");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});
```

### 2. Database Connection
Cập nhật connection string trong `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=THLTW;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### 3. AI Service Configuration
```csharp
// Program.cs
builder.Services.AddScoped<IAISearchService, AISearchService>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
```

## 📈 Performance Optimizations

### 1. Caching Strategy
- **Memory caching**: Cho search results
- **Response caching**: Cho static content
- **Database query optimization**: Index trên SearchLogs

### 2. Real-time Updates
- **Efficient polling**: 30-second intervals
- **Incremental updates**: Chỉ cập nhật dữ liệu thay đổi
- **Background processing**: Non-blocking operations

### 3. SEO & Vietnamese Content
- **Meta tags**: Tiếng Việt SEO-friendly
- **URL structure**: Clean và descriptive
- **Sitemap**: Bao gồm tất cả pages

## 🔮 Tính năng tương lai (Future Enhancements)

### 1. Advanced AI Features
- **Machine Learning Models**: Huấn luyện trên dữ liệu Việt Nam
- **Recommendation Engine**: Cá nhân hóa based on behavior
- **Sentiment Analysis**: Phân tích cảm xúc từ reviews

### 2. Integration Opportunities
- **Payment Gateways**: VNPay, MoMo, ZaloPay
- **Shipping APIs**: GHN, GHTK, Viettel Post
- **Social Login**: Facebook, Google, Zalo

### 3. Mobile App
- **React Native**: Cross-platform mobile app
- **Push Notifications**: Real-time alerts
- **Offline Mode**: Cached product data

## 🤝 Đóng góp (Contributing)

### 1. Development Setup
```bash
git clone [repository-url]
cd THLTW
dotnet restore
dotnet ef database update
dotnet run
```

### 2. Code Standards
- **C# Conventions**: Microsoft coding standards
- **JavaScript**: ES6+ với async/await
- **CSS**: BEM methodology
- **Vietnamese Comments**: Cho business logic

### 3. Testing
- **Unit Tests**: xUnit framework
- **Integration Tests**: ASP.NET Core TestHost
- **UI Tests**: Selenium WebDriver

## 📞 Hỗ trợ (Support)

Để được hỗ trợ hoặc báo cáo lỗi, vui lòng:
1. Tạo issue trên GitHub repository
2. Cung cấp thông tin chi tiết về lỗi
3. Bao gồm screenshots nếu có thể
4. Mô tả các bước tái tạo lỗi

---

**Phiên bản**: 1.0.0  
**Cập nhật lần cuối**: May 31, 2025  
**Tác giả**: THLTW Development Team  
**Giấy phép**: MIT License
