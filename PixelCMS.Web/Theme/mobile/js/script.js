$(document).ready(function(){
	
	// Category expand/collapse
	$('.menu-list').hide();
	$('.menu-head').click(function(){
		$(this).next('.menu-list').slideToggle();
	});
	
	$('.submenu').hide();
	$(".expand").hide();
	$(".collapse").click(function(){
		$(this).parent(".menu").next(".submenu").slideToggle();		
		$(this).parent(".menu").children(".collapse").toggle();
	});
	$(".show").click(function(){
		$(this).parent(".menu").next("ul.submenu").slideToggle();		
		$(this).parent(".menu").children(".collapse").toggle();
	});

	// active Parent Category
	$('.menu-list li .menu a').click(function(){
		$('.menu-list li').removeClass('active');
		$(this).parent().parent().addClass('active');
	});
	// active Child Category		
	$('.submenu li .menu a').click(function(){
		$('.menu-list li').removeClass('active');
		$(this).parent().parent().parent().parent().addClass('active');
		$('.submenu li').removeClass('active');
		$(this).parent().parent().addClass('active');
	});
	
	// SLIDE
	$('.main-slider').bxSlider({
		auto: true,
		mode: 'fade'
	});
	$('.feature-slider').bxSlider({
		slideWidth: 180,
		minSlides: 2,
		maxSlides: 6,
		moveSlides: 1,
		controls: false,
		slideMargin: 10
	});
	$('.album-slider').bxSlider({
		auto: true,
		controls: false
	});
	
	/************ Tabs Fast Add ****************/
	$(".tab_content:not(:first)").hide();
	$(".tabs li a").click(function(){
		$(".tabs li").removeClass("active");
		$(this).parent("li").addClass("active");
		$(".tab_content").hide();
		
		var activeTab = $(this).attr('href');
		$(activeTab).fadeIn();
		return false;
	});
	
	// CART
	$('#enterpass').hide();
	$('#yes-acc').click(function(){
		$('#enterpass').show('slide');
	});
	$('#no-acc').click(function(){
		$('#enterpass').hide('slide');
	});
	
	// Fancybox
	$('.login-page').fancybox({
		width : '100%',
		height : '100%'
	});
	$('.signup-page').fancybox({
		width : '100%',
		height : '100%'
	});
	
});