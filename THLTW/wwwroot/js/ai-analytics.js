// Real-time AI Analytics Dashboard for THLTW
class AIAnalyticsDashboard {
    constructor() {
        this.charts = {};
        this.updateInterval = 30000; // 30 seconds
        this.intervalId = null;
        this.isRunning = false;
        this.initializeDashboard();
    }

    initializeDashboard() {
        // Check if Chart.js is loaded
        if (typeof Chart === 'undefined') {
            console.warn('Chart.js is required for AI Analytics Dashboard');
            this.loadChartJS();
            return;
        }

        this.setupCharts();
        this.startRealTimeUpdates();
    }

    loadChartJS() {
        const script = document.createElement('script');
        script.src = 'https://cdn.jsdelivr.net/npm/chart.js';
        script.onload = () => {
            this.setupCharts();
            this.startRealTimeUpdates();
        };
        document.head.appendChild(script);
    }

    setupCharts() {
        this.setupSearchTrendsChart();
        this.setupPopularProductsChart();
        this.setupUserActivityChart();
        this.setupAIPerformanceChart();
    }

    setupSearchTrendsChart() {
        const ctx = document.getElementById('searchTrendsChart');
        if (!ctx) return;

        this.charts.searchTrends = new Chart(ctx, {
            type: 'line',
            data: {
                labels: [],
                datasets: [{
                    label: 'Tìm kiếm AI',
                    data: [],
                    borderColor: '#667eea',
                    backgroundColor: 'rgba(102, 126, 234, 0.1)',
                    tension: 0.4,
                    fill: true
                }, {
                    label: 'Tìm kiếm thường',
                    data: [],
                    borderColor: '#764ba2',
                    backgroundColor: 'rgba(118, 75, 162, 0.1)',
                    tension: 0.4,
                    fill: true
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    title: {
                        display: true,
                        text: 'Xu hướng tìm kiếm theo thời gian thực'
                    },
                    legend: {
                        position: 'top'
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        title: {
                            display: true,
                            text: 'Số lượng tìm kiếm'
                        }
                    },
                    x: {
                        title: {
                            display: true,
                            text: 'Thời gian'
                        }
                    }
                }
            }
        });
    }

    setupPopularProductsChart() {
        const ctx = document.getElementById('popularProductsChart');
        if (!ctx) return;

        this.charts.popularProducts = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: [],
                datasets: [{
                    data: [],
                    backgroundColor: [
                        '#667eea',
                        '#764ba2',
                        '#f093fb',
                        '#f5576c',
                        '#4facfe',
                        '#00f2fe',
                        '#43e97b',
                        '#38f9d7'
                    ],
                    borderWidth: 2,
                    borderColor: '#fff'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    title: {
                        display: true,
                        text: 'Sản phẩm được tìm kiếm nhiều nhất'
                    },
                    legend: {
                        position: 'right'
                    }
                }
            }
        });
    }

    setupUserActivityChart() {
        const ctx = document.getElementById('userActivityChart');
        if (!ctx) return;

        this.charts.userActivity = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: ['Tìm kiếm', 'Xem sản phẩm', 'Thêm giỏ hàng', 'Mua hàng', 'Chat AI'],
                datasets: [{
                    label: 'Hoạt động người dùng',
                    data: [],
                    backgroundColor: [
                        'rgba(102, 126, 234, 0.8)',
                        'rgba(118, 75, 162, 0.8)',
                        'rgba(240, 147, 251, 0.8)',
                        'rgba(245, 87, 108, 0.8)',
                        'rgba(79, 172, 254, 0.8)'
                    ],
                    borderColor: [
                        '#667eea',
                        '#764ba2',
                        '#f093fb',
                        '#f5576c',
                        '#4facfe'
                    ],
                    borderWidth: 2
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    title: {
                        display: true,
                        text: 'Hoạt động người dùng trong 24h qua'
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        title: {
                            display: true,
                            text: 'Số lượng hoạt động'
                        }
                    }
                }
            }
        });
    }

    setupAIPerformanceChart() {
        const ctx = document.getElementById('aiPerformanceChart');
        if (!ctx) return;

        this.charts.aiPerformance = new Chart(ctx, {
            type: 'radar',
            data: {
                labels: ['Độ chính xác', 'Tốc độ phản hồi', 'Sự hài lòng', 'Tỷ lệ chuyển đổi', 'Hiệu quả gợi ý'],
                datasets: [{
                    label: 'Hiệu suất AI',
                    data: [],
                    backgroundColor: 'rgba(102, 126, 234, 0.2)',
                    borderColor: '#667eea',
                    pointBackgroundColor: '#667eea',
                    pointBorderColor: '#fff',
                    pointHoverBackgroundColor: '#fff',
                    pointHoverBorderColor: '#667eea'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    title: {
                        display: true,
                        text: 'Hiệu suất AI tổng thể'
                    }
                },
                scales: {
                    r: {
                        beginAtZero: true,
                        max: 100,
                        ticks: {
                            stepSize: 20
                        }
                    }
                }
            }
        });
    }

    async updateAllCharts() {
        try {
            const analytics = await this.fetchAIAnalytics();
            if (!analytics) return;

            this.updateSearchTrendsChart(analytics.searchTrends);
            this.updatePopularProductsChart(analytics.popularProducts);
            this.updateUserActivityChart(analytics.userActivity);
            this.updateAIPerformanceChart(analytics.aiPerformance);
            this.updateStatsCards(analytics.stats);
            
            this.updateLastRefreshTime();
        } catch (error) {
            console.error('Error updating charts:', error);
            this.showError('Lỗi cập nhật dữ liệu: ' + error.message);
        }
    }

    async fetchAIAnalytics() {
        try {
            const response = await fetch('/AITrending/GetAIAnalytics', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
                }
            });

            if (!response.ok) {
                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
            }

            return await response.json();
        } catch (error) {
            console.error('Failed to fetch AI analytics:', error);
            return null;
        }
    }

    updateSearchTrendsChart(data) {
        if (!this.charts.searchTrends || !data) return;

        const chart = this.charts.searchTrends;
        chart.data.labels = data.labels || [];
        chart.data.datasets[0].data = data.aiSearches || [];
        chart.data.datasets[1].data = data.regularSearches || [];
        chart.update('none');
    }

    updatePopularProductsChart(data) {
        if (!this.charts.popularProducts || !data) return;

        const chart = this.charts.popularProducts;
        chart.data.labels = data.productNames || [];
        chart.data.datasets[0].data = data.searchCounts || [];
        chart.update('none');
    }

    updateUserActivityChart(data) {
        if (!this.charts.userActivity || !data) return;

        const chart = this.charts.userActivity;
        chart.data.datasets[0].data = data.activityCounts || [];
        chart.update('none');
    }

    updateAIPerformanceChart(data) {
        if (!this.charts.aiPerformance || !data) return;

        const chart = this.charts.aiPerformance;
        chart.data.datasets[0].data = data.performanceScores || [];
        chart.update('none');
    }

    updateStatsCards(stats) {
        if (!stats) return;

        // Update stat cards
        const statElements = {
            'total-searches': stats.totalSearches,
            'ai-accuracy': stats.aiAccuracy + '%',
            'avg-response-time': stats.avgResponseTime + 'ms',
            'user-satisfaction': stats.userSatisfaction + '%'
        };

        Object.entries(statElements).forEach(([id, value]) => {
            const element = document.getElementById(id);
            if (element) {
                this.animateValue(element, value);
            }
        });
    }

    animateValue(element, newValue) {
        const currentValue = element.textContent;
        if (currentValue !== newValue.toString()) {
            element.style.transition = 'all 0.3s ease';
            element.style.transform = 'scale(1.1)';
            element.textContent = newValue;
            
            setTimeout(() => {
                element.style.transform = 'scale(1)';
            }, 300);
        }
    }

    updateLastRefreshTime() {
        const timeElement = document.getElementById('last-refresh-time');
        if (timeElement) {
            const now = new Date();
            timeElement.textContent = now.toLocaleTimeString('vi-VN');
        }
    }

    startRealTimeUpdates() {
        if (this.isRunning) return;

        this.isRunning = true;
        this.updateAllCharts(); // Initial update
        
        this.intervalId = setInterval(() => {
            this.updateAllCharts();
        }, this.updateInterval);

        this.updateControlButtons();
    }

    stopRealTimeUpdates() {
        if (!this.isRunning) return;

        this.isRunning = false;
        if (this.intervalId) {
            clearInterval(this.intervalId);
            this.intervalId = null;
        }

        this.updateControlButtons();
    }

    updateControlButtons() {
        const startBtn = document.getElementById('start-updates');
        const stopBtn = document.getElementById('stop-updates');
        const statusIndicator = document.getElementById('update-status');

        if (startBtn) startBtn.disabled = this.isRunning;
        if (stopBtn) stopBtn.disabled = !this.isRunning;
        
        if (statusIndicator) {
            statusIndicator.textContent = this.isRunning ? 'Đang cập nhật' : 'Đã dừng';
            statusIndicator.className = `badge ${this.isRunning ? 'bg-success' : 'bg-secondary'}`;
        }
    }

    showError(message) {
        const errorContainer = document.getElementById('error-container');
        if (errorContainer) {
            errorContainer.innerHTML = `
                <div class="alert alert-danger alert-dismissible fade show" role="alert">
                    <i class="fas fa-exclamation-triangle me-2"></i>
                    ${message}
                    <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                </div>
            `;
        }
    }

    exportData() {
        // Export current analytics data
        this.fetchAIAnalytics().then(data => {
            if (data) {
                const blob = new Blob([JSON.stringify(data, null, 2)], { type: 'application/json' });
                const url = URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;
                a.download = `ai-analytics-${new Date().toISOString().split('T')[0]}.json`;
                document.body.appendChild(a);
                a.click();
                document.body.removeChild(a);
                URL.revokeObjectURL(url);
            }
        });
    }
}

// Initialize dashboard when DOM is loaded
let aiDashboard;
document.addEventListener('DOMContentLoaded', function() {
    aiDashboard = new AIAnalyticsDashboard();
    
    // Bind control buttons
    document.getElementById('start-updates')?.addEventListener('click', () => {
        aiDashboard.startRealTimeUpdates();
    });
    
    document.getElementById('stop-updates')?.addEventListener('click', () => {
        aiDashboard.stopRealTimeUpdates();
    });
    
    document.getElementById('refresh-now')?.addEventListener('click', () => {
        aiDashboard.updateAllCharts();
    });
    
    document.getElementById('export-data')?.addEventListener('click', () => {
        aiDashboard.exportData();
    });
});

// Global access
window.aiDashboard = aiDashboard;
