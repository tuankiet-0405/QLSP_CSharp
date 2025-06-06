// AI Search Enhancement JavaScript for Vietnamese Interface
class AISearchEnhancer {
    constructor() {
        this.initializeEnhancements();
        this.setupEventListeners();
    }

    initializeEnhancements() {
        this.addSearchSuggestions();
        this.enhanceSearchInputs();
        this.addQuickFilters();
        this.setupKeyboardShortcuts();
    }

    addSearchSuggestions() {
        // Add search suggestions functionality
        const searchInputs = document.querySelectorAll('input[type="search"], input[placeholder*="tìm"]');
        
        searchInputs.forEach(input => {
            this.createSuggestionContainer(input);
        });
    }

    createSuggestionContainer(input) {
        const container = document.createElement('div');
        container.className = 'search-suggestions-container';
        container.innerHTML = `
            <div class="suggestions-list" id="suggestions-${this.generateId()}">
                <!-- Suggestions will be populated here -->
            </div>
        `;
        
        input.parentNode.appendChild(container);
        
        // Add input event listener for real-time suggestions
        input.addEventListener('input', (e) => {
            this.fetchSuggestions(e.target.value, container.querySelector('.suggestions-list'));
        });
        
        // Hide suggestions when clicking outside
        document.addEventListener('click', (e) => {
            if (!input.contains(e.target) && !container.contains(e.target)) {
                container.style.display = 'none';
            }
        });
        
        // Show suggestions when focusing on input
        input.addEventListener('focus', () => {
            if (input.value.trim()) {
                container.style.display = 'block';
            }
        });
    }

    async fetchSuggestions(query, suggestionsContainer) {
        if (query.length < 2) {
            suggestionsContainer.innerHTML = '';
            suggestionsContainer.parentNode.style.display = 'none';
            return;
        }

        try {
            const response = await fetch('/AITrending/GetSearchSuggestions', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
                },
                body: JSON.stringify({ partialQuery: query })
            });

