(function ($) {
    var _productsService = abp.services.app.product,
        l = abp.localization.getSource('SimpleTaskApp'),
        _$modal = $('#editModal'),
        _$form = _$modal.find('form');


    function save() {
        if (!_$form.valid()) {
            return; // không submit nếu không hợp lệ
        }

        var formElement = _$form[0]; // DOM element
        var formData = new FormData(formElement); // Lấy toàn bộ form, bao gồm file

        // Thêm ExistingImageUrl vào formData nếu không có file mới
        if (!formData.get('ImageUrl') || formData.get('ImageUrl').size === 0) {
            var currentImageUrl = $('#imagePreview').attr('src');
            if (currentImageUrl && currentImageUrl !== '#') {
                formData.set('ExistingImageUrl', currentImageUrl);
            }
        }

        // Đảm bảo các trường dữ liệu được thêm vào formData
        var quantity = $('#Quantity').val();
        if (quantity) {
            formData.set('Quantity', quantity);
        }

        var price = $('#Price').val();
        if (price) {
            formData.set('Price', price);
        }

        var categoryId = $('#CategoryId').val();
        if (categoryId) {
            formData.set('CategoryId', categoryId);
        }

        if (!formData.get('Discount')) {
            formData.set('Discount', 0);
        }
        abp.ui.setBusy(_$form);

        $.ajax({
            url: abp.appPath + 'Products/Update',
            type: 'POST',
            data: formData,
            processData: false, // Không xử lý dữ liệu
            contentType: false, // Không đặt content-type mặc định
            success: function () {
                _$modal.modal('hide');
                abp.notify.success(l('SavedSuccessfully'));
                // Cập nhật lại bảng dữ liệu
                var $productsTable = $('#ProductsTable').DataTable();
                $productsTable.ajax.reload(null, false); // false để giữ nguyên trang hiện tại
            },
            error: function (err) {
                //abp.notify.error('Lỗi khi cập nhật sản phẩm!');
                abp.notify.error(l('SavedFailed'));
                console.error(err);
            },
            complete: function () {
                abp.ui.clearBusy(_$form);
            }
        });
    }



    _$form.closest('div.modal-content').find(".save-button").click(function (e) {
        e.preventDefault();
        save();
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
                max: 2000000000
            },
            Quantity: {
                required: true,
                number: true,
                min: 0,
                max: 10000
            },
            Discount: {
                number: true,
                min: 0,
                max: 100
            },
            imagePreview: {
                required: true,
                imageExtension: true,

            },
            ImageUrl: {
                filesize: 50 * 1024 * 1024
            },
            CategoryId: {
                required: true
            }
        },
        messages: {
            Name: {
                required: "Tên sản phẩm không được để trống",
                minlength: "Tên ít nhất 3 ký tự",
                maxlength: "Tên tối đa 100 ký tự"
            },
            Price: {
                required: "Vui lòng nhập giá",
                number: "Giá phải là số",
                min: "Giá phải lớn hơn hoặc bằng 0",
                max: "Số tiền quá lớn(không quá 20 triệu",
            },
            Quantity: {
                required: "Vui lòng nhập số lượng",
                number: "Số lượng phải là số",
                min: "Số lượng phải lớn hơn hoặc bằng 0",
                max: "Số lượng không được vượt quá 10,000"
            },
            Discount: {
                number: "Giảm giá phải là số",
                min: "Tối thiểu là 0%",
                max: "Tối đa là 100%"
            },
            imagePreview: {
                required: "Vui lòng chọn ảnh",
                imageExtension: "Chỉ chấp nhận file ảnh JPG, PNG, GIF, BMP",

            },
            ImageUrl: {
                filesize: "Dung lượng ảnh tối đa là 2MB"
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



})(jQuery)