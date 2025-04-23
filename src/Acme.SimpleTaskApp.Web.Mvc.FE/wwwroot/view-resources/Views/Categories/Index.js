(function ($) {
    var _categoriesService = abp.services.app.categories,
        l = abp.localization.getSource('SimpleTaskApp'), // dịch đa ngôn ngữ 
        _$createModal = $('#createModal'),

        _$createForm = _$createModal.find('form'),
        _$table = $('#CategoriesTable');

    var _$categoriesTable = _$table.DataTable({
        paging: true,
        serverSide: true,
        //pageLength: 15,
        //lengthMenu: [5, 10, 15, 20, 50],
        listAction: {
            ajaxFunction: _categoriesService.getAllCategories,
            inputFilter: function () { 
                var filter = $('#CategoriesSearchForm').serializeFormToObject(true);
                console.log('Dữ liệu gửi đi:', filter);  // Kiểm tra giá trị filter
                return filter;
            }
        },
        buttons: [
            {
                name: 'refresh',  // tự đặt tên , không lquan đến logic
                text: '<i class="fas fa-redo-alt"></i>',
                action: () => _$categoriesTable.draw(false) // reload lại dữ liệu bảng , ko reload lại dữ liệu trang
            }
        ],
        responsive: {
            details: {
                type: 'column' //là khi ở tbi nhỏ sẽ tự ẩn bớt
            }
        },
        columnDefs: [
            {
                targets: 0,  // dùng cho chế độ responsive để hiển thị nút mở rộng hàng
                className: 'control',
                defaultContent: '',
            },
            {
                targets: 1,
                data: 'nameCategory',                
                sortable: false
            },
            {
                targets: 2,
                data: 'creationTime',
                sortable: false,
                render: function (data, type, row, meta) {
                    if (!data) return '';
                    const date = new Date(data);
                    return date.toLocaleString('vi-VN'); // hoặc chỉnh định dạng theo ý bạn
                }
            },
            {
                targets: 3,
                data: 'lastModificationTime',
                sortable: false,
                render: function (data, type, row, meta) {
                    if (!data) return '';
                    const date = new Date(data);
                    return date.toLocaleString('vi-VN');
                }
            },
            {
                targets: 4,
                data: null,
                sortable: false,
                autoWidth: false,
                defaultContent: '',
                render: (data, type, row, meta) => {
                    return [
                        `<button type="button" class="btn btn-sm btn-primary mr-2 edit-category" 
                data-category-id="${row.id}" data-toggle="modal" data-target="#editModal">
                <i class="fas fa-edit mr-1"></i> ${l('Edit')}
            </button>
            <button type="button" class="btn btn-sm btn-danger delete-category" 
                data-category-id="${row.id}" data-category-name="${row.nameCategory}" data-toggle="modal" data-target="#deleteModal">
                <i class="fas fa-trash mr-1"></i> ${l('Delete')}
            </button>`
                    ].join('');
                }
            }
        ]
    });



    _$createForm.validate({
        rules: {
            NameCategory: {
                required: true,
                minlength: 1,
                maxlength: 100
            }
        },
        messages: {
            NameCategory: {
                required: "Tên danh mục không được để trống",
                minlength: "Tên ít nhất 1 ký tự",
                maxlength: "Tên tối đa 100 ký tự"
            }
        }
    });

    _$createForm.find('.save-button').on('click', (e) => {
        e.preventDefault();

        if (!_$createForm.valid()) {
            return; // không submit nếu không hợp lệ
        }

        var category = _$createForm.serializeFormToObject();

        abp.ui.setBusy(_$createModal);
        _categoriesService.create(category).done(function () {
            _$createModal.modal('hide');
            _$createForm[0].reset();
            abp.message.success(l('SuccessfullyRegistered'), l('Success'));
            _$categoriesTable.ajax.reload();

        }).always(function () {
            abp.ui.clearBusy(_$createModal);
        });
    });


    //$(document).on('click','a[data-ta]')

    $(document).on('click', '.edit-category', function (e) {
        var categoryId = $(this).attr('data-category-id');
        console.log('categoryId', categoryId);
        e.preventDefault();
        abp.ajax({
            url: abp.appPath + 'Categories/EditModal?id=' + categoryId,
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


    abp.event.on('category.edited', (data) => {
        _$categoriesTable.ajax.reload();
    });


    $(document).on('click', '.delete-category', function () {
        var categoryId = $(this).attr('data-category-id');
        var categoryName = $(this).attr('data-category-name');

        deleteCategory(categoryId, categoryName);


    });

    function deleteCategory(categoryId, categoryName) {
        abp.message.confirm(
            abp.utils.formatString(
                l('AreYouSureWantToDelete'),
                categoryName),
            null,
            (isConfirmed) => {
                if (isConfirmed) {
                    _categoriesService.delete(categoryId).done(() => {
                        abp.message.success(l('SuccessfullyDeleted'), l('Success'));
                        _$categoriesTable.ajax.reload();
                    }).fail((error) => {
                        let errorMessage = "Đã xảy ra lỗi khi xóa!(vì tồn tại sản phẩm trong danh mục)";

                        if (error.responseJSON && error.responseJSON.error && error.responseJSON.error.message) {
                            errorMessage = error.responseJSON.error.message;
                        }

                        abp.message.error(errorMessage, "Lỗi");
                    });
                }
            }
        );
    }


    

    $('.btn-search').on('click', (e) => {
        _$categoriesTable.ajax.reload();
    });

    $('.txt-search').on('keypress', (e) => {
        if (e.which == 13) {
            _$categoriesTable.ajax.reload();
            return false;
        }
    });


})(jQuery);
