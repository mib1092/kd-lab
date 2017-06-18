CKEDITOR.plugins.addExternal('imimagepaste', '/includes/ckeditor/plugins/imimagepaste/');
CKEDITOR.plugins.addExternal('imimageuploader', '/includes/ckeditor/plugins/imimageuploader/');
CKEDITOR.plugins.addExternal('iframedialog', '/includes/ckeditor/plugins/iframedialog/');

CKEDITOR.editorConfig = function (config) {

    config.width = 699;
    config.height = 400;
	
    config.forcePasteAsPlainText = true;
    config.allowedContent = true;

    config.toolbar = 'Imagemakers';
    config.extraPlugins = 'imimageuploader,justify,imimagepaste';
    config.removePlugins = 'image';

    config.contentsCss = '/includes/css/site.css';

    config.stylesSet = 'default:/includes/ckeditor/styles.js';
    config.resize_dir = 'both';

    config.filebrowserUploadUrl = '/SiteAdmin/FileUpload/QuickUpload';

    config.toolbar_Imagemakers =
    [
	    { name: 'document', items: ['Source'/*, '-', 'Save'*/] },
	    { name: 'clipboard', items: ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo'] },
	    /*{ name: 'editing', items: ['Find', 'Replace', '-', 'SelectAll', '-', 'SpellChecker'] },*/
	    '/',
	    { name: 'basicstyles', items: ['Bold', 'Italic', 'Strike', 'Subscript', 'Superscript', '-', 'RemoveFormat'] }, // 'Underline'
	    { name: 'paragraph', items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'] },
	    { name: 'links', items: ['Link', 'Unlink', 'Anchor'] },
	    { name: 'insert', items: ['imImageUploader', 'Table', 'HorizontalRule', 'SpecialChar'] },
	    '/',
	    { name: 'styles', items: ['Styles'] },
	    { name: 'tools', items: ['Maximize', /*'ShowBlocks',*/ '-', 'About'] }
    ];


    config.toolbar_basic =
        [
            { name: 'document', items: ['Source'/*, '-', 'Save'*/] },
            { name: 'clipboard', items: ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo'] },
            { name: 'basicstyles', items: ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-', 'RemoveFormat'] },
            { name: 'paragraph', items: ['NumberedList', 'BulletedList'] },
            /*{ name: 'editing', items: ['Find', 'Replace', '-', 'SelectAll', '-', 'SpellChecker'] },*/
            '/',
            { name: 'tools', items: ['Maximize', /*'ShowBlocks',*/ '-', 'About'] }
        ];

    config.toolbar_email =
    [
        { name: 'document', items: ['Source'/*, '-', 'Save'*/] },
	    { name: 'clipboard', items: ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo'] },
	    { name: 'tools', items: ['Maximize', /*'ShowBlocks',*/ '-', 'About'] },
	    /*{ name: 'editing', items: ['Find', 'Replace', '-', 'SelectAll', '-', 'SpellChecker'] },*/
	    '/',
	    { name: 'basicstyles', items: ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-', 'RemoveFormat'] },
	    { name: 'paragraph', items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'] },
	    { name: 'links', items: ['Link', 'Unlink'] },
	    { name: 'insert', items: ['HorizontalRule', 'SpecialChar'] },
    ];
};
