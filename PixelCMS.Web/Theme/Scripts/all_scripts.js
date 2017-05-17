//Main Scripts
(function($) {

  $.fn.menumaker = function(options) {
      
      var cssmenu = $(this), settings = $.extend({
        title: "Menu",
        format: "dropdown",
        sticky: false
      }, options);

      return this.each(function() {
        cssmenu.prepend('<div id="menu-button">' + settings.title + '</div>');
        $(this).find("#menu-button").on('click', function(){
          $(this).toggleClass('menu-opened');
          var mainmenu = $(this).next('ul');
          if (mainmenu.hasClass('open')) { 
            mainmenu.hide().removeClass('open');
          }
          else {
            mainmenu.show().addClass('open');
            if (settings.format === "dropdown") {
              mainmenu.find('ul').slideToggle();
            }
          }
        });

        cssmenu.find('li ul').parent().addClass('has-sub');		

        multiTg = function() {
          cssmenu.find(".has-sub").prepend('<span class="submenu-button"></span>');
          cssmenu.find('.submenu-button').on('click', function() {
            $(this).toggleClass('submenu-opened');
            if ($(this).siblings('ul').hasClass('open')) {
              $(this).siblings('ul').removeClass('open').slideToggle();
            }
            else {
              $(this).siblings('ul').addClass('open').slideToggle();
            }
          });
        };

        if (settings.format === 'multitoggle') multiTg();
        else cssmenu.addClass('dropdown');

        if (settings.sticky === true) cssmenu.css('position', 'fixed');

        resizeFix = function() {
          if ($( window ).width() > 768) {
            cssmenu.find('ul').show();
          }

          if ($(window).width() <= 768) {
            cssmenu.find('ul').hide().removeClass('open');
          }
        };
        resizeFix();
        return $(window).on('resize', resizeFix);

      });
  };
})(jQuery);

(function($){
$(document).ready(function(){

$("#cssmenu").menumaker({
   title: "",
   format: "multitoggle"
});

});
})(jQuery);
$(document).ready(function(){							  			
		$('.button-click-footer').click(function () {
					$('#intro-page footer').toggleClass('footer-show');
					$('.button-click-footer').toggleClass('button-rotate');							
				});	 		
		// fade in #back-top
		$(function () {			
			// scroll body to 0px on click
			$('.btn_toTop').click(function () {
				$('body,html').animate({
					scrollTop: 0
				}, 500);
				return false;
			});
		});
$('.characteristic-items .char-item').addClass('wow slideInLeft');
$('.characteristic-items .char-item:nth-child(n+3)').addClass('wow slideInRight');		
$('.why-choose-items .why-item, .news-item').addClass('wow slideInUp');
$('.pro-item').addClass('wow zoomIn');
		$('#slide_banner').each(function(){
			if( $(this).find("div").length > 1 ) $(this).owlCarousel({
				autoplay: true,
				autoplayTimeout:3000,
				autoplayHoverPause:false,
				nav:true,
				navSpeed : 500,
				dots : false,
				margin: 0,
				smartSpeed: 500,
				loop: true,
				items:1,
				animateOut: 'fadeOut'
			  })
			});			
			$('.news-items').each(function(){
			if( $(this).find("div").length > 1 ) $(this).owlCarousel({
				autoplay: false,
				autoplayTimeout:3000,
				autoplayHoverPause:true,
				nav:true,
				navSpeed : 500,
				dots : false,
				margin: 30,
				smartSpeed: 1000,
				loop: true,
				items:3,
				responsive:{
					0:{items:1},
					480:{items:1},
					640:{items:2},
					980:{items:3}			
				}
			  })
			});
			$('.slide-brands').each(function(){
			if( $(this).find("div").length > 1 ) $(this).owlCarousel({
				autoplay: false,
				autoplayTimeout:3000,
				autoplayHoverPause:true,
				nav:true,
				navSpeed : 500,
				dots : false,
				margin: 20,
				smartSpeed: 1000,
				loop: true,
				items:5,
				responsive:{
					0:{items:1},
					380:{items:2},
					640:{items:3},
					768:{items:4},
					980:{items:5}				
				}
			  })
			});
			$(".fancybox-pro").fancybox({
								openEffect	: 'none',
								closeEffect	: 'none'
							});
			$(".tab-item").hide();
			$(".tab-item:first-child").show();
			$(".tabs a").click(function(event) {
				event.preventDefault();
				$(this).parent().addClass("active");
				$(this).parent().siblings().removeClass("active");
				var tab = $(this).attr("href");
				$(".tab-item").not(tab).css("display", "none");
				$(tab).fadeIn();			
			});
		//Video
		$(".video iframe, .info_g iframe").wrap( "<div class='videoWrapper'></div>" );
		$(".videoWrapper").wrap( "<div class='youtubevideowrap'></div>" );
});