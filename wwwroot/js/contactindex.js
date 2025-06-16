
//// Device-based items per page
//function getItemsPerPage() {
//    const width = window.innerWidth;
//    if (width < 768) return 5;  // Mobile: 5 contacts
//    if (width < 992) return 8;  // Tablet: 8 contacts
//    return 10;                  // Desktop: 10 contacts
//}

//// Enhanced search functionality with debouncing
//let searchTimeout;
//const searchInput = document.getElementById("searchInput");

//searchInput.addEventListener("input", function () {
//    clearTimeout(searchTimeout);
//    searchTimeout = setTimeout(() => {
//        performSearch();
//    }, 300);
//});

//function performSearch() {
//    const searchValue = searchInput.value.toLowerCase().trim();
//    const rows = document.querySelectorAll(".contact-row");
//    let visibleCount = 0;

//    rows.forEach(row => {
//        const name = row.querySelector(".contact-name")?.textContent?.toLowerCase() || "";
//        const email = row.querySelector(".contact-email")?.textContent?.toLowerCase() || "";
//        const phone = row.cells[2]?.textContent?.toLowerCase() || "";
//        const address = row.cells[3]?.textContent?.toLowerCase() || "";

//        const isVisible = name.includes(searchValue) ||
//            email.includes(searchValue) ||
//            phone.includes(searchValue) ||
//            address.includes(searchValue);

//        if (isVisible) {
//            row.style.display = "";
//            row.style.animation = "fadeIn 0.3s ease-in";
//            visibleCount++;
//        } else {
//            row.style.display = "none";
//        }
//    });

//    updateResultsInfo(visibleCount, searchValue);
//    updateContactCount(visibleCount);
//}

//// Sort functionality with animations
//let sortDirection = {};

//function sortTable(column) {
//    const table = document.querySelector('.table');
//    const tbody = table.querySelector('tbody');
//    const rows = Array.from(tbody.querySelectorAll('.contact-row'));

//    // Toggle sort direction
//    sortDirection[column] = sortDirection[column] === 'asc' ? 'desc' : 'asc';

//    // Add loading animation
//    const sortIcon = document.getElementById(`sort-${column}`);
//    sortIcon.className = 'loading';

//    setTimeout(() => {
//        rows.sort((a, b) => {
//            let aValue, bValue;

//            switch (column) {
//                case 'name':
//                    aValue = a.querySelector('.contact-name')?.textContent || '';
//                    bValue = b.querySelector('.contact-name')?.textContent || '';
//                    break;
//                case 'email':
//                    aValue = a.querySelector('.contact-email')?.textContent || '';
//                    bValue = b.querySelector('.contact-email')?.textContent || '';
//                    break;
//                case 'phone':
//                    aValue = a.cells[2]?.textContent || '';
//                    bValue = b.cells[2]?.textContent || '';
//                    break;
//                case 'address':
//                    aValue = a.cells[3]?.textContent || '';
//                    bValue = b.cells[3]?.textContent || '';
//                    break;
//                default:
//                    return 0;
//            }

//            aValue = aValue.toLowerCase();
//            bValue = bValue.toLowerCase();

//            if (sortDirection[column] === 'asc') {
//                return aValue.localeCompare(bValue);
//            } else {
//                return bValue.localeCompare(aValue);
//            }
//        });

//        // Re-append sorted rows with animation
//        rows.forEach((row, index) => {
//            setTimeout(() => {
//                tbody.appendChild(row);
//                row.style.animation = "slideInUp 0.3s ease-out";
//            }, index * 50);
//        });

//        // Update sort icons
//        updateSortIcons(column);
//    }, 500);
//}

//function updateSortIcons(activeColumn) {
//    const columns = ['name', 'email', 'phone', 'address'];

//    columns.forEach(column => {
//        const icon = document.getElementById(`sort-${column}`);
//        if (icon) {
//            if (column === activeColumn) {
//                icon.className = sortDirection[column] === 'asc' ? 'bi bi-sort-up sort-icon active' : 'bi bi-sort-down sort-icon active';
//            } else {
//                icon.className = 'bi bi-arrow-up-down sort-icon';
//            }
//        }
//    });
//}

//function updateResultsInfo(visibleCount, searchTerm) {
//    const resultsInfo = document.querySelector('#resultsText');
//    if (resultsInfo && searchTerm) {
//        resultsInfo.innerHTML = `Showing ${visibleCount} contacts matching "${searchTerm}"`;
//    }
//}

//function updateContactCount(count) {
//    const contactCount = document.getElementById('contactCount');
//    if (contactCount) {
//        contactCount.textContent = `${count} contacts`;
//    }
//}

//// Enhanced delete confirmation
//function confirmDelete(contactName) {
//    return confirm(`⚠️ Delete Contact\n\nAre you sure you want to permanently delete "${contactName}"?\n\nThis action cannot be undone.`);
//}

//// Responsive handling
//function handleResize() {
//    const itemsPerPage = getItemsPerPage();
//    console.log(`Current device shows ${itemsPerPage} items per page`);

//    // Update pagination if needed
//    // This would typically be handled server-side in a real application
//}

//// Add CSS animations
//const style = document.createElement('style');
//style.textContent = `
//        @keyframes fadeIn {
//                from { opacity: 0; }
//                to { opacity: 1; }
//            }

//        @keyframes slideInUp {
//                from {
//                    transform: translateY(20px);
//                    opacity: 0;
//                }
//                to {
//                    transform: translateY(0);
//                    opacity: 1;
//                }
//            }

