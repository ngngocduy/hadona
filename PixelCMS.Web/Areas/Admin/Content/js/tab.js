jQuery(document).ready(function ($) {

    /*----------------------------------------------------*/
    /*	Tabs
    /*----------------------------------------------------*/

    (function () {

        var $tabsNav = $('.tabs-nav'),
			$tabsNavLis = $tabsNav.children('li'),
			$tabContent = $('.tab-content');
        $tabsNav.each(function () {
            var $this = $(this);

            $this.next().children('.tab-content').stop(true, true)
												 .first().show();

            $this.children('li').first().addClass('active').stop(true, true).show();
        });

        $tabsNavLis.on('click', function (e) {
            var $this = $(this);

            $this.siblings().removeClass('active').end()
				 .addClass('active');

            $this.parent().next().children('.tab-content').stop(true, true).hide()
														  .siblings($this.find('a').attr('href')).fadeIn();

            e.preventDefault();
        });

    })();
    setTimeout(function () {
        $('.tabs-nav li').first().mouseenter().click();
    }, 3000);

    /* End Document */
});
