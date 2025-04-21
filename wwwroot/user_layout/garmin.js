    document.addEventListener('DOMContentLoaded', function() {
    // Lấy tất cả các thẻ li trong nav
    const navItems = document.querySelectorAll('#js__results-nav-tabs li');
    // Lấy tất cả các div nội dung
    const contentDivs = document.querySelectorAll('.sp__results__wrapper__content');

    // Hàm để ẩn tất cả các div và bỏ class active cho tất cả li
    function resetContent() {
    contentDivs.forEach(div => {
    div.hidden = true;
});
    navItems.forEach(item => {
    item.querySelector('a').classList.remove('active');
});
}

    // Đặt mặc định là hiển thị nội dung đầu tiên
    resetContent();
    document.getElementById('js__results-content-1').hidden = false;
    navItems[0].querySelector('a').classList.add('active');

    // Thêm sự kiện click cho từng nav item
    navItems.forEach(item => {
    item.addEventListener('click', function(event) {
    event.preventDefault(); // Ngăn chặn hành vi mặc định của thẻ a

    const tabId = item.querySelector('a').getAttribute('data-tab-id');

    // Ẩn tất cả các nội dung và bỏ class active
    resetContent();

    // Hiển thị nội dung tương ứng
    document.getElementById(`js__results-content-${tabId}`).hidden = false;

    // Thêm class active cho li đã chọn
    item.querySelector('a').classList.add('active');
});
});
});
