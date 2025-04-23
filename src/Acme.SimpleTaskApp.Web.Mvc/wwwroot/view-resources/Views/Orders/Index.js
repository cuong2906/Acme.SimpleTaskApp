(function ($) {
    var _orderService = abp.services.app.order,
        l = abp.localization.getSource('SimpleTaskApp'),
        _$modal = $('#editModal'),
        _$form = _$modal.find('form'),
        _$table = $('#OrdersTable');

    var _permissions = {
        edit: abp.auth.isGranted('Pages.Orders.Edit'),
        delete: abp.auth.isGranted('Pages.Orders.Delete')
    };

    var _$ordersTable = _$table.DataTable({
        paging: true,
        serverSide: true,
        ordering: true,
        processing: true,
        order: [[5, 'desc']],
        dom: 'Bfrtip',
        //buttons: [
        //    'copy', 'csv', 'excel', 'pdf', 'print'
        //],
        listAction: {
            ajaxFunction: _orderService.getAllOrders,
            inputFilter: function () {
                return $('#OrdersSearchForm').serializeFormToObject(true);
            }
        },
        columnDefs: [
            {
                targets: 0,
                className: 'control',
                defaultContent: '',
                orderable: false
            },
            {
                targets: 1,
                data: 'userName',
                orderable: true
            },
            {
                targets: 2,
                data: 'userEmail',
                orderable: true
            },
            {
                targets: 3,
                data: 'totalAmount',
                orderable: true,
                render: function (data, type, row, meta) {
                    if (!data) return '0';
                    return Number(data).toLocaleString("vi-VN") + ' VND';
                }
            },
            {
                targets: 4,
                data: 'statusText',
                orderable: true,
                render: function (data, type, row, meta) {
                    var statusClass = '';
                    switch (row.status) {
                        case 0: // Pending
                            statusClass = 'badge-warning';
                            break;
                        case 1: // Processing
                            statusClass = 'badge-info';
                            break;
                        case 2: // Shipped
                            statusClass = 'badge-primary';
                            break;
                        case 3: // Delivered
                            statusClass = 'badge-success';
                            break;
                        case 4: // Cancelled
                            statusClass = 'badge-danger';
                            break;
                    }
                    return `<span class="badge ${statusClass}">${data}</span>`;
                }
            },
            {
                targets: 5,
                data: 'creationTime',
                orderable: true,
                render: function (data, type, row, meta) {
                    if (!data) return '';
                    const date = new Date(data);
                    return date.toLocaleString('vi-VN');
                }
            },
            {
                targets: 6,
                data: 'lastModificationTime',
                orderable: true,
                render: function (data, type, row, meta) {
                    if (!data) return '';
                    const date = new Date(data);
                    return date.toLocaleString('vi-VN');
                }
            },
            {
                targets: 7,
                data: null,
                orderable: false,
                autoWidth: false,
                defaultContent: '',
                visible: _permissions.edit || _permissions.delete,
                render: (data, type, row, meta) => {
                    if (!_permissions.edit && !_permissions.delete) {
                        return '';
                    }
                    
                    let buttons = [];
                    
                    if (_permissions.edit) {
                        buttons.push(`
                            <button type="button" class="dropdown-item text-secondary edit-order" data-order-id="${row.id}" data-toggle="modal" data-target="#editModal">
                                <i class="fas fa-edit mr-2"></i>  ${l('Edit')}
                            </button>
                        `);
                    }
                    
                    if (_permissions.delete) {
                        if (buttons.length > 0) {
                            buttons.push('<div class="dropdown-divider m-0"></div>');
                        }
                        buttons.push(`
                            <button type="button" class="dropdown-item text-danger delete-order" data-order-id="${row.id}" data-order-name="${row.userName}" data-toggle="modal" data-target="#deleteModal">
                                <i class="fas fa-trash mr-2"></i>  ${l('Delete')}
                            </button>
                        `);
                    }
                    
                    return buttons.join('');
                }
            }
        ]
    });

    $(document).on('click', '.edit-order', function (e) {
        e.preventDefault();
        if (!_permissions.edit) {
            abp.message.warn("Bạn không đủ quyền để chỉnh sửa đơn hàng!");
            return;
        }
        var orderId = $(this).attr('data-order-id');

        abp.ajax({
            url: abp.appPath + 'Orders/EditModal?orderId=' + orderId,
            type: 'POST',
            dataType: 'html',
            success: function (content) {
                $('#editModal div.modal-content').html(content);
            },
            error: function (e) {
                abp.notify.error(l('ErrorLoadingData'));
            }
        });
    });

    abp.event.on('order.edited', (data) => {
        _$ordersTable.ajax.reload();
    });

    $(document).on('click', '.delete-order', function () {
        if (!_permissions.delete) {
            abp.message.warn("Bạn không đủ quyền để xoá đơn hàng!");
            return;
        }

        var orderId = $(this).attr('data-order-id');
        var orderName = $(this).attr('data-order-name');

        abp.message.confirm(
            l('DeleteOrderWarningMessage', orderName),
            null,
            (isConfirmed) => {
                if (isConfirmed) {
                    _orderService.deleteOrder(orderId).done(() => {
                        abp.notify.info(l('SuccessfullyDeleted'));
                        _$ordersTable.ajax.reload();
                    });
                }
            }
        );
    });

    $('.btn-search').on('click', (e) => {
        _$ordersTable.ajax.reload();
    });

    $('#OrdersSearchForm input[type=text]').on('keypress', (e) => {
        if (e.which === 13) {
            e.preventDefault();
            _$ordersTable.ajax.reload();
        }
    });

    $('#OrdersSearchForm select').on('change', () => {
        _$ordersTable.ajax.reload();
    });
})(jQuery); 