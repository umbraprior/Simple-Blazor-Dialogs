window.FocusTrap = {
    // Store the previously focused element to restore focus later
    previouslyFocusedElement: null,
    
    // Store active dialog elements for focus management
    activeDialogs: new Map(),
    
    // Initialize focus trap for a dialog
    initializeFocusTrap: function(dialogId, dialogElement) {
        // Store the currently focused element
        this.previouslyFocusedElement = document.activeElement;
        
        // Find all focusable elements within the dialog
        const focusableElements = this.getFocusableElements(dialogElement);
        
        // Store dialog info
        this.activeDialogs.set(dialogId, {
            element: dialogElement,
            focusableElements: focusableElements,
            keydownHandler: null
        });
        
        // Set initial focus to first focusable element
        if (focusableElements.length > 0) {
            focusableElements[0].focus();
        }
        
        // Add keyboard event listener
        const keydownHandler = (e) => this.handleKeydown(e, dialogId);
        document.addEventListener('keydown', keydownHandler);
        
        // Store the handler so we can remove it later
        this.activeDialogs.get(dialogId).keydownHandler = keydownHandler;
    },
    
    // Remove focus trap for a dialog
    removeFocusTrap: function(dialogId) {
        const dialogInfo = this.activeDialogs.get(dialogId);
        if (dialogInfo && dialogInfo.keydownHandler) {
            // Remove event listener
            document.removeEventListener('keydown', dialogInfo.keydownHandler);
            
            // Remove from active dialogs
            this.activeDialogs.delete(dialogId);
            
            // Restore focus to previously focused element if no other dialogs are open
            if (this.activeDialogs.size === 0 && this.previouslyFocusedElement) {
                this.previouslyFocusedElement.focus();
                this.previouslyFocusedElement = null;
            }
        }
    },
    
    // Handle keyboard events for focus trapping
    handleKeydown: function(event, dialogId) {
        const dialogInfo = this.activeDialogs.get(dialogId);
        if (!dialogInfo) return;
        
        const { focusableElements } = dialogInfo;
        
        // Handle Tab key for focus trapping
        if (event.key === 'Tab') {
            if (focusableElements.length === 0) {
                event.preventDefault();
                return;
            }
            
            const currentFocusIndex = focusableElements.indexOf(document.activeElement);
            
            if (event.shiftKey) {
                // Shift + Tab (reverse)
                if (currentFocusIndex <= 0) {
                    event.preventDefault();
                    focusableElements[focusableElements.length - 1].focus();
                }
            } else {
                // Tab (forward)
                if (currentFocusIndex >= focusableElements.length - 1) {
                    event.preventDefault();
                    focusableElements[0].focus();
                }
            }
        }
        
        // Handle Escape key to close dialog
        if (event.key === 'Escape') {
            event.preventDefault();
            // Call back to Blazor to close the dialog
            DotNet.invokeMethodAsync('Simple.Blazor.Dialogs', 'HandleEscapeKey', dialogId);
        }
    },
    
    // Get all focusable elements within a container
    getFocusableElements: function(container) {
        const focusableSelectors = [
            'button:not([disabled])',
            'input:not([disabled])',
            'select:not([disabled])',
            'textarea:not([disabled])',
            'a[href]',
            '[tabindex]:not([tabindex="-1"])',
            '[contenteditable="true"]'
        ].join(', ');
        
        const elements = container.querySelectorAll(focusableSelectors);
        
        // Filter out hidden elements
        return Array.from(elements).filter(element => {
            return element.offsetWidth > 0 && 
                   element.offsetHeight > 0 && 
                   !element.hasAttribute('hidden') &&
                   window.getComputedStyle(element).visibility !== 'hidden';
        });
    },
    
    // Cleanup all focus traps (useful for component disposal)
    cleanup: function() {
        this.activeDialogs.forEach((dialogInfo, dialogId) => {
            this.removeFocusTrap(dialogId);
        });
        this.activeDialogs.clear();
        this.previouslyFocusedElement = null;
    }
};

// Background scrolling management
window.BackgroundScrolling = {
    // Store original body styles
    originalStyles: {
        overflow: null,
        paddingRight: null,
        position: null
    },
    
    // Track how many dialogs want background scrolling disabled
    disableCount: 0,
    
    // Disable background scrolling
    disable: function() {
        if (this.disableCount === 0) {
            // Store original styles
            this.originalStyles.overflow = document.body.style.overflow;
            this.originalStyles.paddingRight = document.body.style.paddingRight;
            this.originalStyles.position = document.body.style.position;
            
            // Get scrollbar width to prevent layout shift
            const scrollbarWidth = window.innerWidth - document.documentElement.clientWidth;
            
            // Apply styles to prevent scrolling
            document.body.style.overflow = 'hidden';
            if (scrollbarWidth > 0) {
                document.body.style.paddingRight = scrollbarWidth + 'px';
            }
        }
        
        this.disableCount++;
    },
    
    // Enable background scrolling
    enable: function() {
        this.disableCount = Math.max(0, this.disableCount - 1);
        
        if (this.disableCount === 0) {
            // Restore original styles
            document.body.style.overflow = this.originalStyles.overflow || '';
            document.body.style.paddingRight = this.originalStyles.paddingRight || '';
            document.body.style.position = this.originalStyles.position || '';
        }
    },
    
    // Force enable (cleanup all)
    forceEnable: function() {
        this.disableCount = 0;
        document.body.style.overflow = this.originalStyles.overflow || '';
        document.body.style.paddingRight = this.originalStyles.paddingRight || '';
        document.body.style.position = this.originalStyles.position || '';
    }
}; 