// Cảnh báo khi xóa và tự động ẩn thông báo
$(function () {
    $(".confirmDeletion").on("click", function () {
        return confirm("Confirm deletion");
    });

    setTimeout(() => {
        $("div.alert.notification").fadeOut("slow");
    }, 2000);
});

// Xem trước ảnh ở create product
function previewImage(input, previewId = 'preview-img') {
    const preview = document.getElementById(previewId);
    if (input.files && input.files[0]) {
        const reader = new FileReader();
        reader.onload = function (e) {
            preview.src = e.target.result;
        };
        reader.readAsDataURL(input.files[0]);
    }
}
