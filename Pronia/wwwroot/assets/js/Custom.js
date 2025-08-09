$(function () {
    $('#quickViewColor, #quickViewSize').niceSelect();
});

$(document).on('click', '.quick-view', function (e) {
    e.preventDefault();
    let productId = $(this).data('id');

    $.get(`/Home/GetQuickView?id=${productId}`, function (data) {
        $('#quickViewTitle').text(data.name ?? '');
        $('#quickViewDescription').text(data.description || "No description");
        $('#quickViewPrice').text('$' + (data.price ?? 0));
        
        if (data.mainImage) {
            $('#quickViewImage').attr('src', '/admin/media/products/' + data.mainImage);
        } else {
            $('#quickViewImage').attr('src', '/assets/images/no-image.png');
        }
        
        let $colorSelect = $('#quickViewColor');
        $colorSelect.empty();
        if (data.colors && data.colors.length > 0) {
            data.colors.forEach(function (c) {
                $colorSelect.append(`<option value="${c.id}">${c.name}</option>`);
            });
        } else {
            $colorSelect.append(`<option disabled>No color</option>`);
        }
        
        let $sizeSelect = $('#quickViewSize');
        $sizeSelect.empty();
        if (data.sizes && data.sizes.length > 0) {
            data.sizes.forEach(function (s) {
                $sizeSelect.append(`<option value="${s.id}">${s.name}</option>`);
            });
        } else {
            $sizeSelect.append(`<option disabled>No size</option>`);
        }

        // Dropdown-u yenilə
        $colorSelect.niceSelect('update');
        $sizeSelect.niceSelect('update');

        // Quantity sıfırlanır
        $('#quickViewQty').val(1);
    });
});