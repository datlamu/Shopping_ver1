window.formatCurrencyVND = function (value) {
    const number = Number(value);
    if (isNaN(number)) return value;
    return number.toLocaleString('vi-VN', {
        style: 'currency',
        currency: 'VND',
        minimumFractionDigits: 0
    });
};