            if (response.ok) {
                const suggestions = await response.json();
                this.renderSuggestions(suggestions, suggestionsContainer);
            }
        } catch (error) {
            console.error('Error fetching suggestions:', error);
        }
    }

    renderSuggestions(suggestions, container) {
        if (!suggestions || !suggestions.suggestions || suggestions.suggestions.length === 0) {
            container.innerHTML = '<div class="suggestion-item no-results">Không có gợi ý</div>';
            container.parentNode.style.display = 'block';
            return;
        }

        const suggestionItems = suggestions.suggestions.map(suggestion => `
            <div class="suggestion-item" data-query="${suggestion}">
                <i class="fas fa-search me-2"></i>
                <span class="suggestion-text">${suggestion}</span>
                <span class="suggestion-type">Gợi ý</span>
            </div>
        `).join('');

        container.innerHTML = suggestionItems;
        container.parentNode.style.display = 'block';

        // Add click handlers for suggestions
        container.querySelectorAll('.suggestion-item').forEach(item => {
            item.addEventListener('click', () => {
                const query = item.getAttribute('data-query');
                const searchInput = container.closest('.search-suggestions-container').previousElementSibling;
                if (searchInput) {
                    searchInput.value = query;
                    this.performSearch(query);
                }
                container.parentNode.style.display = 'none';
            });
        });
    }

    enhanceSearchInputs() {
        const searchInputs = document.querySelectorAll('input[type="search"], input[placeholder*="tìm"]');
        
        searchInputs.forEach(input => {
            // Add search icon if not present
            if (!input.parentNode.querySelector('.search-icon')) {
                const icon = document.createElement('i');
                icon.className = 'fas fa-search search-icon';
                input.parentNode.appendChild(icon);
            }
            
            // Add clear button
            const clearBtn = document.createElement('button');
            clearBtn.type = 'button';
            clearBtn.className = 'btn search-clear-btn';
            clearBtn.innerHTML = '<i class="fas fa-times"></i>';
            clearBtn.title = 'Xóa';
            clearBtn.style.display = input.value ? 'block' : 'none';
            
            clearBtn.addEventListener('click', () => {
                input.value = '';
                input.focus();
                clearBtn.style.display = 'none';
                this.clearSearchResults();
            });
            
            input.addEventListener('input', () => {
                clearBtn.style.display = input.value ? 'block' : 'none';
            });
            
            input.parentNode.appendChild(clearBtn);
        });
    }

    addQuickFilters() {
        const quickFilters = [
            { text: 'Phổ biến', filter: 'popular' },
            { text: 'Mới nhất', filter: 'newest' },
            { text: 'Giá thấp', filter: 'price-low' },
            { text: 'Giá cao', filter: 'price-high' },
            { text: 'Đánh giá cao', filter: 'rating' }
        ];

        const filterContainer = document.createElement('div');
        filterContainer.className = 'quick-filters-container';
        filterContainer.innerHTML = `
            <div class="quick-filters">
                <span class="filter-label">Lọc nhanh:</span>
                ${quickFilters.map(filter => `
                    <button class="btn btn-outline-primary btn-sm filter-btn" data-filter="${filter.filter}">
                        ${filter.text}
                    </button>
                `).join('')}
            </div>
        `;

        // Insert after search inputs
        const searchForm = document.querySelector('form[role="search"], .search-form');
        if (searchForm) {
            searchForm.parentNode.insertBefore(filterContainer, searchForm.nextSibling);
            
            // Add event listeners for filter buttons
            filterContainer.querySelectorAll('.filter-btn').forEach(btn => {
                btn.addEventListener('click', () => {
                    // Remove active class from other buttons
                    filterContainer.querySelectorAll('.filter-btn').forEach(b => b.classList.remove('active'));
                    // Add active class to clicked button
                    btn.classList.add('active');
                    // Apply filter
                    this.applyQuickFilter(btn.getAttribute('data-filter'));
                });
            });
        }
    }

    setupKeyboardShortcuts() {
        document.addEventListener('keydown', (e) => {
            // Ctrl + K for quick search
            if (e.ctrlKey && e.key === 'k') {
                e.preventDefault();
                const searchInput = document.querySelector('input[type="search"], input[placeholder*="tìm"]');
                if (searchInput) {
                    searchInput.focus();
                    searchInput.select();
                }
            }
            
            // Ctrl + / for voice search
            if (e.ctrlKey && e.key === '/') {
                e.preventDefault();
                if (window.voiceSearchManager) {
                    window.voiceSearchManager.startListening();
                }
            }
            
            // Escape to clear search
            if (e.key === 'Escape') {
                const searchInput = document.querySelector('input[type="search"]:focus, input[placeholder*="tìm"]:focus');
                if (searchInput) {
                    searchInput.value = '';
                    this.clearSearchResults();
                }
            }
        });
    }

    async performSearch(query) {
        try {
            this.showSearchLoading();
            
            const response = await fetch('/AITrending/SmartSearch', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
                },
                body: JSON.stringify({ query: query, maxResults: 20 })
            });

            if (response.ok) {
                const results = await response.json();
                this.displaySearchResults(results);
                this.hideSearchLoading();
            } else {
                throw new Error('Search failed');
            }
        } catch (error) {
            console.error('Search error:', error);
            this.showSearchError('Lỗi tìm kiếm. Vui lòng thử lại.');
            this.hideSearchLoading();
        }
    }

    applyQuickFilter(filterType) {
        // Implement quick filter logic
        const currentQuery = document.querySelector('input[type="search"], input[placeholder*="tìm"]')?.value || '';
        
        const filterQueries = {
            'popular': currentQuery + ' phổ biến',
            'newest': currentQuery + ' mới nhất',
            'price-low': currentQuery + ' giá rẻ',
            'price-high': currentQuery + ' cao cấp',
            'rating': currentQuery + ' đánh giá cao'
        };
        
        const enhancedQuery = filterQueries[filterType] || currentQuery;
        this.performSearch(enhancedQuery);
    }

    showSearchLoading() {
        const loader = document.getElementById('search-loader') || this.createSearchLoader();
        loader.style.display = 'block';
    }

    hideSearchLoading() {
        const loader = document.getElementById('search-loader');
        if (loader) {
            loader.style.display = 'none';
        }
    }

    createSearchLoader() {
        const loader = document.createElement('div');
        loader.id = 'search-loader';
        loader.className = 'search-loader';
        loader.innerHTML = `
            <div class="search-loading-content">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Đang tìm kiếm...</span>
                </div>
                <div class="loading-text">Đang tìm kiếm...</div>
            </div>
        `;
        document.body.appendChild(loader);
        return loader;
    }

    displaySearchResults(results) {
        // Create or update search results container
        let resultsContainer = document.getElementById('search-results');
        if (!resultsContainer) {
            resultsContainer = document.createElement('div');
            resultsContainer.id = 'search-results';
            resultsContainer.className = 'search-results-container';
            
            // Insert after main content or search form
            const mainContent = document.querySelector('main, .container');
            if (mainContent) {
                mainContent.appendChild(resultsContainer);
            }
        }

        if (!results || results.length === 0) {
            resultsContainer.innerHTML = `
                <div class="no-results">
                    <i class="fas fa-search fa-3x text-muted mb-3"></i>
                    <h3>Không tìm thấy kết quả</h3>
                    <p>Hãy thử tìm kiếm với từ khóa khác</p>
                </div>
            `;
            return;
        }

        const resultsHTML = `
            <div class="search-results-header">
                <h3>Kết quả tìm kiếm (${results.length})</h3>
            </div>
            <div class="results-grid">
                ${results.map(product => `
                    <div class="result-item" data-product-id="${product.id}">
                        <div class="product-image">
                            <img src="${product.imageUrl || '/images/no-image.png'}" alt="${product.name}" loading="lazy">
                        </div>
                        <div class="product-info">
                            <h4 class="product-name">${product.name}</h4>
                            <p class="product-description">${product.description || ''}</p>
                            <div class="product-price">${this.formatPrice(product.price)}</div>
                            <div class="product-actions">
                                <button class="btn btn-primary btn-sm" onclick="addToCart(${product.id})">
                                    <i class="fas fa-cart-plus"></i> Thêm vào giỏ
                                </button>
                                <button class="btn btn-outline-secondary btn-sm" onclick="viewProduct(${product.id})">
                                    <i class="fas fa-eye"></i> Xem chi tiết
                                </button>
                            </div>
                        </div>
                    </div>
                `).join('')}
            </div>
        `;

        resultsContainer.innerHTML = resultsHTML;
        resultsContainer.scrollIntoView({ behavior: 'smooth' });
    }

    formatPrice(price) {
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        }).format(price);
    }

    clearSearchResults() {
        const resultsContainer = document.getElementById('search-results');
        if (resultsContainer) {
            resultsContainer.innerHTML = '';
        }
    }

    showSearchError(message) {
        const errorContainer = document.getElementById('search-error') || this.createErrorContainer();
        errorContainer.innerHTML = `
            <div class="alert alert-danger alert-dismissible fade show">
                <i class="fas fa-exclamation-triangle me-2"></i>
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        `;
        errorContainer.style.display = 'block';
    }

    createErrorContainer() {
        const container = document.createElement('div');
        container.id = 'search-error';
        container.className = 'search-error-container';
        document.body.appendChild(container);
        return container;
    }

    generateId() {
        return Math.random().toString(36).substr(2, 9);
    }

    setupEventListeners() {
        // Listen for voice search results
        document.addEventListener('voiceSearchResults', (event) => {
            this.displaySearchResults(event.detail.results);
        });

        // Listen for language changes
        document.addEventListener('languageChanged', (event) => {
            this.updateUILanguage(event.detail.language);
        });
    }

    updateUILanguage(language) {
        // Update placeholder texts based on language
        const searchInputs = document.querySelectorAll('input[type="search"], input[placeholder*="tìm"]');
        const placeholder = language === 'vi-VN' ? 'Tìm kiếm sản phẩm...' : 'Search products...';
        
        searchInputs.forEach(input => {
            input.placeholder = placeholder;
        });

        // Update quick filter labels
        const filterLabels = {
            'vi-VN': {
                'popular': 'Phổ biến',
                'newest': 'Mới nhất', 
                'price-low': 'Giá thấp',
                'price-high': 'Giá cao',
                'rating': 'Đánh giá cao'
            },
            'en-US': {
                'popular': 'Popular',
                'newest': 'Newest',
                'price-low': 'Price Low',
                'price-high': 'Price High', 
                'rating': 'Top Rated'
            }
        };

        const filterButtons = document.querySelectorAll('.filter-btn');
        filterButtons.forEach(btn => {
            const filter = btn.getAttribute('data-filter');
            if (filterLabels[language] && filterLabels[language][filter]) {
                btn.textContent = filterLabels[language][filter];
            }
        });
    }
}

