//document.addEventListener('DOMContentLoaded', function () {
//    var sidebar = document.getElementById('sidebar');
//    var openBtn = document.getElementById('sidebarOpenBtn'); // You need to add this button
//    var closeBtn = document.querySelector('[data-bs-dismiss="offcanvas"]'); // Target the actual close button
//    var mainContent = document.querySelector('.main-content');
    
//    function openSidebar() {
//        sidebar.classList.add('show'); // Use Bootstrap's 'show' class
//        document.body.style.overflow = 'hidden'; // Prevent body scroll
//    }
    
//    function closeSidebar() {
//        sidebar.classList.remove('show');
//        document.body.style.overflow = '';
//    }
    
//    if (openBtn) openBtn.addEventListener('click', openSidebar);
//    if (closeBtn) closeBtn.addEventListener('click', closeSidebar);
    
//    // Close sidebar when clicking outside
//    document.addEventListener('click', function (e) {
//        if (window.innerWidth < 992 && sidebar.classList.contains('show')) {
//            if (!sidebar.contains(e.target) && (!openBtn || e.target !== openBtn)) {
//                closeSidebar();
//            }
//        }
//    });
//});



