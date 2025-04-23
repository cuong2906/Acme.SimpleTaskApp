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

        // Lấy giá tiền của sản phẩm
        var price = parseFloat($(this).closest('tr').find('td:eq(1)').text().replace(/[^0-9.-]+/g, ''));

        // Tính và cập nhật tổng tiền của sản phẩm
        var totalPrice = price * quantity;
        $(this).closest('tr').find('td:eq(3)').text(formatCurrency(totalPrice));

        // Tính tổng tiền của giỏ hàng
        var cartTotal = 0;
        $('.quantity-input').each(function () {
            var qty = parseInt($(this).val());
            var prc = parseFloat($(this).closest('tr').find('td:eq(1)').text().replace(/[^0-9.-]+/g, ''));
            cartTotal += qty * prc;
        });

        // Cập nhật tổng tiền của giỏ hàng
        $('tfoot tr td:last').text(formatCurrency(cartTotal));

        // Gọi API để cập nhật server
        updateCartItem(productId, quantity);
    });

    // Xử lý xóa sản phẩm khỏi giỏ hàng
    $('.remove-from-cart').click(function () {
        var productId = $(this).data('product-id');

        abp.message.confirm(
            abp.localization.localize('AreYouSureToRemoveTheCartItem', 'SimpleTaskApp'),
            abp.localization.localize('Confirmation', 'SimpleTaskApp'),
            function (isConfirmed) {
                if (isConfirmed) {
                    _cartService.removeFromCart({
                        productId: productId
                    }).done(function () {
                        abp.notify.success(abp.localization.localize('SuccessfullyRemoved', 'SimpleTaskApp'));
                        location.reload();
                    });
                }
            }
        );
    });

    // Xử lý nút thanh toán
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

    function formatCurrency(amount) {
        return '₫' + amount.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
    }
});