$(function () {
    var _cartService = abp.services.app.cart;

    // Handle quantity increase
    $('.increase-quantity').click(function () {
        var productId = $(this).data('product-id');
        var input = $('input[data-product-id="' + productId + '"]');
        var currentValue = parseInt(input.val());
        input.val(currentValue + 1).trigger('change');
    });

    // Handle quantity decrease
    $('.decrease-quantity').click(function () {
        var productId = $(this).data('product-id');
        var input = $('input[data-product-id="' + productId + '"]');
        var currentValue = parseInt(input.val());
        if (currentValue > 1) {
            input.val(currentValue - 1).trigger('change');
        }
    });

    // Handle quantity change
    $('.quantity-input').change(function () {
        var productId = $(this).data('product-id');
        var quantity = parseInt($(this).val());
        var card = $(this).closest('.card');

        if (quantity < 1) {
            $(this).val(1);
            quantity = 1;
        }

        // Get product price
        var price = parseFloat(card.find('.text-success').text().replace(/[^0-9.-]+/g, ''));

        // Calculate and update item total
        var totalPrice = price * quantity;
        card.find('.text-end.fw-medium').text(formatCurrency(totalPrice));

        // Calculate cart total
        var cartTotal = 0;
        $('.quantity-input').each(function () {
            var qty = parseInt($(this).val());
            var prc = parseFloat($(this).closest('.card').find('.text-success').text().replace(/[^0-9.-]+/g, ''));
            cartTotal += qty * prc;
        });

        // Update cart totals in summary
        $('.fw-bold.fs-5 span:last').text(formatCurrency(cartTotal));
        $('.fw-medium span:last').text(formatCurrency(cartTotal));

        // Update item summary
        updateItemSummary(productId, quantity, totalPrice);

        // Call API to update server
        updateCartItem(productId, quantity);
    });

    // Handle remove from cart
    $('.remove-from-cart').click(function () {
        var productId = $(this).data('product-id');
        var card = $(this).closest('.card');

        abp.message.confirm(
            abp.localization.localize('AreYouSureToRemoveTheCartItem', 'SimpleTaskApp'),
            abp.localization.localize('Confirmation', 'SimpleTaskApp'),
            function (isConfirmed) {
                if (isConfirmed) {
                    _cartService.removeFromCart({
                        productId: productId
                    }).done(function () {
                        abp.notify.success(abp.localization.localize('SuccessfullyRemoved', 'SimpleTaskApp'));
                        card.fadeOut(300, function() {
                            $(this).remove();
                            updateCartSummary();
                            if ($('.card').length === 0) {
                                location.reload(); // Reload to show empty cart state
                            }
                        });
                    });
                }
            }
        );
    });

    // Handle checkout button
    $('#checkoutButton').click(function () {
        abp.message.confirm(
            abp.localization.localize('AreYouSureToCheckout', 'SimpleTaskApp'),
            abp.localization.localize('Confirmation', 'SimpleTaskApp'),
            function (isConfirmed) {
                if (isConfirmed) {
                    _cartService.checkout().done(function () {
                        abp.notify.success(abp.localization.localize('CheckoutSuccessful', 'SimpleTaskApp'));
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
        });
    }

    function updateItemSummary(productId, quantity, totalPrice) {
        var summaryItem = $('.border-top.border-bottom .d-flex').filter(function() {
            return $(this).find('span:first').text().includes(productId);
        });

        if (summaryItem.length) {
            summaryItem.find('span:first').text(productId + ' x ' + quantity);
            summaryItem.find('span:last').text(formatCurrency(totalPrice));
        }
    }

    function updateCartSummary() {
        var cartTotal = 0;
        $('.quantity-input').each(function () {
            var qty = parseInt($(this).val());
            var prc = parseFloat($(this).closest('.card').find('.text-success').text().replace(/[^0-9.-]+/g, ''));
            cartTotal += qty * prc;
        });

        $('.fw-bold.fs-5 span:last').text(formatCurrency(cartTotal));
        $('.fw-medium span:last').text(formatCurrency(cartTotal));
    }

    function formatCurrency(amount) {
        return '₫' + amount.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
    }
});