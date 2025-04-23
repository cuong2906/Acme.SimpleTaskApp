(function ($) {
    var _orderService = abp.services.app.order,
        l = abp.localization.getSource('SimpleTaskApp'),
        _$modal = $('#editModal'),
        _$form = _$modal.find('form');

    _$form.validate({
        rules: {
            Status: "required"
        }
    });

    _$form.find('button[type="submit"]').click(function (e) {
        e.preventDefault();

        if (!_$form.valid()) {
            return;
        }

        var order = _$form.serializeFormToObject();

        abp.ui.setBusy(_$modal);
        _orderService.updateOrderStatus({
            id: order.Id,
            status: parseInt(order.Status),
            note: order.Note
        }).done(function () {
            _$modal.modal('hide');
            abp.notify.info(l('SavedSuccessfully'));
            abp.event.trigger('order.edited', order);
        }).always(function () {
            abp.ui.clearBusy(_$modal);
        });
    });

    _$modal.on('shown.bs.modal', function () {
        _$modal.find('input:not([type=hidden]):first').focus();
    });
})(jQuery); 