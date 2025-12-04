// Chart.js JSInterop Wrapper for Blazor
// Phase 3.10: Added performance optimizations and accessibility

window.chartHelpers = {
    charts: {}, // Store chart instances
    resizeDebounceTimers: {}, // Debounce timers for resize

    // === UTILITY FUNCTIONS ===

    /**
     * Debounce function to limit execution rate
     */
    debounce: function (func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    },

    /**
     * Check if element is in viewport (for lazy loading)
     */
    isInViewport: function (element) {
        if (!element) return false;
        const rect = element.getBoundingClientRect();
        return (
            rect.top >= -rect.height &&
            rect.left >= -rect.width &&
            rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) + rect.height &&
            rect.right <= (window.innerWidth || document.documentElement.clientWidth) + rect.width
        );
    },

    // === CHART CREATION ===

    createLineChart: function (canvasId, config) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) {
            console.error(`Canvas element with id ${canvasId} not found`);
            return null;
        }

        // Destroy existing chart if it exists
        if (this.charts[canvasId]) {
            this.charts[canvasId].destroy();
        }

        // Performance: Skip if not in viewport (lazy load)
        if (!this.isInViewport(canvas)) {
            // Set up intersection observer for lazy loading
            this.setupLazyLoad(canvas, canvasId, config, 'line');
            return canvasId;
        }

        // Create new chart
        const ctx = canvas.getContext('2d');
        this.charts[canvasId] = new Chart(ctx, {
            type: 'line',
            data: config.data,
            options: {
                responsive: true,
                maintainAspectRatio: false,
                // Performance optimizations
                animation: {
                    duration: 300 // Reduced animation time
                },
                elements: {
                    point: {
                        radius: config.data.datasets[0].data.length > 30 ? 0 : 3 // Hide points for large datasets
                    }
                },
                plugins: {
                    legend: {
                        display: config.showLegend ?? true,
                    },
                    // Accessibility: Describe chart for screen readers
                    title: {
                        display: false
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true
                    }
                },
                // Performance: Disable hover animations for large datasets
                interaction: {
                    mode: config.data.datasets[0].data.length > 100 ? 'nearest' : 'index',
                    intersect: false
                }
            }
        });

        // Add ARIA label for accessibility
        canvas.setAttribute('role', 'img');
        canvas.setAttribute('aria-label', config.ariaLabel || 'Line chart visualization');

        return canvasId;
    },

    createBarChart: function (canvasId, config) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) {
            console.error(`Canvas element with id ${canvasId} not found`);
            return null;
        }

        // Destroy existing chart if it exists
        if (this.charts[canvasId]) {
            this.charts[canvasId].destroy();
        }

        // Create new bar chart with optimizations
        const ctx = canvas.getContext('2d');
        
        // Merge in performance options
        const enhancedConfig = {
            ...config,
            options: {
                ...config.options,
                responsive: true,
                maintainAspectRatio: false,
                animation: {
                    duration: 300
                }
            }
        };

        this.charts[canvasId] = new Chart(ctx, enhancedConfig);

        // Accessibility
        canvas.setAttribute('role', 'img');
        canvas.setAttribute('aria-label', config.ariaLabel || 'Bar chart visualization');

        return canvasId;
    },

    // === LAZY LOADING ===

    setupLazyLoad: function (canvas, canvasId, config, chartType) {
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    observer.disconnect();
                    if (chartType === 'line') {
                        this.createLineChart(canvasId, config);
                    } else if (chartType === 'bar') {
                        this.createBarChart(canvasId, config);
                    }
                }
            });
        }, {
            rootMargin: '100px' // Load 100px before entering viewport
        });

        observer.observe(canvas);
    },

    // === CHART UPDATES ===

    updateChart: function (canvasId, newData) {
        const chart = this.charts[canvasId];
        if (chart) {
            // Debounced update for performance
            if (this.resizeDebounceTimers[canvasId]) {
                clearTimeout(this.resizeDebounceTimers[canvasId]);
            }
            
            this.resizeDebounceTimers[canvasId] = setTimeout(() => {
                chart.data = newData;
                chart.update('none'); // Skip animation for updates
            }, 100);
        }
    },

    destroyChart: function (canvasId) {
        const chart = this.charts[canvasId];
        if (chart) {
            chart.destroy();
            delete this.charts[canvasId];
        }
        
        // Clear any pending debounce timers
        if (this.resizeDebounceTimers[canvasId]) {
            clearTimeout(this.resizeDebounceTimers[canvasId]);
            delete this.resizeDebounceTimers[canvasId];
        }
    },

    // === RESIZE HANDLING ===

    /**
     * Handle window resize with debouncing
     */
    handleResize: function () {
        Object.keys(this.charts).forEach(canvasId => {
            const chart = this.charts[canvasId];
            if (chart) {
                chart.resize();
            }
        });
    },

    // === ACCESSIBILITY HELPERS ===

    /**
     * Generate text description of chart data for screen readers
     */
    getChartDescription: function (canvasId) {
        const chart = this.charts[canvasId];
        if (!chart) return 'Chart not available';

        const data = chart.data;
        const labels = data.labels || [];
        const datasets = data.datasets || [];

        let description = `Chart with ${datasets.length} dataset(s). `;
        
        datasets.forEach((dataset, i) => {
            const values = dataset.data || [];
            const min = Math.min(...values);
            const max = Math.max(...values);
            const avg = values.reduce((a, b) => a + b, 0) / values.length;
            
            description += `Dataset ${i + 1}: ${dataset.label || 'Unnamed'}. `;
            description += `Range: ${min.toFixed(1)} to ${max.toFixed(1)}. `;
            description += `Average: ${avg.toFixed(1)}. `;
        });

        return description;
    },

    // === CLEANUP ===

    destroyAllCharts: function () {
        Object.keys(this.charts).forEach(canvasId => {
            this.destroyChart(canvasId);
        });
    }
};

// Set up debounced resize handler
window.addEventListener('resize', window.chartHelpers.debounce(() => {
    window.chartHelpers.handleResize();
}, 250));

// Cleanup on page unload
window.addEventListener('beforeunload', () => {
    window.chartHelpers.destroyAllCharts();
});
