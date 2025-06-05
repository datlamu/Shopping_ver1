function sendAjax({ url, method = "POST", data = {}, onSuccess = null }) {
    $.ajax({
        type: method,
        url: url,
        data: data,
        success: function (result) {
            // Kiểm tra
            if (result.success) {
                Swal.fire(result.message);
                // Nếu có function thì thực thi
                if (typeof onSuccess === "function") {
                    onSuccess(result);
                }
            }
            else
                Swal.fire(result.message);
        }
    });
}
