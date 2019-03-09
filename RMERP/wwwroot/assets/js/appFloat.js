

$(document).ready(function() {	
	
	// Select2	
	if($('.select').length > 0 ){
		$('.select').select2({
			minimumResultsForSearch: -1,
			width: '100%'
		});
	}
	
	// Floating Label

	if($('.floating').length > 0 ){
		$('.floating').on('focus blur', function (e) {
		$(this).parents('.form-focus').toggleClass('focused', (e.type === 'focus' || this.value.length > 0));
		}).trigger('blur');
	}
    
});