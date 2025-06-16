// Enhanced CKEditor initialization
if (document.getElementById('body')) {
    CKEDITOR.replace('body', {
        versionCheck: false,
        height: 300,
        toolbar: [
            { name: 'document', items: ['Source', '-', 'Save', 'NewPage', 'Preview', 'Print', '-', 'Templates'] },
            { name: 'clipboard', items: ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo'] },
            { name: 'editing', items: ['Find', 'Replace', '-', 'SelectAll', '-', 'Scayt'] },
            '/',
            { name: 'basicstyles', items: ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-', 'CopyFormatting', 'RemoveFormat'] },
            { name: 'paragraph', items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'Blockquote', 'CreateDiv', '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-', 'BidiLtr', 'BidiRtl'] },
            { name: 'links', items: ['Link', 'Unlink', 'Anchor'] },
            { name: 'insert', items: ['Image', 'Flash', 'Table', 'HorizontalRule', 'Smiley', 'SpecialChar', 'PageBreak', 'Iframe'] },
            '/',
            { name: 'styles', items: ['Styles', 'Format', 'Font', 'FontSize'] },
            { name: 'colors', items: ['TextColor', 'BGColor'] },
            { name: 'tools', items: ['Maximize', 'ShowBlocks'] }
        ]
    });
}

// Enhanced mobile navigation - Fixed version
document.addEventListener('DOMContentLoaded', function () {
    // Handle mobile sidebar navigation
    const mobileNavLinks = document.querySelectorAll('.mobile-nav-link');
    const sidebarElement = document.getElementById('sidebar');

    mobileNavLinks.forEach(link => {
        link.addEventListener('click', function (e) {
            // Allow the link to navigate normally
            // Close the offcanvas after a short delay to allow navigation
            setTimeout(function () {
                const offcanvasInstance = bootstrap.Offcanvas.getInstance(sidebarElement);
                if (offcanvasInstance) {
                    offcanvasInstance.hide();
                }
            }, 100);
        });
    });

    // Enhanced dropdown behavior
    const userDropdown = document.getElementById('userDropdown');
    if (userDropdown) {
        userDropdown.addEventListener('show.bs.dropdown', function () {
            this.classList.add('show');
        });
        userDropdown.addEventListener('hide.bs.dropdown', function () {
            this.classList.remove('show');
        });
    }

    // Smooth scrolling for anchor links (only for hash links, not navigation links)
    document.querySelectorAll('a[href^="#"]:not(.mobile-nav-link)').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });
});