(function ($) {
    var _productService = abp.services.app.product,
        l = abp.localization.getSource('SimpleTaskApp'),
        source = abp.localization.defaultSourceName;
        _$modal = $('#createModal'),

        _$form = _$modal.find('form'),
        _$table = $('#ProductsTable');


    var _permissions = {
        create: abp.auth.isGranted('Pages.Products.Create'),
        edit: abp.auth.isGranted('Pages.Products.Edit'),
        delete: abp.auth.isGranted('Pages.Products.Delete')
    };


    var _$productsTable = _$table.DataTable({
        paging: true,
        serverSide: true,
        ordering: true,
        processing: true,
        order: [[5, 'desc']],
        dom: 'Bfrtip',
        buttons: [
            'copy', 'csv', 'excel', 'pdf', 'print'
        ],
        listAction: {
            ajaxFunction: _productService.getProductPaged,
            inputFilter: function () {
                var filter = $('#ProductsSearchForm').serializeFormToObject(true);
                var dataTable = _$table.DataTable();
                var order = dataTable.order();

                // Lấy giá trị từ input trong dropdown
                var minPrice = parseFloat($('.dropdown-menu input[name="MinPrice"]').val());
                var maxPrice = parseFloat($('.dropdown-menu input[name="MaxPrice"]').val());
                var categoryId = $('.dropdown-menu select[name="CategoryId"]').val();

                // Thêm giá trị vào filter nếu có
                if (!isNaN(minPrice)) {
                    filter.MinPrice = minPrice;
                }
                if (!isNaN(maxPrice)) {
                    filter.MaxPrice = maxPrice;
                }
                if (categoryId) {
                    filter.CategoryId = categoryId;
                }

                if (order.length > 0) {
                    var columnIndex = order[0][0];
                    var direction = order[0][1];
                    var sortField = dataTable.column(columnIndex).dataSrc();
                    filter.sorting = sortField + ' ' + direction;
                }

                return filter;
            }
        },


        buttons: [
            {
                name: 'refresh',
                text: '<i class="fas fa-redo-alt"></i>',
                action: () => _$productsTable.draw(false)
            },
        ],
        responsive: {
            details: {
                type: 'column'
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
                data: 'name',
                orderable: true,

            },
            {
                targets: 2,
                data: 'price',
                orderable: true,
                render: function (data, type, row, meta) {
                    if (!data) return '0';
                    return Number(data).toLocaleString("vi-VN") + ' VND';
                }
            },
            {
                targets: 3,
                data: 'imageUrl',
                orderable: false,
                render: function (data, type, row, meta) {
                    if (!data) return '';
                    return `<img src="${data}" alt="image" style="width: 60px; height: 60px; border-radius: 8px; object-fit: cover;" />`;
                }
            },
            {
                targets: 4,
                data: 'nameCategory',
                orderable: false,
            },
            {
                targets: 5,
                data: 'quantity',
                orderable: true,
                render: function (data, type, row, meta) {
                    return data !== null && data !== undefined ? Number(data).toLocaleString("vi-VN") : '0';
                }
            },
            {
                targets: 6,
                data: 'creationTime',
                orderable: true,
                render: function (data, type, row, meta) {
                    if (!data) return '';
                    const date = new Date(data);
                    return date.toLocaleString('vi-VN');
                }
            },
            {
                targets: 7,
                data: 'lastModificationTime',
                orderable: true,
                render: function (data, type, row, meta) {
                    if (!data) return '';
                    const date = new Date(data);
                    return date.toLocaleString('vi-VN');
                }
            },
            {
                targets: 8,
                data: null,
                orderable: false,
                autoWidth: false,
                defaultContent: '',
                visible: _permissions.addToCart,
                render: function (data, type, row, meta) {
                    return `
                        <button type="button" class="btn btn-sm btn-primary add-to-cart" data-product-id="${row.id}" data-product-name="${row.name}">
                            <i class="fas fa-cart-plus mr-1"></i> ${l('AddToCart')}
                        </button>
                    `;
                }
            },
            {
                targets: 9,
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
                            <button type="button" class="dropdown-item text-secondary edit-product" data-product-id="${row.id}" data-toggle="modal" data-target="#editModal">
                                <i class="fas fa-edit mr-2"></i>  ${l('Edit')}
                            </button>
                        `);
                    }
                    
                    if (_permissions.delete) {
                        if (buttons.length > 0) {
                            buttons.push('<div class="dropdown-divider m-0"></div>');
                        }
                        buttons.push(`
                            <button type="button" class="dropdown-item text-danger delete-product" data-product-id="${row.id}" data-product-name="${row.name}" data-toggle="modal" data-target="#deleteModal">
                                <i class="fas fa-trash mr-2"></i>  ${l('Delete')}
                            </button>
                        `);
                    }
                    
                    return buttons.join('');
                }
            }
        ]
    });




    _$form.validate({
        rules: {
            Name: {
                required: true,
                minlength: 3,
                maxlength: 100
            },
            Price: {
                required: true,
                number: true,
                min: 0,
                max: 20000000
            },
            Quantity: {
                required: true,
                number: true,
                min: 0,
                max: 10000
            },
            ImageUrl: {
                required: true,
                imageExtension: true,
                filesize: 50 * 1024 * 1024 // 5MB
            },
            CategoryId: {
                required: true
            }
        },
        messages: {
            Name: {
                required: "Tên sản phẩm không được để trống",
                minlength: l("PleaseEnterAtLeastNCharacter"),
                maxlength: l("PleaseEnterNoMoreThanNCharacter")
            },
            Price: {
                required: "Vui lòng nhập giá",
                number: "Giá phải là số",
                min: "Giá phải lớn hơn hoặc bằng 0",
                max: "Số tiền quá lớn",
            },
            Quantity: {
                required: "Vui lòng nhập số lượng",
                number: "Số lượng phải là số",
                min: "Số lượng phải lớn hơn hoặc bằng 0",
                max: "Số lượng không được vượt quá 10,000"
            },
            ImageUrl: {
                required: "Vui lòng chọn ảnh",
                imageExtension: "Chỉ chấp nhận file ảnh JPG, PNG, GIF, BMP",
                filesize: "Dung lượng ảnh tối đa là 5MB"
            },
            CategoryId: {
                required: "Vui lòng chọn danh mục"
            }
        }
    });

    // Thêm phương thức kiểm tra size ảnh
    $.validator.addMethod('filesize', function (value, element, param) {
        return this.optional(element) || (element.files[0].size <= param);
    }, 'Dung lượng ảnh vượt quá giới hạn');

    $.validator.addMethod("imageExtension", function (value, element) {
        if (element.files.length === 0) return false;
        var fileName = element.files[0].name;
        return /\.(jpe?g|png|gif|bmp|webp)$/i.test(fileName);
    }, "Chỉ chấp nhận ảnh định dạng JPG, PNG, GIF, BMP");


    _$form.find('.save-button').on('click', (e) => {
        e.preventDefault();

        if (!_$form.valid()) {
            return;
        }

        var formElements = _$form[0].elements;
        var formData = new FormData(_$form[0]);
        
        abp.ui.setBusy(_$modal);

        $.ajax({
            url: abp.appPath + 'Products/Create',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function () {
                _$modal.modal('hide');
                _$form[0].reset();
                abp.notify.success(l('SavedSuccessfully'),L('Success'));
                _$productsTable.ajax.reload();
            },
            error: function (err) {
                abp.notify.error(l('ErrorSavingData'));
                console.error(err);
            },
            complete: function () {
                abp.ui.clearBusy(_$modal);
            }
        });
    });


    function ImagePreview(modalSelector) {
        const $modal = $(modalSelector);

        // Preview ảnh 
        $modal.find('#image').on('change', function () {
            var reader = new FileReader();

            reader.onload = function (e) {
                $modal.find('#imagePreview').attr('src', e.target.result).show(); // lấy result gắn vào src
            };

            reader.readAsDataURL(this.files[0]); //chuyển sang dạng base64 và gắn vào src
        });

        // Reset preview ảnh 
        $modal.on('hidden.bs.modal', function () { // sự kiện của bootstrap khi đóng modal
            $modal.find('#imagePreview').attr('src', '#').hide();
            $modal.find('#image').val('');
        });
    }

    ImagePreview('#createModal');




    $(document).on('click', '.edit-product', function (e) {
        e.preventDefault();
        if (!_permissions.edit) {
            abp.message.warn("Bạn không đủ quyền để chỉnh sửa sản phẩm!");
            return;
        }
        var productId = $(this).attr('data-product-id');
        console.log('Opening edit modal for product:', productId);

        abp.ajax({
            url: abp.appPath + 'Products/EditModal?productId=' + productId, 
            type: 'POST',
            dataType: 'html',
            success: function (content) {
                $('#editModal div.modal-content').html(content);
                //$('#editModal').modal('show');
                ImagePreview('#editModal');
            },
            error: function (e) {
                abp.notify.error(l('ErrorLoadingData'));
            }
        });
    });

    abp.event.on('product.edited', (data) => {
        _$productsTable.ajax.reload();
    });



    $(document).on('click', '.delete-product', function () {

        if (!_permissions.delete) {
            abp.message.warn("Bạn không đủ quyền để xoá sản phẩm!");
            return;
        }

        var productId = $(this).attr('data-product-id');
        var productName = $(this).attr('data-product-name');
        deleteProduct(productId, productName);


    });

    function deleteProduct(productId, productName) {
        abp.message.confirm(           // confirm(message,title,callback)
            abp.utils.formatString( // chèn productName vào nội dung confirm
                l('AreYouSureWantToDelete'),
                productName),
            "Xác nhận xóa sản phẩm",
            (isConfirmed) => {
                if (isConfirmed) {
                    _productService.delete(productId).done(() => {
                        abp.message.success(l('SuccessfullyDeleted'), l('Success'));
                        _$productsTable.ajax.reload();
                    });
                }
            }
        );
    }


    $('.btn-search').on('click', (e) => {
        _$productsTable.ajax.reload();
    });

    $('.txt-search').on('keypress', (e) => {
        if (e.which == 13) {
            _$productsTable.ajax.reload();
            return false;
        }
    });

    // Thêm sự kiện cho nút tìm kiếm trong dropdown
    $('.dropdown-menu .btn-search').on('click', function () {
        _$productsTable.ajax.reload();
        $('.dropdown-menu').removeClass('show');
    });

    // Thêm sự kiện cho nút xóa trong dropdown
    $('.dropdown-menu .btn-clear').on('click', function () {
        $('.dropdown-menu input[name="MinPrice"]').val('');
        $('.dropdown-menu input[name="MaxPrice"]').val('');
        $('.dropdown-menu select[name="CategoryId"]').val('');
        _$productsTable.ajax.reload();
        $('.dropdown-menu').removeClass('show');
    });

    // Thêm validation cho input giá
    $('.dropdown-menu input[name="MinPrice"], .dropdown-menu input[name="MaxPrice"]').on('input', function () {
        var value = parseFloat($(this).val());
        if (!isNaN(value) && value < 0) {
            $(this).val(0);
        }
    });

    // Thêm sự kiện cho phím Enter trong input giá
    $('.dropdown-menu input[name="MinPrice"], .dropdown-menu input[name="MaxPrice"]').on('keypress', function (e) {
        if (e.which == 13) {
            e.preventDefault();
            $('.dropdown-menu .btn-search').click();
        }
    });

    // Add click handler for add to cart button
    $(document).on('click', '.add-to-cart', function () {
        var productId = $(this).attr('data-product-id');
        var productName = $(this).attr('data-product-name');
        
        abp.ajax({
            url: abp.appPath + 'Cart/AddToCart',
            type: 'POST',
            data: JSON.stringify({
                productId: productId,
                quantity: 1
            }),
            success: function () {
                abp.notify.success(l('ProductAddedToCart'));
            },
            error: function (error) {
                if (error.responseJSON && error.responseJSON.error && error.responseJSON.error.message) {
                    abp.notify.error(error.responseJSON.error.message);
                } else {
                    abp.notify.error(l('ErrorAddingToCart'));
                }
            }
        });
    });

    $(function () {
        var _cartService = abp.services.app.cart;

        // Xử lý tăng số lượng
        $('.increase-quantity').click(function () {
            var productId = $(this).data('product-id');
            var input = $('input[data-product-id="' + productId + '"]');
            var currentValue = parseInt(input.val());
            input.val(currentValue + 1).trigger('change');
        });

        // Xử lý giảm số lượng
        $('.decrease-quantity').click(function () {
            var productId = $(this).data('product-id');
            var input = $('input[data-product-id="' + productId + '"]');
            var currentValue = parseInt(input.val());
            if (currentValue > 1) {
                input.val(currentValue - 1).trigger('change');
            }
        });

        // Xử lý thay đổi số lượng
        $('.quantity-input').change(function () {
            var productId = $(this).data('product-id');
            var quantity = parseInt($(this).val());

            if (quantity < 1) {
                $(this).val(1);
                quantity = 1;
            }

            updateCartItem(productId, quantity);
        });

        // Xử lý xóa sản phẩm khỏi giỏ hàng
        $('.remove-from-cart').click(function () {
            var productId = $(this).data('product-id');
            
            abp.message.confirm(
                app.localize('AreYouSureToRemoveTheCartItem'),
                app.localize('Confirmation'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _cartService.removeFromCart({
                            productId: productId
                        }).done(function () {
                            abp.notify.success(app.localize('SuccessfullyRemoved'));
                            location.reload();
                        });
                    }
                }
            );
        });

        // Xử lý nút thanh toán
        $('#checkoutButton').click(function () {
            abp.message.confirm(
                app.localize('AreYouSureToCheckout'),
                app.localize('Confirmation'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _cartService.checkout().done(function () {
                            abp.notify.success(app.localize('CheckoutSuccessful'));
                            location.reload();
                        });
                    }
                }
            );
        });

        function updateCartItem(productId, quantity) {
            _cartService.updateCartItem({
                productId: productId,
                quantity: quantity
            }).done(function () {
                location.reload();
            });
        }
    });

})(jQuery);
