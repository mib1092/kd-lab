$(function () {
  $('#adminNavExpand').on('click', function (e) {
    e.preventDefault();
    $('.adminCollapsed').removeClass('adminCollapsed').addClass('adminOpen');
  });
  $('.adminNavCollapseTrigger').on('click', function (e) {
    e.preventDefault();
    $('.fullWrap').removeClass('adminOpen').addClass('adminCollapsed');
  });

  $('[data-showadminsubnav]').on('click', function (e) {
    e.preventDefault();
    var toShow = $(this).data('showadminsubnav'), adminSubnavbar = $('[data-adminsubnav="' + toShow + '"]');

    if (adminSubnavbar.length == 0)
      throw new Error("Unable to find subnav for " + toShow);

    if (adminSubnavbar.hasClass('expanded')) {
      adminSubnavbar.removeClass('expanded');
    } else {
      $('.adminSubnavWrap.expanded').removeClass('expanded');
      adminSubnavbar.addClass('expanded');
    }
  });

  $('.closeAdminSubnav').on('click', function (e) {
    $(this).closest('.adminSubnavWrap').removeClass('expanded');
  })
})