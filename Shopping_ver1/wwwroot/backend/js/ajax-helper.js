function sendAjax({ url, method = "POST", data = {}, onSuccess = null }) {
    $.ajax({
        type: method,
        url: url,
        data: data,
        success: function (result) {
            // Kiểm tra
            if (result.success) {
                Swal.fire({
                    position: 'top-end',
                    icon: 'success',
                    title: result.message,
                    showConfirmButton: false,
                    timer: 1500
                });
                // Nếu có function thì thực thi
                if (typeof onSuccess === "function") {
                    onSuccess(result);
                }
            }
            else
                Swal.fire({
                    position: 'top-end',
                    icon: 'error',
                    title: result.message,
                    showConfirmButton: false,
                    timer: 1500
                });
        }
    });
}
