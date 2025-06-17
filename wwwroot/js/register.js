// JavaScript for Registration Page Functionality
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

    // Password strength checker
    const strengthIndicator = document.getElementById('passwordStrength');
    const strengthText = document.getElementById('strengthText');
    const strengthFill = document.getElementById('strengthFill');

    if (passwordInput && strengthIndicator) {
        passwordInput.addEventListener('input', function () {
            const password = this.value;
            const strength = checkPasswordStrength(password);

            if (password.length > 0) {
                strengthIndicator.style.display = 'block';
                updateStrengthIndicator(strength);
            } else {
                strengthIndicator.style.display = 'none';
            }
        });
    }

    function checkPasswordStrength(password) {
        let score = 0;
        let feedback = [];

        // Length check
        if (password.length >= 8) score += 1;
        else feedback.push('at least 8 characters');

        // Uppercase check
        if (/[A-Z]/.test(password)) score += 1;
        else feedback.push('uppercase letter');

        // Lowercase check
        if (/[a-z]/.test(password)) score += 1;
        else feedback.push('lowercase letter');

        // Number check
        if (/\d/.test(password)) score += 1;
        else feedback.push('number');

        // Special character check
        if (/[!@#$%^&*(),.?":{}|<>]/.test(password)) score += 1;
        else feedback.push('special character');

        return { score, feedback };
    }

    function updateStrengthIndicator(strength) {
        const { score, feedback } = strength;
        let text, className, width;

        if (score < 2) {
            text = 'Weak password';
            className = 'strength-weak';
            width = '25%';
        } else if (score < 4) {
            text = 'Medium password';
            className = 'strength-medium';
            width = '50%';
        } else if (score < 5) {
            text = 'Strong password';
            className = 'strength-strong';
            width = '75%';
        } else {
            text = 'Very strong password';
            className = 'strength-strong';
            width = '100%';
        }

        strengthText.textContent = text;
        strengthText.className = className;
        strengthFill.style.width = width;
        strengthFill.className = `strength-fill ${className}`;
    }

    // Form submission with loading state
    const registerForm = document.getElementById('registerForm');
    const registerBtn = document.getElementById('registerBtn');

    if (registerForm && registerBtn) {
        registerForm.addEventListener('submit', function (e) {
            // Add loading state
            registerBtn.classList.add('btn-loading');
            registerBtn.disabled = true;

            // Note: Form will submit normally to MVC action
            // Loading state will be visible during server processing
        });
    }

    // Social registration buttons
    const googleSignUp = document.getElementById('googleSignUp');
    const microsoftSignUp = document.getElementById('microsoftSignUp');

    if (googleSignUp) {
        googleSignUp.addEventListener('click', function () {
            // Add loading state
            this.classList.add('btn-loading');
            this.disabled = true;

            // Here you would implement Google OAuth
            setTimeout(() => {
                this.classList.remove('btn-loading');
                this.disabled = false;
                console.log('Google Sign-Up clicked');
                // Implement actual Google OAuth logic here
            }, 1500);
        });
    }

    if (microsoftSignUp) {
        microsoftSignUp.addEventListener('click', function () {
            // Add loading state
            this.classList.add('btn-loading');
            this.disabled = true;

            // Here you would implement Microsoft OAuth
            setTimeout(() => {
                this.classList.remove('btn-loading');
                this.disabled = false;
                console.log('Microsoft Sign-Up clicked');
                // Implement actual Microsoft OAuth logic here
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

// Display server messages
@if (!string.IsNullOrEmpty(ViewBag.Message)) {
    <text>
        document.addEventListener('DOMContentLoaded', function() {
                // Create a more elegant notification instead of alert
                const message = '@Html.Raw(ViewBag.Message)';
        const alertDiv = document.createElement('div');
        alertDiv.className = 'alert alert-info alert-dismissible fade show position-fixed';
        alertDiv.style.cssText = 'top: 20px; right: 20px; z-index: 9999; max-width: 400px;';
        alertDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        `;
        document.body.appendChild(alertDiv);

                // Auto-dismiss after 5 seconds
                setTimeout(() => {
                    if (alertDiv.parentNode) {
            alertDiv.remove();
                    }
                }, 5000);
            });
    </text>
}