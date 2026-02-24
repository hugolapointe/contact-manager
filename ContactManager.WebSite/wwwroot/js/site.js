$(function () {
	// Confirmation unique pour toutes les actions de suppression.
	$('.delete-button').on('click', function () {
		const name = $(this).data('name') || 'this item';
		const message = `Are you sure you want to delete ${name}?`;

		if (confirm(message)) {
			$(this).closest('form').submit();
		}
	});
});
