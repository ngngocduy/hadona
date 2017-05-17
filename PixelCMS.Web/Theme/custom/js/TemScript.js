$(document).ready(function(e) {
	
	$(".panel").not(":first").hide();
	$(".accor-bar").click(function() {
		if($(this).next(".panel").is(":visible")){
			$(this).removeClass('active');
			$(this).next(".panel").slideUp();
		} else {
			$(".accor-bar").removeClass('active');
			$(this).addClass('active');
			
			$(".panel").slideUp();
			$(this).next(".panel").slideToggle();
		}
	});
	
	// FORM
	// Edit
	$(".fedit").click(function(){		
		$(this).prev().prop("disabled", false);
	});
	
});