// JavaScript for Login Page Functionality
document.addEventListener('DOMContentLoaded', function () {
    // Password toggle functionality
    const passwordToggle = document.getElementById('togglePassword');
    const passwordInput = document.getElementById('passwordInput');
    const eyeIcon = document.getElementById('eyeIcon');

    if (passwordToggle && passwordInput && eyeIcon) {
        passwordToggle.addEventListener('click', function () {
            const isPassword = passwordInput.type === 'password';

            // Toggle input type
            passwordInput.type = isPassword ? 'text' : 'password';

            // Toggle icon
            eyeIcon.className = isPassword ? 'far fa-eye-slash' : 'far fa-eye';

            // Update aria-label for accessibility
            passwordToggle.setAttribute('aria-label',
                isPassword ? 'Hide password' : 'Show password'
            );

            // Keep focus on input
            passwordInput.focus();
        });
    }

    // Form submission with loading state
    const loginForm = document.getElementById('loginForm');
    const loginBtn = document.getElementById('loginBtn');

    if (loginForm && loginBtn) {
        loginForm.addEventListener('submit', function (e) {
            // Add loading state
            loginBtn.classList.add('btn-loading');
            loginBtn.disabled = true;

            // Note: Form will submit normally to MVC action
            // Loading state will be visible during server processing
        });
    }

    // Social login buttons
    const googleSignIn = document.getElementById('googleSignIn');
    const appleSignIn = document.getElementById('appleSignIn');

    if (googleSignIn) {
        googleSignIn.addEventListener('click', function () {
            // Add loading state
            this.classList.add('btn-loading');
            this.disabled = true;

            // Here you would implement Google OAuth
            setTimeout(() => {
                this.classList.remove('btn-loading');
                this.disabled = false;
                console.log('Google Sign-In clicked');
                // Implement actual Google OAuth logic here
            }, 1500);
        });
    }

    if (appleSignIn) {
        appleSignIn.addEventListener('click', function () {
            // Add loading state
            this.classList.add('btn-loading');
            this.disabled = true;

            // Here you would implement Apple OAuth
            setTimeout(() => {
                this.classList.remove('btn-loading');
                this.disabled = false;
                console.log('Apple Sign-In clicked');
                // Implement actual Apple OAuth logic here
            }, 1500);
        });
    }

    // Input focus animations
    const inputs = document.querySelectorAll('.form-control');
    inputs.forEach(input => {
        input.addEventListener('focus', function () {
            this.parentElement.classList.add('focused');
        });

        input.addEventListener('blur', function () {
            this.parentElement.classList.remove('focused');
        });
    });
});

// Keyboard navigation enhancements
document.addEventListener('keydown', function (e) {
    // Enter key on social buttons
    if (e.key === 'Enter' && e.target.classList.contains('social-btn')) {
        e.target.click();
    }
});