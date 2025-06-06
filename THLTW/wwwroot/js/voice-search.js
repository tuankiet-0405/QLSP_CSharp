// Voice Search functionality for THLTW AI Interface
class VoiceSearchManager {
    constructor() {
        this.recognition = null;
        this.isListening = false;
        this.currentLanguage = 'vi-VN'; // Default to Vietnamese
        this.initializeSpeechRecognition();
    }

    initializeSpeechRecognition() {
        // Check if browser supports speech recognition
        if (!('webkitSpeechRecognition' in window) && !('SpeechRecognition' in window)) {
            console.warn('Speech recognition not supported in this browser');
            return;
        }

        // Initialize speech recognition
        const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
        this.recognition = new SpeechRecognition();
        
        this.recognition.continuous = false;
        this.recognition.interimResults = false;
        this.recognition.maxAlternatives = 1;
        this.recognition.lang = this.currentLanguage;

        // Event handlers
        this.recognition.onstart = () => {
            this.isListening = true;
            this.updateVoiceButton(true);
            this.showVoiceIndicator();
        };

        this.recognition.onresult = (event) => {
            const transcript = event.results[0][0].transcript;
            this.handleVoiceResult(transcript);
        };

        this.recognition.onerror = (event) => {
            console.error('Speech recognition error:', event.error);
            this.stopListening();
            this.showErrorMessage('Lỗi nhận dạng giọng nói: ' + event.error);
        };

        this.recognition.onend = () => {
            this.stopListening();
        };
    }

    startListening() {
        if (!this.recognition) {
            this.showErrorMessage('Trình duyệt không hỗ trợ nhận dạng giọng nói');
            return;
        }

        if (this.isListening) {
            this.stopListening();
            return;
        }

        try {
            this.recognition.start();
        } catch (error) {
            console.error('Error starting speech recognition:', error);
            this.showErrorMessage('Không thể bắt đầu nhận dạng giọng nói');
        }
    }

    stopListening() {
        if (this.recognition && this.isListening) {
            this.recognition.stop();
        }
        this.isListening = false;
        this.updateVoiceButton(false);
        this.hideVoiceIndicator();
    }

    updateVoiceButton(listening) {
        const voiceButtons = document.querySelectorAll('.voice-search-btn');
        voiceButtons.forEach(button => {
            if (listening) {
                button.classList.add('listening');
                button.innerHTML = '<i class="fas fa-stop"></i>';
                button.title = 'Dừng ghi âm';
            } else {
                button.classList.remove('listening');
                button.innerHTML = '<i class="fas fa-microphone"></i>';
                button.title = 'Tìm kiếm bằng giọng nói';
            }
        });
    }

    showVoiceIndicator() {
        // Create or show voice indicator
        let indicator = document.getElementById('voice-indicator');
        if (!indicator) {
            indicator = document.createElement('div');
            indicator.id = 'voice-indicator';
            indicator.innerHTML = `
                <div class="voice-indicator-content">
                    <div class="voice-wave">
                        <span></span>
                        <span></span>
                        <span></span>
                        <span></span>
                        <span></span>
                    </div>
                    <div class="voice-text">Đang nghe...</div>
                    <button class="btn btn-sm btn-outline-danger" onclick="voiceSearchManager.stopListening()">
                        <i class="fas fa-times"></i> Dừng
                    </button>
                </div>
            `;
            document.body.appendChild(indicator);
        }
        indicator.style.display = 'flex';
    }

    hideVoiceIndicator() {
        const indicator = document.getElementById('voice-indicator');
        if (indicator) {
            indicator.style.display = 'none';
        }
    }

    handleVoiceResult(transcript) {
        console.log('Voice search transcript:', transcript);
        
        // Find search input and set the transcript
        const searchInputs = document.querySelectorAll('input[type="search"], input[placeholder*="tìm"], input[placeholder*="search"]');
        if (searchInputs.length > 0) {
            searchInputs[0].value = transcript;
            searchInputs[0].focus();
            
            // Trigger search if there's a form
            const form = searchInputs[0].closest('form');
            if (form) {
                form.dispatchEvent(new Event('submit'));
            } else {
                // If no form, trigger a custom search event
                this.performAISearch(transcript);
            }
        }

        this.showSuccessMessage(`Đã nhận dạng: "${transcript}"`);
    }

    async performAISearch(query) {
        try {
            const response = await fetch('/AITrending/SmartSearch', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
                },
                body: JSON.stringify({ query: query, maxResults: 10 })
            });

            if (response.ok) {
                const results = await response.json();
                this.displaySearchResults(results);
            } else {
                throw new Error('Search failed');
            }
        } catch (error) {
            console.error('AI Search error:', error);
            this.showErrorMessage('Lỗi tìm kiếm AI');
        }
    }

    displaySearchResults(results) {
        // Implementation for displaying search results
        console.log('Voice search results:', results);
        
        // Dispatch custom event with results
        const event = new CustomEvent('voiceSearchResults', { 
            detail: { results: results } 
        });
        document.dispatchEvent(event);
    }

    setLanguage(language) {
        this.currentLanguage = language;
        if (this.recognition) {
            this.recognition.lang = language;
        }
    }

    showSuccessMessage(message) {
        this.showToast(message, 'success');
    }

    showErrorMessage(message) {
        this.showToast(message, 'error');
    }

    showToast(message, type) {
        // Create toast notification
        const toast = document.createElement('div');
        toast.className = `voice-toast voice-toast-${type}`;
        toast.innerHTML = `
            <div class="voice-toast-content">
                <i class="fas fa-${type === 'success' ? 'check-circle' : 'exclamation-circle'}"></i>
                <span>${message}</span>
            </div>
        `;
        
        document.body.appendChild(toast);
        
        // Show toast
        setTimeout(() => toast.classList.add('show'), 100);
        
        // Remove toast after 3 seconds
        setTimeout(() => {
            toast.classList.remove('show');
            setTimeout(() => document.body.removeChild(toast), 300);
        }, 3000);
    }
}

