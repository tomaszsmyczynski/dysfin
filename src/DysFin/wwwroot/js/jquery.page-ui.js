$(document).ready(function() {
	
	//Scrolled menu
	var $document = $(document), $element = $('header');		
	$document.scroll(function() {
	  if ($document.scrollTop() >= 50 ) {
		$element.addClass('scrolled');
	  } else {		
		$element.removeClass('scrolled');
	  }
	});
	//------
	
	//Mobile menu
	$('.menu-toggle').click(function(){
		$(this).toggleClass('open');
		$('.menu-mobile ul').slideToggle(250);
	});
	//------
	
	//Faq toggle boxes
	$('.faq-toggle').click(function(){
		$(this).toggleClass('active');
		$(this).next('.faq-more').slideToggle(250);
	});
	//------
	
	//More/less boxes
	$('.moreless-toggle').click(function(){
		$(this).toggleClass('active');
		$(this).next('.moreless-content').slideToggle(250);
	});
	//------
	
	//User menu toggle
	$('.um-toggle').click(function(){
		$(this).toggleClass('active');
		$(this).next('.um-submenu').slideToggle(250);
	});
	//------
		
	//Page 404 align
	if ($('.al-intro-sm').length>0) {
		if ($('.col-admin-login-right').height()>$(window).height()) {
			$('.al-intro-sm').height($('.col-admin-login-right').height()-220);
		} else {
			$('.al-intro-sm').height($(window).height()-220);
		}
	}	
	//------
	
	//Header 2 dropdown (user menu)
	$('.h2d-toggle').click(function(){
		$(this).toggleClass('active');
		$(this).next('.h2d-more').slideToggle(250);
	});
	//------
	
	//Table row toggle
	$('.tr-toggle').click(function(){
		$(this).next('.tr-more').toggle();
	});
	//------
	
	//Contrast button toggle
	$('.contrast-page-toggle').click(function(e){
		e.preventDefault();
		$('body').toggleClass('page-contrast');
	});
	//------
	
	//Font size button
	var font_size_toggle_i = 0;
	$('.font-size-toggle').click(function() {
		font_size_toggle_i++;
		$('*').each(function () {
			if ($(this).css('font-size').indexOf('px')>0) {				
				if (font_size_toggle_i<3) {
					font_size = parseInt($(this).css('font-size').substr( 0, $(this).css('font-size').indexOf('px') ))+parseInt(3)+'px';
				} else {
					font_size = parseInt($(this).css('font-size').substr( 0, $(this).css('font-size').indexOf('px') ))-parseInt(6)+'px';
				}				
				$(this).attr({
					'data-font-size' : font_size
				});
			}		
		});
		if (font_size_toggle_i>2) {
			font_size_toggle_i = 0;
		}
		$('*').each(function () {
			$(this).css({
				'font-size' : $(this).attr('data-font-size')
			});
		});
	});
	//------	
	
	//Flexible horizontal tabs navigation
	tabs_loaded = true;
	initial_element = $('.tabs-menu').html();
	tabs_count = $('.tabs li').length;
	function calculateTabs(load_first) {
		$('.tabs-menu').html(initial_element);
		tabs_width = $('.tabs').width();	
		content_width = 0;		
		if (load_first) {
			load_first_fix = 2;
		} else {
			load_first_fix = 4;
		}
		$('.tabs li').each(function() {		
			if (!$(this).hasClass('tabs-toggle')) {
				content_width += $(this).width()+load_first_fix;
				if ($(this).index()+3>tabs_count) {
					toggle_width = 0;
				} else {
					toggle_width = $('.tabs-toggle').width();
				}
				//console.log($('.tabs li').length);
				if (content_width+toggle_width>tabs_width) {
					$('.tabs-toggle').css({display: 'inline-block'});
					$(this).appendTo(".tabs-more");					
				}
			}
		});
	}
	$(window).resize(function() {
		tabs_loaded = false;
		calculateTabs(tabs_loaded);
	});
	calculateTabs(tabs_loaded);
	
	$(window).click(function() {
		$('.tabs-more').slideUp(250);
	});
	$('body').on('click', '.tabs-toggle', function(event) {
		event.stopPropagation();
		$('.tabs-more').slideToggle(250);
	});
	//------
	
	//Toggle table columns
	function createCookie(name,value,days) {
		if (days) {
			var date = new Date();
			date.setTime(date.getTime()+(days*24*60*60*1000));
			var expires = "; expires="+date.toGMTString();
		}
		else var expires = "";
		document.cookie = name+"="+value+expires+"; path=/";
	}

	function readCookie(name) {
		var nameEQ = name + "=";
		var ca = document.cookie.split(';');
		for(var i=0;i < ca.length;i++) {
			var c = ca[i];
			while (c.charAt(0)==' ') c = c.substring(1,c.length);
			if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length,c.length);
		}
		return null;
	}

	function eraseCookie(name) {
		createCookie(name,"",-1);
	}
	
	$('.fs-columns input').change(function() {
		column = $(this).parent().parent().parent().index();
		if ($(this).prop('checked')) {
			eraseCookie($(this).prop('name'));
			$('#main-table tr').each(function() {
				if (!$(this).hasClass('tr-more')) {
					$(this).children('td:nth('+column+')').show();
					if ($(this).index()==0) {					
						$(this).children('th:nth('+column+')').show();
					}
				}
			});
		} else {
			createCookie($(this).prop('name'),true,30);
			$('#main-table tr').each(function() {
				if (!$(this).hasClass('tr-more')) {
					$(this).children('td:nth('+column+')').hide();
					if ($(this).index()==0) {
						$(this).children('th:nth('+column+')').hide();
					}
				}
			});
		}
	});
	
	$('.fs-columns input').each(function() {
		column = $(this).parent().parent().parent().index();
		if ($(this).prop('checked')) {
			$('#main-table tr').each(function() {
				if (!$(this).hasClass('tr-more')) {
					$(this).children('td:nth('+column+')').show();
					if ($(this).index()==0) {					
						$(this).children('th:nth('+column+')').show();
					}
				}
			});
		} else {
			$('#main-table tr').each(function() {
				if (!$(this).hasClass('tr-more')) {
					$(this).children('td:nth('+column+')').hide();
					if ($(this).index()==0) {					
						$(this).children('th:nth('+column+')').hide();
					}
				}
			});
		}
	});
	
	$(window).click(function() {
		$('.fs-list').slideUp(250);
	});
	$('body').on('click', '.action-box > .btn', function(event) {
		event.stopPropagation();
		$(this).next('ul').slideToggle(250);
	});
	
	$('body').on('click', '.action-box', function(event) {
		event.stopPropagation();
	});
	//-----
	
	//Side panel
	// $('.side-panel').each(function() {
		// sp_height = parseInt($(this).height())+parseInt(60);
		// //if ($(this).next('div').height()>parseInt(sp_height)+parseInt(60)) {
			// //$(this).css({'min-height':$(this).next('div').height()+'px'});
		// //} else {
			// $(this).parent().css({'min-height':sp_height+'px'});
		// //}
	// });
	$('.side-toggle').click(function() {
		$('.side-panel').hide();
		$('.side-panel').next('div').removeClass('col-md-8');
		$('.side-panel').next('div').addClass('col-md-12');
		$('#side-open').show();		
	});
	
	$('#side-open').click(function() {
		$('.side-panel').show();
		$(this).parent().addClass('col-md-8');
		$(this).parent().removeClass('col-md-12');
		$(this).hide();
	});
	//-------
	
	//Footer menus on mobile
	$('footer h4').click(function() {
		if ($(window).width()<768) {
			$(this).toggleClass('active');
			$(this).next('ul').slideToggle(250);
		}
	});	
	//-------
	
	//Footer menus on mobile
	$('.submenu-toggle').click(function() {
		if ($(this).hasClass('active')) {
			$(this).removeClass('active');
			$(this).parent().children('.submenu-more').slideUp(250);
		} else {
			submenu_toggle = $(this);
			$('.submenu-toggle').removeClass('active');
			$('.submenu-more').slideUp(250);
			//$('.submenu-more').promise().done(function() {
				submenu_toggle.addClass('active');
				submenu_toggle.parent().children('.submenu-more').slideDown(250);
			//});
		}
	});	
	//-------
	
});