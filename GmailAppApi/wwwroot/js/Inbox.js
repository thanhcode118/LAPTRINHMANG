// script.js
document.addEventListener('DOMContentLoaded', () => {
    const links = document.querySelectorAll('.sidebar-link');
    const path = window.location.pathname;
    const filename = path.substring(path.lastIndexOf('/') + 1);

    links.forEach(link => {
        if (link.getAttribute('href') === filename) {
            link.classList.add('active');
        }
    });
});