// CSS styles for voice search
const voiceSearchStyles = `
<style>
    .voice-search-btn {
        border: none;
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        color: white;
        border-radius: 50%;
        width: 40px;
        height: 40px;
        display: flex;
        align-items: center;
        justify-content: center;
        transition: all 0.3s ease;
        box-shadow: 0 2px 10px rgba(102, 126, 234, 0.3);
    }
    
    .voice-search-btn:hover {
        transform: scale(1.1);
        box-shadow: 0 4px 20px rgba(102, 126, 234, 0.5);
        color: white;
    }
    
    .voice-search-btn.listening {
        background: linear-gradient(135deg, #ff6b6b 0%, #ee5a24 100%);
        animation: pulse 1.5s infinite;
    }
    
    @keyframes pulse {
        0% { transform: scale(1); }
        50% { transform: scale(1.1); }
        100% { transform: scale(1); }
    }
    
    #voice-indicator {
        position: fixed;
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
        z-index: 9999;
        background: rgba(0, 0, 0, 0.9);
        padding: 30px;
        border-radius: 15px;
        color: white;
        text-align: center;
        display: none;
        flex-direction: column;
        align-items: center;
        gap: 15px;
        box-shadow: 0 10px 30px rgba(0, 0, 0, 0.5);
    }
    
    .voice-wave {
        display: flex;
        gap: 3px;
        align-items: center;
        height: 40px;
    }
    
    .voice-wave span {
        width: 4px;
        height: 20px;
        background: #667eea;
        border-radius: 2px;
        animation: wave 1.2s infinite ease-in-out;
    }
    
    .voice-wave span:nth-child(2) { animation-delay: -1.1s; }
    .voice-wave span:nth-child(3) { animation-delay: -1.0s; }
    .voice-wave span:nth-child(4) { animation-delay: -0.9s; }
    .voice-wave span:nth-child(5) { animation-delay: -0.8s; }
    
    @keyframes wave {
        0%, 40%, 100% { 
            transform: scaleY(0.4);
            background: #667eea;
        }
        20% { 
            transform: scaleY(1.0);
            background: #764ba2;
        }
    }
    
    .voice-text {
        font-size: 16px;
        font-weight: 500;
        margin: 10px 0;
    }
    
    .voice-toast {
        position: fixed;
        top: 20px;
        right: 20px;
        z-index: 10000;
        padding: 15px 20px;
        border-radius: 8px;
        color: white;
        font-weight: 500;
        transform: translateX(100%);
        transition: transform 0.3s ease;
        box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
    }
    
    .voice-toast.show {
        transform: translateX(0);
    }
    
    .voice-toast-success {
        background: linear-gradient(135deg, #28a745 0%, #20c997 100%);
    }
    
    .voice-toast-error {
        background: linear-gradient(135deg, #dc3545 0%, #fd7e14 100%);
    }
    
    .voice-toast-content {
        display: flex;
        align-items: center;
        gap: 10px;
    }
</style>
`;

// Add styles to document
document.head.insertAdjacentHTML('beforeend', voiceSearchStyles);

// Initialize voice search manager when DOM is loaded
let voiceSearchManager;
document.addEventListener('DOMContentLoaded', function() {
    voiceSearchManager = new VoiceSearchManager();
    
    // Add voice search buttons to existing search inputs
    const searchContainers = document.querySelectorAll('.search-container, .input-group, form');
    searchContainers.forEach(container => {
        const searchInput = container.querySelector('input[type="search"], input[type="text"]');
        if (searchInput && !container.querySelector('.voice-search-btn')) {
            const voiceBtn = document.createElement('button');
            voiceBtn.type = 'button';
            voiceBtn.className = 'btn voice-search-btn';
            voiceBtn.innerHTML = '<i class="fas fa-microphone"></i>';
            voiceBtn.title = 'Tìm kiếm bằng giọng nói';
            voiceBtn.onclick = () => voiceSearchManager.startListening();
            
            // Insert voice button after search input
            searchInput.parentNode.insertBefore(voiceBtn, searchInput.nextSibling);
        }
    });
    
    // Listen for language changes
    document.addEventListener('languageChanged', function(event) {
        const language = event.detail.language;
        const voiceLang = language === 'vi-VN' ? 'vi-VN' : 'en-US';
        voiceSearchManager.setLanguage(voiceLang);
    });
});

// Global function to add voice search to any input
function addVoiceSearchToInput(inputElement) {
    if (!inputElement || inputElement.parentNode.querySelector('.voice-search-btn')) {
        return; // Already has voice search or invalid input
    }
    
    const voiceBtn = document.createElement('button');
    voiceBtn.type = 'button';
    voiceBtn.className = 'btn voice-search-btn ms-2';
    voiceBtn.innerHTML = '<i class="fas fa-microphone"></i>';
    voiceBtn.title = 'Tìm kiếm bằng giọng nói';
    voiceBtn.onclick = () => voiceSearchManager.startListening();
    
    inputElement.parentNode.insertBefore(voiceBtn, inputElement.nextSibling);
}
