function sendAjax({ url, method = "POST", data = {}, onSuccess = null }) {
    $.ajax({
        type: method,
        url: url,
        data: data,
        success: function (result) {
            Swal.fire({
                position: 'top-end',
                icon: result.success ? 'success' : 'error',
                title: result.message,
                showConfirmButton: false,
                timer: 5000
            });

            if (result.success && typeof onSuccess === "function") {
                onSuccess(result);
            }
        },
        error: function () {
            Swal.fire({
                position: 'top-end',
                icon: 'error',
                title: "Có lỗi xảy ra.",
                showConfirmButton: false,
                timer: 5000
            });
        }
    });
}
