// Add JWT token to all requests but with better navigation handling
(function() {
    // Store original fetch
    const originalFetch = window.fetch;

    // Override fetch to add Authorization header
    window.fetch = function(url, options = {}) {
        const token = localStorage.getItem('jwt_token');
        if (token) {
            options.headers = options.headers || {};
            options.headers['Authorization'] = `Bearer ${token}`;
        }
        return originalFetch(url, options);
    };

    // Handle AJAX requests made with jQuery
    $(document).ajaxSend(function(e, xhr, options) {
        const token = localStorage.getItem('jwt_token');
        if (token) {
            xhr.setRequestHeader('Authorization', `Bearer ${token}`);
        }
    });

    // Create a custom Authorization header for all requests
    const tokenObserver = new MutationObserver(function() {
        // Add JWT token to all forms
        document.querySelectorAll('form').forEach(form => {
            // Skip if already processed
            if (form.dataset.jwtProcessed) return;
            form.dataset.jwtProcessed = 'true';

            form.addEventListener('submit', function() {
                const token = localStorage.getItem('jwt_token');
                if (!token) return; // Continue normal submission

                // Create hidden input with token
                let tokenInput = document.createElement('input');
                tokenInput.type = 'hidden';
                tokenInput.name = 'JwtToken';
                tokenInput.value = token;
                form.appendChild(tokenInput);
            });
        });
    });

    // Start observing for form additions
    tokenObserver.observe(document.body, {
        childList: true,
        subtree: true
    });

    // Create JWT cookie for server-side usage
    function updateJwtCookie() {
        const token = localStorage.getItem('jwt_token');
        if (token) {
            // Extract expiration from JWT
            try {
                const payload = JSON.parse(atob(token.split('.')[1]));
                const expirySeconds = payload.exp - Math.floor(Date.now() / 1000);
                document.cookie = `jwt_auth=${token}; path=/; max-age=${expirySeconds}; SameSite=Lax`;
            } catch (e) {
                console.error('Error setting JWT cookie:', e);
            }
        }
    }

    function navigateToIdentityPage(url) {
        const token = localStorage.getItem('jwt_token');
        if (!token) {
            window.location.href = '/JwtLogin';
            return;
        }

        // Ensure JWT cookie is set
        try {
            const payload = JSON.parse(atob(token.split('.')[1]));
            const expirySeconds = payload.exp - Math.floor(Date.now() / 1000);
            document.cookie = `jwt_auth=${token}; path=/; max-age=${expirySeconds}; SameSite=Lax`;

            // Navigate after cookie is set
            window.location.href = url;
        } catch (e) {
            console.error('Error setting JWT cookie:', e);
            window.location.href = url; // Navigate anyway as fallback
        }
    }

// Update the updateAuthUI function to show account dropdown when logged in
    function updateAuthUI() {
        const loggedIn = isJwtLoggedIn();
        const username = getJwtUsername();

        // Elements for logged in users
        document.querySelectorAll('.jwt-auth-loggedin').forEach(el => {
            el.style.display = loggedIn ? '' : 'none';
        });

        // Elements for logged out users
        document.querySelectorAll('.jwt-auth-loggedout').forEach(el => {
            el.style.display = loggedIn ? 'none' : '';
        });

        // Update username display
        document.querySelectorAll('.jwt-username').forEach(el => {
            el.textContent = username || 'User';
        });

        // Handle Identity navigation links
        document.querySelectorAll('a[href^="/Identity/Account"]').forEach(link => {
            const originalUrl = link.getAttribute('href');
            link.addEventListener('click', function(e) {
                e.preventDefault();
                navigateToIdentityPage(originalUrl);
            });
        });
    }
    

    // Update cookie on page load and every minute
    updateJwtCookie();
    setInterval(updateJwtCookie, 60000);
})();