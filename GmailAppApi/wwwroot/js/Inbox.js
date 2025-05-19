// script.js

function setActive(button, page) {
    // Xóa class active hiện tại
    document.querySelectorAll('.sidebar-link').forEach(link => {
        link.classList.remove('active');
    });

    // Thêm class active vào nút được nhấn
    button.classList.add('active');

    // Điều hướng
    window.location.href = page;
}