//            .contact-row {
//                animation: fadeIn 0.5s ease-in;
//            }
//        `;
//document.head.appendChild(style);

//// Initialize
//document.addEventListener('DOMContentLoaded', function () {
//    handleResize();
//    window.addEventListener('resize', handleResize);
//    console.log('Responsive contacts table initialized successfully');
//});





// Device-based items per page
function getItemsPerPage() {
    const width = window.innerWidth;
    if (width < 768) return 5;  // Mobile: 5 contacts
    if (width < 992) return 8;  // Tablet: 8 contacts
    return 10;                  // Desktop: 10 contacts
}

// Enhanced search functionality with debouncing
let searchTimeout;
const searchInput = document.getElementById("searchInput");

searchInput.addEventListener("input", function () {
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(() => {
        performSearch();
    }, 300);
});

function performSearch() {
    const searchValue = searchInput.value.toLowerCase().trim();
    const rows = document.querySelectorAll(".contact-row");
    let visibleCount = 0;

    rows.forEach(row => {
        const name = row.querySelector(".contact-name")?.textContent?.toLowerCase() || "";
        const email = row.querySelector(".contact-email")?.textContent?.toLowerCase() || "";
        const phone = row.cells[2]?.textContent?.toLowerCase() || "";
        const address = row.cells[3]?.textContent?.toLowerCase() || "";

        const isVisible = name.includes(searchValue) ||
            email.includes(searchValue) ||
            phone.includes(searchValue) ||
            address.includes(searchValue);

        if (isVisible) {
            row.style.display = "";
            row.style.animation = "fadeIn 0.3s ease-in";
            visibleCount++;
        } else {
            row.style.display = "none";
        }
    });

    updateResultsInfo(visibleCount, searchValue);
}

// Sort functionality with animations
let sortDirection = {};

function sortTable(column) {
    const table = document.querySelector('.table');
    const tbody = table.querySelector('tbody');
    const rows = Array.from(tbody.querySelectorAll('.contact-row'));

    // Toggle sort direction
    sortDirection[column] = sortDirection[column] === 'asc' ? 'desc' : 'asc';

    // Add loading animation
    const sortIcon = document.getElementById(`sort-${column}`);
    sortIcon.className = 'loading';

    setTimeout(() => {
        rows.sort((a, b) => {
            let aValue, bValue;

            switch (column) {
                case 'name':
                    aValue = a.querySelector('.contact-name')?.textContent || '';
                    bValue = b.querySelector('.contact-name')?.textContent || '';
                    break;
                case 'email':
                    aValue = a.querySelector('.contact-email')?.textContent || '';
                    bValue = b.querySelector('.contact-email')?.textContent || '';
                    break;
                case 'phone':
                    aValue = a.cells[2]?.textContent || '';
                    bValue = b.cells[2]?.textContent || '';
                    break;
                case 'address':
                    aValue = a.cells[3]?.textContent || '';
                    bValue = b.cells[3]?.textContent || '';
                    break;
                default:
                    return 0;
            }

            aValue = aValue.toLowerCase();
            bValue = bValue.toLowerCase();

            if (sortDirection[column] === 'asc') {
                return aValue.localeCompare(bValue);
            } else {
                return bValue.localeCompare(aValue);
            }
        });

        // Re-append sorted rows with animation
        rows.forEach((row, index) => {
            setTimeout(() => {
                tbody.appendChild(row);
                row.style.animation = "slideInUp 0.3s ease-out";
            }, index * 50);
        });

        // Update sort icons
        updateSortIcons(column);
    }, 500);
}

function updateSortIcons(activeColumn) {
    const columns = ['name', 'email', 'phone', 'address'];

    columns.forEach(column => {
        const icon = document.getElementById(`sort-${column}`);
        if (icon) {
            if (column === activeColumn) {
                icon.className = sortDirection[column] === 'asc' ? 'bi bi-sort-up sort-icon active' : 'bi bi-sort-down sort-icon active';
            } else {
                icon.className = 'bi bi-arrow-up-down sort-icon';
            }
        }
    });
}

function updateResultsInfo(visibleCount, searchTerm) {
    const resultsInfo = document.querySelector('#resultsText');
    if (resultsInfo && searchTerm) {
        resultsInfo.innerHTML = `Showing ${visibleCount} contacts matching "${searchTerm}"`;
    }
}

// Enhanced delete confirmation
function confirmDelete(contactName) {
    return confirm(`⚠️ Delete Contact\n\nAre you sure you want to permanently delete "${contactName}"?\n\nThis action cannot be undone.`);
}

// Responsive handling
function handleResize() {
    const itemsPerPage = getItemsPerPage();
    console.log(`Current device shows ${itemsPerPage} items per page`);

    // Update pagination if needed
    // This would typically be handled server-side in a real application
}

// Add CSS animations
const style = document.createElement('style');
style.textContent = `
        @keyframes fadeIn {
                from { opacity: 0; }
                to { opacity: 1; }
            }

        @keyframes slideInUp {
                from {
                    transform: translateY(20px);
                    opacity: 0;
                }
                to {
                    transform: translateY(0);
                    opacity: 1;
                }
            }

            .contact-row {
                animation: fadeIn 0.5s ease-in;
            }
        `;
document.head.appendChild(style);

// Initialize
document.addEventListener('DOMContentLoaded', function () {
    handleResize();
    window.addEventListener('resize', handleResize);
    console.log('Responsive contacts table initialized successfully');
});