(function ($) {
    var _categoriesService = abp.services.app.categories,
        l = abp.localization.getSource('SimpleTaskApp'),
        _$editModal = $('#editModal'),
        _$editForm = _$editModal.find('form');


    function save() {
        if (!_$editForm.valid()) {
            return;
        }


        var categories = _$editForm.serializeFormToObject();
        console.log('Dữ liệu categories gửi đi :', categories);  


        abp.ui.setBusy(_$editForm);  // hiển thị trạng thái loading
        _categoriesService.update(categories).done(function () {  // gọi update ở service
            _$editModal.modal('hide');
            abp.message.success(l('SavedSuccessfully'), l('Success'));
            abp.event.trigger('category.edited', categories);
        }).always(function () {
            abp.ui.clearBusy(_$editForm); // tắt loading
        });
    }

    // closest tìm phần từ cha gần nhất 
    _$editForm.closest('div.modal-content').find(".save-button").click(function (e) {
        e.preventDefault();
        save();
    });



    _$editForm.validate({
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

})(jQuery)