// CSS Styles for AI Search Enhancements
const aiSearchStyles = `
<style>
    .search-suggestions-container {
        position: relative;
        display: none;
        z-index: 1000;
    }
    
    .suggestions-list {
        position: absolute;
        top: 5px;
        left: 0;
        right: 0;
        background: white;
        border: 1px solid #ddd;
        border-radius: 8px;
        box-shadow: 0 4px 15px rgba(0,0,0,0.1);
        max-height: 300px;
        overflow-y: auto;
    }
    
    .suggestion-item {
        display: flex;
        align-items: center;
        padding: 12px 15px;
        cursor: pointer;
        border-bottom: 1px solid #f0f0f0;
        transition: all 0.2s ease;
    }
    
    .suggestion-item:hover {
        background-color: #f8f9fa;
        color: #667eea;
    }
    
    .suggestion-item:last-child {
        border-bottom: none;
    }
    
    .suggestion-text {
        flex: 1;
        font-weight: 500;
    }
    
    .suggestion-type {
        font-size: 0.8em;
        color: #666;
        background: #e9ecef;
        padding: 2px 8px;
        border-radius: 12px;
    }
    
    .suggestion-item.no-results {
        color: #666;
        font-style: italic;
        justify-content: center;
    }
    
    .quick-filters-container {
        margin: 15px 0;
    }
    
    .quick-filters {
        display: flex;
        align-items: center;
        gap: 10px;
        flex-wrap: wrap;
    }
    
    .filter-label {
        font-weight: 600;
        color: #333;
        margin-right: 10px;
    }
    
    .filter-btn {
        border-radius: 20px;
        transition: all 0.3s ease;
    }
    
    .filter-btn.active {
        background-color: #667eea;
        border-color: #667eea;
        color: white;
    }
    
    .filter-btn:hover {
        transform: translateY(-2px);
        box-shadow: 0 4px 8px rgba(102, 126, 234, 0.3);
    }
    
    .search-clear-btn {
        position: absolute;
        right: 40px;
        top: 50%;
        transform: translateY(-50%);
        border: none;
        background: none;
        color: #666;
        padding: 5px;
        border-radius: 50%;
        width: 30px;
        height: 30px;
        display: none;
    }
    
    .search-clear-btn:hover {
        background: #f0f0f0;
        color: #333;
    }
    
    .search-loader {
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        background: rgba(0, 0, 0, 0.3);
        display: none;
        align-items: center;
        justify-content: center;
        z-index: 9999;
    }
    
    .search-loading-content {
        background: white;
        padding: 30px;
        border-radius: 15px;
        text-align: center;
        box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
    }
    
    .loading-text {
        margin-top: 15px;
        font-weight: 500;
        color: #333;
    }
    
    .search-results-container {
        margin-top: 30px;
    }
    
    .search-results-header {
        border-bottom: 2px solid #667eea;
        padding-bottom: 10px;
        margin-bottom: 20px;
    }
    
    .results-grid {
        display: grid;
        grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
        gap: 20px;
    }
    
    .result-item {
        border: 1px solid #e0e0e0;
        border-radius: 12px;
        overflow: hidden;
        transition: all 0.3s ease;
        background: white;
    }
    
    .result-item:hover {
        transform: translateY(-5px);
        box-shadow: 0 8px 25px rgba(0, 0, 0, 0.1);
        border-color: #667eea;
    }
    
    .product-image {
        height: 200px;
        overflow: hidden;
        background: #f8f9fa;
    }
    
    .product-image img {
        width: 100%;
        height: 100%;
        object-fit: cover;
        transition: transform 0.3s ease;
    }
    
    .result-item:hover .product-image img {
        transform: scale(1.05);
    }
    
    .product-info {
        padding: 15px;
    }
    
    .product-name {
        font-size: 1.1em;
        font-weight: 600;
        margin-bottom: 8px;
        color: #333;
        line-height: 1.3;
    }
    
    .product-description {
        color: #666;
        font-size: 0.9em;
        margin-bottom: 10px;
        display: -webkit-box;
        -webkit-line-clamp: 2;
        -webkit-box-orient: vertical;
        overflow: hidden;
    }
    
    .product-price {
        font-size: 1.2em;
        font-weight: 700;
        color: #667eea;
        margin-bottom: 15px;
    }
    
    .product-actions {
        display: flex;
        gap: 8px;
    }
    
    .product-actions .btn {
        flex: 1;
        font-size: 0.85em;
    }
    
    .no-results {
        text-align: center;
        padding: 60px 20px;
        color: #666;
    }
    
    .search-error-container {
        position: fixed;
        top: 20px;
        right: 20px;
        z-index: 10000;
        max-width: 400px;
    }
    
    @media (max-width: 768px) {
        .quick-filters {
            flex-direction: column;
            align-items: flex-start;
        }
        
        .results-grid {
            grid-template-columns: 1fr;
        }
        
        .product-actions {
            flex-direction: column;
        }
    }
</style>
`;

// Add styles to document
document.head.insertAdjacentHTML('beforeend', aiSearchStyles);

// Initialize AI Search Enhancer
let aiSearchEnhancer;
document.addEventListener('DOMContentLoaded', function() {
    aiSearchEnhancer = new AISearchEnhancer();
});

// Global access
window.aiSearchEnhancer = aiSearchEnhancer;
