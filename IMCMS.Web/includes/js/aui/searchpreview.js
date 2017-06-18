var SearchPreview = function (title, path, $meta, $title) {
    if ($('div.searchPreview').length > 0) {
        var $searchPreviewTitle = $('.searchPreview .searchTitle');
        var $searchPreviewURL = $('.searchPreview .searchURL');
        var $searchPreviewMeta = $('.searchPreview .searchDesc');

        var updateNewUrl = function (newUrl) {
            $searchPreviewURL.text(newUrl);
        };
        var url = getAbsoluteUrl(path);
        var updatePreview = function () {
            var newTitle = ($title != null ? $title.val().trim() + ' - ' : '') + title;
            $searchPreviewTitle.text(newTitle);
            $searchPreviewMeta.text($meta.val());
            var newUrl = url;
            if ($title != null) {
                if ($title.data('slug') !== '') {
                    newUrl = newUrl + $title.data('slug');
                    $title.data('slug', '');
                } else {
                    $.post('/SiteAdmin/Slug?title=' + $title.val().trim(), function (e) {
                        newUrl = newUrl + e.slug;
                        updateNewUrl(newUrl);
                    });
                }
            }
            updateNewUrl(newUrl);
        };


        updatePreview();

        $meta.bind('change', function () {
            updatePreview();
        });

        if ($title != null) {
            $title.bind('change', function () {
                updatePreview();
            });
        }
    }
};

function getAbsoluteUrl(relativeUrl) {
    var basePath = location.protocol + '//' + location.hostname + (typeof (location.port) !== 'undefined' && location.port != '' && location.port != '80' ? ':' + location.port : '');
    return basePath + relativeUrl;
}