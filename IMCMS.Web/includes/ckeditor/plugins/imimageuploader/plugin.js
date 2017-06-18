CKEDITOR.plugins.add('imimageuploader', {
	icons: 'imimageuploader',
	requires: [ 'iframedialog' ],
	init: function (editor) {
		var me = this;
		editor.addCommand('imImageUpload', 
			new CKEDITOR.dialogCommand('imImageUploaderDialog')
		);
		editor.on('doubleclick', function(evt) {
			var element = evt.data.element;

			if (element.is('img') && !element.data('cke-realelement') && !element.isReadOnly() && evt.data.dialog == null) {
				editor.execCommand('imImageUpload');
			}
		});

		editor.ui.addButton('imImageUploader', {
			label: 'Upload Image',
			command: 'imImageUpload',
			toolbar: 'insert'
		});
		
		var imMaxWidth = 800;
		var imMaxHeight = 600;
		var imMaxSize = 4096; //4MB

		//read parameters if defined. Otherwise use default
		var imageUploaderResources = CKEDITOR.plugins.get("imageUploaderResources");
		if (imageUploaderResources != null) {
			if (imageUploaderResources.maxWidth != null)
				imMaxWidth = imageUploaderResources.maxWidth;
			if (imageUploaderResources.maxHeight != null)
				imMaxHeight = imageUploaderResources.maxHeight;
			if (imageUploaderResources.maxSize != null)
				imMaxSize = imageUploaderResources.maxSize;
		}
		
		var iframeWindow = null;
		var currentElement = null;
		CKEDITOR.dialog.add('imImageUploaderDialog', function() {
			return {
				title: 'Image Uploader',
				minWidth: imMaxWidth,
				minHeight: imMaxHeight,
				resizable: CKEDITOR.DIALOG_RESIZE_NONE,
				contents:
					  [
						 {
						 	id: 'iframe',
						 	label: 'Insert an Image...',
						 	//title: '',
						 	expand: true,
						 	elements:
								  [
									 {
									 	type: 'iframe',
									 	src: '/ImageUploader/?maxWidth=' + imMaxWidth + '&maxHeight=' + imMaxHeight + "&maxSize=" + imMaxSize,
									 	width: '100%',
									 	height: '100%',
									 	onContentLoad: function () {
									 		// Iframe is loaded...
									 		//alert("Iframe is loaded..." + currentElement);
									 		var iframe = document.getElementById(this._.frameId);
									 		iframeWindow = iframe.contentWindow;
									 		$('.cke_dialog_page_contents').height("100%");  //workaorund to force height to 100%
									 		iframeWindow.imageUploader.dialogInstance = this;
									 		if (!this.getDialog().insertMode) {
									 			var filePath = currentElement.getAttribute("src");
									 			var alternativeText = currentElement.getAttribute("alt");
									 			var currentWidth = 0;
									 			if (currentElement.getAttribute("width") != null)
									 				currentWidth = parseFloat(currentElement.getAttribute("width").replace(/px/i, ""), 10);
									 			if (currentElement.getStyle("width") != null && currentElement.getStyle("width") != "" && !isNaN(currentElement.getStyle("width").replace(/px/i, "")))
									 				currentWidth = parseFloat(currentElement.getStyle("width").replace(/px/i, ""), 10);
									 			if (currentWidth == 0)
									 				currentWidth = $(currentElement.$).width();
									 			var currentHeight = 0;
									 			if (currentElement.getAttribute("height") != null)
									 				currentHeight = parseFloat(currentElement.getAttribute("height").replace(/px/i, ""), 10);
									 			if (currentElement.getStyle("height") != null && currentElement.getStyle("height") != "" && !isNaN(currentElement.getStyle("height").replace(/px/i, "")))
									 				currentHeight = parseFloat(currentElement.getStyle("height").replace(/px/i, ""), 10);
									 			if (currentHeight == 0)
									 				currentHeight = $(currentElement.$).height();
									 			if (currentWidth > imMaxWidth || currentHeight > imMaxHeight) {
									 				//Do not allow to oversize maximun dimmensions
									 				var nPercentW = (imMaxWidth / currentWidth);
									 				var nPercentH = (imMaxHeight / currentHeight);
									 				var nPercent = nPercentH < nPercentW ? nPercentH : nPercentW;
									 				//Replace new resize values to make image editable
									 				currentWidth = currentWidth * nPercent;
									 				currentHeight = currentHeight * nPercent;
									 			}
									 			var croppedFilePath = null;
									 			var originalWidth = currentWidth;
									 			var originalHeight = currentHeight;
									 			if (currentElement.data("filepath") != null && currentElement.data("filepath") != "") {
									 				filePath = currentElement.data("filepath");
									 			}
									 			if (currentElement.data("croppedfilepath") != null && currentElement.data("croppedfilepath") != "") {
									 				croppedFilePath = currentElement.data("croppedfilepath");;
									 			}
									 			if (currentElement.data("width") != null && currentElement.data("width") != "") {
									 				originalWidth = parseFloat(currentElement.data("width"), 10);
									 			}
									 			if (currentElement.data("height") != null && currentElement.data("height") != "") {
									 				originalHeight = parseFloat(currentElement.data("height"), 10);
									 			}
									 			var alignment = 0;
									 			if (currentElement.data("alignment") != null && currentElement.data("alignment") != "") {
									 				alignment = parseInt(currentElement.data("alignment"), 10);
									 			}
									 			iframeWindow.setupEditImage(filePath, croppedFilePath, alternativeText, currentWidth, currentHeight, originalWidth, originalHeight, alignment);
									 		} else {
									 			iframeWindow.setupUploadImage();
									 		}
									 	}
									 }
								  ]
						 }
					  ],
				onShow: function () {
					//alert("on show=" + iframeWindow);
					var selection = this._.editor.getSelection(),
					   element = selection.getStartElement();
					if (element) 
						element = element.getAscendant('img', true);

					if (!element || element.getName() != 'img' || element.data('cke-realelement')) {
						element = this._.editor.document.createElement('img');
						this.insertMode = true;

						//initial settings for dialog when upload
						this.getButton('ok').getElement().hide();
						this.getButton('cancel').getElement().hide();
						this.resize(300, 200);
						this.move(Math.max(0, (($(window).width() - 300) / 2) + $(window).scrollLeft()),
							Math.max(0, (($(window).height() - 200) / 2) + $(window).scrollTop()));
						this.layout();
					}
					else
						this.insertMode = false;

					currentElement = element;
				},
				onOk: function () {
					// Notify your iframe scripts here...
					//alert("Notify your iframe scripts here");
					iframeWindow.destroyPUploader();
					var filepath = iframeWindow.imageUploader.wasCropped ? iframeWindow.imageUploader.croppedFilePath : iframeWindow.imageUploader.filePath;
					var data = JSON.stringify({ filePath: filepath, width: iframeWindow.imageUploader.currentSize.width, height: iframeWindow.imageUploader.currentSize.height });
					var finalImageName = null;
					$.ajaxSetup({ async: false });
					$.ajax({
						url: "/ImageUploader/FinalProcessing",
						type: "post",
						data: data,
						contentType: "application/json",
						sucess: function (response) {
							if (response.ErrorMessage) {
								alert(response.ErrorMessage);
							} else if (response.finalImageName && response.finalImageName !== "") {
								finalImageName = response.finalImageName;
							}
						}
					});
					$.ajaxSetup({ async: true });
					this._.editor.insertHtml(iframeWindow.imageUploader.getImageHtml(finalImageName));
				},
				onCancel: function () {
					iframeWindow.destroyPUploader();
  					//remove uploaded image if existed
					var filepath = iframeWindow.imageUploader.filePath;
					if (filepath && filepath != "" && this.insertMode) {
						$.post("/ImageUploader/DeleteFile", { filePath: filepath }, function (response) {
							if (response.ErrorMessage) {
								alert(response.ErrorMessage);
							}
						});
					}

				}
			};
		});
	}
});