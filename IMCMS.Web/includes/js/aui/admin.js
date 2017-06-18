$(function () {
    $.ajaxPrefilter(function (options, originalOptions, jqXHR) {
        if (options.type.toUpperCase() === "POST") {
            // We need to add the verificationToken to all POSTs
            var token = $("input[name^=__RequestVerificationToken]").first();
            if (!token.length) return;

            var tokenName = token.attr("name");

            // If the data is JSON, then we need to put the token in the QueryString:
            if (options.contentType.indexOf('application/json') === 0) {
                // Add the token to the URL, because we can't add it to the JSON data:
                options.url += ((options.url.indexOf("?") === -1) ? "?" : "&") + token.serialize();
            } else if (typeof options.data === 'string' && options.data.indexOf(tokenName) === -1
	            || (options.contentType.indexOf("application/x-www-form-urlencoded") == 0)) {
            	// Append to the data string:
	            options.data = options.data || "";
                options.data += (options.data ? "&" : "") + token.serialize();
            }
        }
    });


    if (jQuery.datepicker)
        $('.datepicker').datepicker(); // run date picker

    if (typeof (CKEDITOR) !== "undefined" && CKEDITOR) {
        $('textarea.HTMLeditor, textarea.html-editor').each(function () {
            Admin.CreateEditor($(this).get(0));
        });
    }

    $(window).bind('beforeunload', function () {
        if (Admin.isDirty && !Admin.byPassDirty) {
            return "There are unsaved changed on the page.";
        }
    });

    $(':input:not(.no-dirty)').on('keypress change', function () {
        Admin.setDirty();
    });

    $('.sm_save, #sm_cancel a, .save, #cancel a, .cancel').click(function () {
        Admin.byPassDirty = true;
    });

    $('form').submit(function () {
        Admin.byPassDirty = true;
        var shouldContinueSubmit = true;
        
        for (var i = 0; i < Admin.Editors.length; i++) {
            Admin.Editors[i].updateElement();
        }
        
        $('input, textarea').each(function () {
            var pattern = new RegExp("http(s?):\/\/(.*)(.?)(imcrs.com)");
            if (pattern.test($(this).val())) {
                shouldContinueSubmit = confirm("Looks like a staging URL was used in the content.\n\nThis could cause long term issues with broken links when the site is live.\n\nDo you want to continue with the save?");
            }
        });

        return shouldContinueSubmit;
    });

    $('.closebutton').click(function () {
        $(this).parents('.message').slideUp('fast');
        _gaq.push(['_trackEvent', 'Message', 'Close', $(this).parents('.message').data('type')]);
    });

    $('.adminOverlayClose').on('click', function () {
        $('.adminOverlayWrap').fadeOut();
    });

    $('th.sortableColumn').click(function () {
    	var direction = $(this).hasClass('sortedUp') ? 'desc' : 'asc';
    	var column = $(this).data('column');
    	var url = document.location.href.replace(/(\?|&)d=(desc|asc)&c=(.)+/g, '');
    	var qs = ((url.indexOf('?') != -1) ? '&' : '?') + 'd=' + direction + '&c=' + column;
    	document.location.href = url + qs;
    });
});


function executeDelete($this, deleteUrl, undoUrl) {
	var $par = $this.parents('.data').first();
	var itemid = $par.data('id');
	$par.css('opacity', '.3');
	_gaq.push(['_trackEvent', 'Delete', deleteUrl, itemid]);
	$.post(deleteUrl, { id: itemid }, function (d) {
		$par.hide();
		$.event.trigger({
			type: "Admin.delete",
			id: itemid,
			element: $par
		});
		$('#undo-bar a#undo-action').click(function (e) {
			e.preventDefault();
			_gaq.push(['_trackEvent', 'Undo', undoUrl, itemid]);
			$.post(undoUrl, { id: itemid }, function (d) {
				$par.css('opacity', '1').show();
				$('#undo-bar').hide();
				$.event.trigger({
					type: "Admin.undo",
					id: itemid,
					element: $par
				});
			});
		});
		$('#undo-bar').show();
	}).fail(function () {
		$par.css('opacity', '1');
		alert("The delete operation didn't complete successfully. Maybe try again?");
		_gaq.push(['_trackEvent', 'Error', 'Delete']);
	});
};

(function ($) {
    $.fn.adminDelete = function (deleteUrl, undoUrl) {
        this.each(function () {
        	$(this).click(function (e) {
        		e.preventDefault();
            	executeDelete($(this), deleteUrl, undoUrl);
            });
        });
    };
})(jQuery);



var Admin = {
    dateLoaded: new Date().getTime(),
    isDirty: false,
    byPassDirty: false,
    setDirty: function () {
        Admin.isDirty = true;
        $('#changesunsaved').show();
    },
    Updated: function () {
        $('#itemupdated').slideDown('fast', function () {
            setTimeout(function () { $('#itemupdated').fadeOut('fast'); }, 10000);
        });
    },
    Created: function () {
        $('#itemcreated').slideDown('fast', function () {
            setTimeout(function () { $('#itemcreated').fadeOut('fast'); }, 10000);
        });
    },
    Success: function() {
        $(".adminOverlayWrap").show();
    },
    CreateEditor: function (el) {
        var editor = CKEDITOR.replace(el.id, {
            customConfig: '/includes/ckeditor/configFramed.js',
            toolbar: el.dataset.toolbar || 'Imagemakers',
            stylesSet: (el.dataset.stylesset || "default") + ':/includes/ckeditor/styles.js?c=' + Admin.dateLoaded,
            bodyClass: el.dataset.bodyclass,
            width: el.dataset.width || 600
        });
        Admin.Editors.push(editor);
        editor.on('blur', function (e) {
            if (this.checkDirty())
                Admin.setDirty();
        });
        editor.on('focus', function (e) {
            if (this.plugins.autogrow == null)
                return;

            window.scrollTo(0, $(this.container.$).offset().top)
        })
    },
    Editors: [],
    HideMessages: function () {
        $('.message').fadeOut('fast');
    }
};

$(function() {
  $('.galleryImage img').each(function() {
    var image = $(this);
    var imageOrig = new Image();
    imageOrig.src = image.attr('src');
    var imageWidth = imageOrig.naturalWidth;
    var imageHeight = imageOrig.naturalHeight;
    if (imageWidth < imageHeight) {
      $(this).parent('.galleryImage').addClass('galleryImagePortrait');
    }
  });
  // Collapsing Right Column
  $('.adminRightColumnSectionCollapse').slideUp();
  $('.adminRightColumn h4').click(function() {
    $(this).toggleClass('expanded');
    $(this).next('.adminRightColumnSectionCollapse').slideToggle();
  });
});