// JavaScript for Brevo Login Page

document.addEventListener('DOMContentLoaded', function () {
    // Password toggle functionality
    const passwordToggle = document.getElementById('passwordToggle');
    const passwordInput = document.getElementById('password');
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

    // Form validation and submission
    const loginForm = document.getElementById('loginForm');
    const emailInput = document.getElementById('email');
    const loginBtn = loginForm.querySelector('button[type="submit"]');

    if (loginForm) {
        loginForm.addEventListener('submit', function (e) {
            e.preventDefault();

            // Add loading state
            loginBtn.classList.add('btn-loading');
            loginBtn.disabled = true;

            // Simulate form submission (replace with actual logic)
            setTimeout(() => {
                // Remove loading state
                loginBtn.classList.remove('btn-loading');
                loginBtn.disabled = false;

                // Here you would typically handle the actual login
                console.log('Login submitted:', {
                    email: emailInput.value,
                    password: passwordInput.value
                });

                // For demo purposes, show an alert
                alert('Login form submitted! (This is just a demo)');
            }, 2000);
        });

        // Real-time validation
        emailInput.addEventListener('blur', function () {
            validateEmail(this);
        });

        passwordInput.addEventListener('blur', function () {
            validatePassword(this);
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

            setTimeout(() => {
                this.classList.remove('btn-loading');
                this.disabled = false;
                console.log('Google Sign-In clicked');
                alert('Google Sign-In clicked! (This is just a demo)');
            }, 1500);
        });
    }

    if (appleSignIn) {
        appleSignIn.addEventListener('click', function () {
            // Add loading state
            this.classList.add('btn-loading');
            this.disabled = true;

            setTimeout(() => {
                this.classList.remove('btn-loading');
                this.disabled = false;
                console.log('Apple Sign-In clicked');
                alert('Apple Sign-In clicked! (This is just a demo)');
            }, 1500);
        });
    }

    // Input focus animations
    const inputs = document.querySelectorAll('.login-input');
    inputs.forEach(input => {
        input.addEventListener('focus', function () {
            this.parentElement.classList.add('focused');
        });

        input.addEventListener('blur', function () {
            this.parentElement.classList.remove('focused');
        });
    });
});

// Validation functions
function validateEmail(input) {
    const email = input.value.trim();
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    if (!email) {
        showFieldError(input, 'Email address is required');
        return false;
    } else if (!emailRegex.test(email)) {
        showFieldError(input, 'Please enter a valid email address');
        return false;
    } else {
        clearFieldError(input);
        return true;
    }
}

function validatePassword(input) {
    const password = input.value;

    if (!password) {
        showFieldError(input, 'Password is required');
        return false;
    } else if (password.length < 6) {
        showFieldError(input, 'Password must be at least 6 characters');
        return false;
    } else {
        clearFieldError(input);
        return true;
    }
}

function showFieldError(input, message) {
    clearFieldError(input);

    input.classList.add('is-invalid');

    const errorDiv = document.createElement('div');
    errorDiv.className = 'invalid-feedback';
    errorDiv.textContent = message;

    input.parentElement.appendChild(errorDiv);
}

function clearFieldError(input) {
    input.classList.remove('is-invalid');

    const existingError = input.parentElement.querySelector('.invalid-feedback');
    if (existingError) {
        existingError.remove();
    }
}

// Keyboard navigation enhancements
document.addEventListener('keydown', function (e) {
    // Enter key on social buttons
    if (e.key === 'Enter' && e.target.classList.contains('social-btn')) {
        e.target.click();
    }

    // Escape key to clear any error states
    if (e.key === 'Escape') {
        const invalidInputs = document.querySelectorAll('.is-invalid');
        invalidInputs.forEach(input => {
            clearFieldError(input);
        });
    }
});

// Add CSS for invalid state
const style = document.createElement('style');
style.textContent = `
    .login-input.is-invalid {
        border-color: #dc3545;
        box-shadow: 0 0 0 0.2rem rgba(220, 53, 69, 0.1);
    }
    
    .invalid-feedback {
        color: #dc3545;
        font-size: 0.8rem;
        margin-top: 0.25rem;
        display: block;
    }
    
    .focused {
        transform: scale(1.01);
        transition: transform 0.2s ease;
    }
`;
document.head.appendChild(style);