<script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>

$(document).ready(function () {
    function fetchItems(categoryId = null, searchTerm = '') {
        $.ajax({
            url: '/Menu/FilterItems', // Use relative URL
            type: 'GET',
            data: { categoryId: categoryId, searchTerm: searchTerm },
            success: function (data) {
                $('#collapse1').html(data);
            },
            error: function () {
                alert('Error loading items.');
            }
        });
    }

    $('.category-link').click(function (e) {
        e.preventDefault();
        var categoryId = $(this).data('category-id');
        fetchItems(categoryId, $('#searchInput').val());
    });

    $('#searchInput').on('keyup', function () {
        fetchItems($('.nav-link.active').data('category-id'), $(this).val());
    });
});


