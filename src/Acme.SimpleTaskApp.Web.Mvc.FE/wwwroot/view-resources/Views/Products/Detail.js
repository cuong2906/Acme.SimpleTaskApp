$(function () {
    var l = abp.localization.getSource('SimpleTaskApp');
    var _cartService = abp.services.app.cart;

    $('form').on('submit', function (e) {
        e.preventDefault();
        
        var $form = $(this);
        var $submitButton = $form.find('button[type="submit"]');
        
        $submitButton.prop('disabled', true);
        
        var productId = $form.find('input[name="id"]').val();
        
        _cartService.addToCart({
            productId: parseInt(productId),
            quantity: 1
        }).done(function () {
            abp.notify.success(l('ProductAddedToCart'), l('Success'));
            $submitButton.prop('disabled', false);
        }).fail(function (error) {
            abp.notify.error(error.message, l('Error'));
            $submitButton.prop('disabled', false);
        });
    });
}); 