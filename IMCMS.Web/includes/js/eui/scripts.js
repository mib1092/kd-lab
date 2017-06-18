function stickyBtn() {
  if ($('.section-structural').length) {
    $(window).on('load resize', function () {
      setTimeout(function () {
        var section = $('.section-structural'),
            content = section.find('.center-wrap'),
            contentHeight = content.outerHeight(),
            positionTop = content.position().top,
            positionBottom = positionTop + contentHeight,
            height = $(window).height(),
            btn = section.find('.btn-estimate-box'),
            btnHeight = btn.outerHeight(),
            btnTopPosition = height - 98 - btnHeight,
            btnBotoomPosition = height - 98;

        $(window).on('scroll', function () {
          var currentPosition = $(window).scrollTop();

          if (currentPosition > (positionTop - btnTopPosition) && currentPosition < (positionBottom - btnBotoomPosition)) {
            if (!(btn.hasClass('b-fixed'))) {
              btn.addClass('b-fixed');
              btn.css('top', 'auto');
              btn.css('bottom', '98px');
            }
          } else if (currentPosition < (positionTop - btnTopPosition)) {
            if (btn.hasClass('b-fixed')) {
              btn.removeClass('b-fixed');
              btn.css('top', '0px');
              btn.css('bottom', 'auto');
            }
          } else if (currentPosition > (positionBottom - btnBotoomPosition)) {
            if (btn.hasClass('b-fixed')) {
              btn.removeClass('b-fixed');
              btn.css('top', 'auto');
              btn.css('bottom', '0px');
            }
          }
        });
      }, 300);
    });
  }
}
function msieversion() {
  var ua = window.navigator.userAgent;
  var msie = ua.indexOf("MSIE ");

  if (msie > 0) {
    $('body').addClass('ie');
  }

  return false;
}
function setMaxOuterHeight(box) {
  var maxHeight = 0;
  box.each(function () {
    if ($(this).outerHeight() > maxHeight) {
      maxHeight = $(this).outerHeight();
    }
  });
  box.height(maxHeight);
}
$(document).ready(function () {
  msieversion();

  body = $('body');

  // for submenu
  $('.open-overlay, .close-overlay').on('click', function () {
    if ($(this).hasClass('open-overlay')) {
      body.addClass('disable-scroll');
      $(this).next().addClass('open').fadeIn(550);
    } else if ($(this).hasClass('close-overlay')) {
      body.removeClass('disable-scroll');
      $(this).parent().removeClass('open').fadeOut(550);
    }
  });
  $(document).on('keyup', function (e) {
    if (e.keyCode == 27) {
      body.removeClass('disable-scroll');
      $('.submenu-overlay.open').removeClass('open').fadeOut(550);
    }
  });

  //for smooth scroll
  smoothScroll.init({
    selector: '.smooth-scroll, a', // Selector for links (must be a class, ID, data attribute, or element tag)
    speed: 500, // Integer. How fast to complete the scroll in milliseconds
    easing: 'easeInQuad', // Easing pattern to use
    offset: 130 // Integer. How far to offset the scrolling anchor location in pixels
  });

  // for empty links
  $('.prevent').on('click', function (event) {
    event.preventDefault();
  });

  // for main banner
  //if ($('.browserHeight').length) {
  //  $(window).on('load resize', function () {
  //    var banner = $('.browserHeight'),
  //        winHeight = $(window).height() - 106;

  //    banner.height(winHeight);
  //  });
  //}

  //for footer
  // $(window).on('load resize', function() {
  //     if($('.footer-top-list').length) {
  //         var box = $('.footer-table');
  //         setMaxOuterHeight(box);
  //     }
  // });

  //for sticky btn
  stickyBtn();

  //for slider
  if ($('.s-slider').length) {
    var swiper = new Swiper('.swiper-container', {
      pagination: '.swiper-pagination',
      paginationClickable: true,
      nextButton: '.swiper-button-next',
      prevButton: '.swiper-button-prev',
      autoHeight: true,
      centeredSlides: true,
      loop: true,
      observeParents: true
    });
    var duration = 100;
    $('.swiper-slide, .close-swiper').on('click', function () {
      if ($(this).hasClass('close-swiper')) {
        var popup = $(this).parents('.slider-popup.open');
      } else {
        var popup = $(this).parents('.slider-popup');
      }
      var sliderBox = popup.parents('.slider-box'),
          sliderBoxHeight = sliderBox.outerHeight(),
          slide = popup.find('.swiper-slide'),
          popupWrap = popup.find('.slider-popup-wrap'),
          wrap = popup.find('.swiper-wrapper'),
          img = wrap.find('.swiper-slide img'),
          winHeight = $(window).height(),
          imgHeight = winHeight - 126,
          wrapMaxHeight = imgHeight + 86,
          activeIndex = swiper.activeIndex;

      if ($(this).hasClass('close-swiper')) {
        popup.addClass('hide');
        setTimeout(function () {
          popup.removeClass('open');
          slide.removeClass('dis');
          body.removeClass('disable-scroll');
          img.css('max-height', '');
          wrap.css('max-height', '');
          popupWrap.css('margin-top', '');
          sliderBox.css('height', '');
          setTimeout(function () {
            swiper.update();
            swiper.slideTo(activeIndex, 0, false);
          }, duration);
        }, 350);
        setTimeout(function () {
          popup.removeClass('show');
          popup.removeClass('hide');
        }, 700);
      } else if (!($(this).hasClass('dis'))) {
        sliderBox.css('height', sliderBoxHeight);
        popup.addClass('open');
        slide.addClass('dis');
        body.addClass('disable-scroll');
        img.css('max-height', imgHeight);
        wrap.css('max-height', wrapMaxHeight);
        setTimeout(function () {
          popup.addClass('show');
        }, 500);
        setTimeout(function () {

          swiper.update();
          swiper.slideTo(activeIndex, 0, false);
          var h = ((winHeight - wrap.outerHeight()) / 2) - 43;
          popupWrap.css('margin-top', h);
        }, duration);
      }
    });

    $(window).on('load resize', function () {
      if ($('.slider-popup').hasClass('open')) {
        var popup = $('.slider-popup.open'),
            slide = popup.find('.swiper-slide'),
            popupWrap = popup.find('.slider-popup-wrap'),
            wrap = popup.find('.swiper-wrapper'),
            img = wrap.find('.swiper-slide img'),
            winHeight = $(window).height(),
            imgHeight = winHeight - 126,
            wrapMaxHeight = imgHeight + 86;

        img.css('max-height', imgHeight);
        wrap.css('max-height', wrapMaxHeight);
        setTimeout(function () {
          swiper.update();
          var h = ((winHeight - wrap.outerHeight()) / 2) - 43;
          popupWrap.css('margin-top', h);

        }, duration)
      }
    })
  }
  //for btn
  $(window).on('load resize', function () {
    if ($('.leadership-cell .btn').length) {
      var cell = $('.leadership-cell'),
          btn = cell.find('.btn'),
          cellWidth = cell.width(),
          btnWidth = btn.outerWidth();

      if (cellWidth < btnWidth) {
        btn.css('max-width', cellWidth);
        btn.css('min-width', 'auto');
        btn.addClass('small');
      } else {
        btn.css('max-width', '');
        btn.css('min-width', '');
        btn.removeClass('small');
      }
    }
  });

  //for select
  $('select').on('change', function () {
    var color = $(this).find('option:selected').attr('value');
    if (color) {
      $(this).addClass('check');
    } else {
      $(this).removeClass('check');
    }
  });

  //for job history
  $('.right-arrow.job').on('click', function () {
    var btn = $(this),
        jobBox = btn.prev(),
        jobForm = jobBox.find('.job-history-list:first');

    jobForm.clone().appendTo(jobBox);
  })
});