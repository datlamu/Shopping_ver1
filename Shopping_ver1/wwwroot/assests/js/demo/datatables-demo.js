function setupDataTable(config = {}) {
    const enableExport = config.enableExport || false;
    const exportCols = config.exportColumns || [0];
    const buttonsToShow = config.buttonsToShow || ['copy', 'csv', 'excel', 'pdf', 'print'];

    const allExportButtons = {
        copy: {
            extend: 'copy',
            text: '<i class="fas fa-copy"></i> Copy',
            className: 'btn btn-sm btn-primary',
            exportOptions: { columns: exportCols }
        },
        csv: {
            extend: 'csv',
            text: '<i class="fas fa-file-csv"></i> CSV',
            className: 'btn btn-sm btn-success',
            exportOptions: { columns: exportCols }
        },
        excel: {
            extend: 'excel',
            text: '<i class="fas fa-file-excel"></i> Excel',
            className: 'btn btn-sm btn-success',
            exportOptions: { columns: exportCols }
        },
        pdf: {
            extend: 'pdf',
            text: '<i class="fas fa-file-pdf"></i> PDF',
            className: 'btn btn-sm btn-danger',
            exportOptions: { columns: exportCols },
            customize: function (doc) {
                doc.content[1].table.widths = exportCols.map(() => '*');
                doc.pageMargins = [20, 20, 20, 20];
                let body = doc.content[1].table.body;
                for (let i = 0; i < body.length; i++) {
                    for (let j = 0; j < body[i].length; j++) {
                        body[i][j].alignment = 'left';
                    }
                }
            }
        },
        print: {
            extend: 'print',
            text: '<i class="fas fa-print"></i> Print',
            className: 'btn btn-sm btn-info',
            exportOptions: { columns: exportCols, footer: true },
            customize: function (win) {
                // Lấy phần tfoot thật trong DOM
                const originalTfoot = document.querySelector('#dataTable tfoot');
                const clonedTfoot = originalTfoot ? originalTfoot.cloneNode(true) : null;

                // Tìm bảng in trong cửa sổ mới
                const printTable = win.document.querySelector('table');

                // Nếu có tfoot và bảng, gắn tfoot lại vào bảng in
                if (printTable && clonedTfoot) {
                    printTable.appendChild(clonedTfoot);
                }

                // Đảm bảo hiển thị đúng footer khi in
                const style = win.document.createElement('style');
                style.innerHTML = 'tfoot { display: table-footer-group; }';
                win.document.head.appendChild(style);
            }
        }
    };

    const exportButtons = buttonsToShow.map(type => allExportButtons[type]);

    const options = {
        dom: enableExport
            ? "<'row mb-3'<'col-md-4'l><'col-md-4 d-flex justify-content-center flex-wrap'B><'col-md-4 text-md-end'f>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row mt-2'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>"
            : "<'row mb-3'<'col-md-6'l><'col-md-6 text-end'f>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row mt-2'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",

        buttons: enableExport ? exportButtons : [],
        language: {
            lengthMenu: "Hiển thị _MENU_ dòng",
            zeroRecords: "Không tìm thấy kết quả nào",
            info: "Hiển thị _START_ đến _END_ của _TOTAL_ dòng",
            infoEmpty: "Không có dữ liệu",
            infoFiltered: "(lọc từ _MAX_ dòng)",
            search: "Tìm kiếm:",
            paginate: {
                first: "Đầu",
                last: "Cuối",
                next: "Tiếp",
                previous: "Trước"
            }
        }
    };

    const table = $('#dataTable').DataTable(options);

    $('.dt-buttons').removeClass('btn-group');
    $('.dt-buttons .btn').removeClass('btn-secondary');

    return table;
}
