/// <reference path="../../../../jquery-updating-will-break-marketo.js" />
/// <reference path="../../../../jquery.Jcrop.min.js" />
/// <reference path="../../../../jquery-ui-1.9.2.resize-drag.min.js" />

var ALIGN_LEFT = 1;
var ALIGN_CENTER = 2;
var ALIGN_RIGHT = 3;
var ALIGN_LEFT_FLOAT = 4;
var ALIGN_RIGHT_FLOAT = 5;

function ImageUploaderClass(maxWidth, maxHeight, minWidth, minHeight) {
	this.filePath = null;
	this.croppedFilePath = null;
	this.alternativeText = null;
	this.originalSize = { width: 0, height: 0 };
	this.currentSize = { width: 0, height: 0 };
	this.maxWidth = maxWidth;
	this.maxHeight = maxHeight;
	this.minWidth = minWidth;
	this.minHeight = minHeight;
	this.imageToEditId = "resizableImage";
	this.imageToCropId = "croppableImage";
	this.jcropApi = null;
	this.jcropX = 0;
	this.jcropY = 0;
	this.jcropX2 = 0;
	this.jcropY2 = 0;
	this.jcropW = 0;
	this.jcropH = 0;
	this.wasCropped = false;
	this.alignment = 0;

	this.$imageToEdit = null;
	this.$imageToCrop = null;

	this.initImage = function () {
		this.$imageToEdit = $("#" + this.imageToEditId);
		this.$imageToCrop = $("#" + this.imageToCropId);
	};

	this.setAlignment = function (alignment) {
		if (alignment == "left" || alignment == ALIGN_LEFT)
			this.alignment = ALIGN_LEFT;
		else if (alignment == "right" || alignment == ALIGN_RIGHT)
			this.alignment = ALIGN_RIGHT;
		else if (alignment == "center" || alignment == ALIGN_CENTER)
			this.alignment = ALIGN_CENTER;
		else if (alignment == "leftFloat" || alignment == ALIGN_LEFT_FLOAT)
			this.alignment = ALIGN_LEFT_FLOAT;
		else if (alignment == "rightFloat" || alignment == ALIGN_RIGHT_FLOAT)
			this.alignment = ALIGN_RIGHT_FLOAT;
		else
			this.alignment = 0;
	};
}

ImageUploaderClass.prototype.loadImage = function (filePath, croppedFilePath, alternativeText, width, height, originalWidth, originalHeight, alignment) {
	var me = this;
	
	me.initImage();
	if (me.$imageToEdit.length == 1) {
		me.filePath = filePath;
		me.croppedFilePath = croppedFilePath;
		me.wasCropped = (croppedFilePath != null && croppedFilePath != "");
		me.alternativeText = alternativeText;
		me.currentSize.width = width;
		me.currentSize.height = height;
		me.originalSize.width = originalWidth;
		me.originalSize.height = originalHeight;
		me.setAlignment(alignment);
		me.$imageToEdit.replaceWith($('<img id="' + me.imageToEditId + '" src="' + (me.wasCropped ? me.croppedFilePath : me.filePath) + '" style="height:' + me.currentSize.height + 'px; width:' + me.currentSize.width + 'px;" alt="' + me.alternativeText + '"/>'));
		me.$imageToCrop.replaceWith($('<img id="' + me.imageToCropId + '" src="' + me.filePath + '" style="height:' + me.originalSize.height + 'px; width:' + me.originalSize.width + 'px;" alt="' + me.alternativeText + '"/>'));
		me.$imageToEdit.show();
	} else {
		alert("image tag id='" + this.imageToEditId + "' does not exists to initialize image uploader.");
	}
};

ImageUploaderClass.prototype.resetToOriginal = function (animated) {
	var me = this;

	me.initImage();
	if (me.$imageToEdit.length == 1) {
		me.$imageToEdit.hide();
		me.$imageToEdit.attr("src", me.filePath);
		me.currentSize.height = me.originalSize.height;
		me.currentSize.width = me.originalSize.width;
		me.$imageToEdit.width(me.currentSize.width);
		me.$imageToEdit.height(me.currentSize.height);
		me.$imageToEdit.fadeIn(animated ? 300 : 0);
		me.wasCropped = false;
		me.croppedFilePath = null;
	}
};

ImageUploaderClass.prototype.initResize = function () {
	var me = this;

	me.initImage();
	if (me.$imageToEdit.length == 1) {
		me.$imageToEdit.resizable({
			//animate: true, //
			maxWidth: me.maxWidth,
			maxHeight: me.maxHeight,
			minHeight: me.minHeight,
			minWidth: me.minWidth,
			aspectRatio: true,
			stop: function (event, ui) {
				me.currentSize = ui.size;
			}
		});
	}
};

ImageUploaderClass.prototype.cancelResize = function () {
	var me = this;

	me.initImage();
	if (me.$imageToEdit.length == 1 && me.$imageToEdit.data("resizable")) {
		me.$imageToEdit.resizable("destroy");
	}
};

ImageUploaderClass.prototype.initCrop = function () {
	var me = this;

	me.initImage();
	var updateCoords = function (c) {
		me.jcropX = c.x;
		me.jcropY = c.y;
		me.jcropX2 = c.x2;
		me.jcropY2 = c.y2;
		me.jcropW = c.w;
		me.jcropH = c.h;
	};

	if (me.$imageToCrop.length == 1) {
		me.$imageToCrop.width(me.originalSize.width).height(me.originalSize.height);
		me.jcropApi = $.Jcrop(me.$imageToCrop[0], {
			onChange: updateCoords,
			onSelect: updateCoords,
			minSize: [me.minWidth, me.minHeight]
		});
		me.jcropApi.animateTo([0, 0, Math.round(me.originalSize.width / 2), Math.round(me.originalSize.height / 2)]);
	}
};

ImageUploaderClass.prototype.cancelCrop = function () {
	var me = this;

	me.initImage();
	if (me.jcropApi != null && me.$imageToCrop.length == 1 && me.$imageToCrop.data("Jcrop")) {
		me.jcropApi.release();
		me.jcropApi.cancel();
		me.jcropApi.destroy();
		me.jcropApi = null;
	}
};

ImageUploaderClass.prototype.performCrop = function (croppedFilePath, callback) {
	var me = this;

	me.initImage();
	if (me.$imageToCrop.length == 1 && me.$imageToEdit.length == 1) {
		me.wasCropped = true;
		me.croppedFilePath = croppedFilePath;
		me.currentSize.height = me.jcropH;
		me.currentSize.width = me.jcropW;
		me.$imageToEdit.hide();
		me.$imageToEdit.attr("src", me.croppedFilePath + "?" + Math.random());
		me.$imageToEdit.width(me.currentSize.width);
		me.$imageToEdit.height(me.currentSize.height);
		setTimeout(function () {
			me.$imageToEdit.fadeIn(300, function () {
				if (callback && typeof (callback) == "function")
					callback();
			});
		}, 600);
	}
};

ImageUploaderClass.prototype.getImageHtml = function (imgSrc) {
	var me = this;

	me.initImage();
	if (me.$imageToEdit.length == 1 && me.filePath != null && me.filePath != "") {
		//Builds image tag to be inserted on editor.
		if (imgSrc == null || imgSrc === "")
			imgSrc = me.wasCropped ? me.croppedFilePath : me.filePath;

		var style = ' width:' + me.currentSize.width + 'px; height:' + me.currentSize.height + 'px;';
		switch (me.alignment) {
			case ALIGN_LEFT:
				break;
			case ALIGN_RIGHT:
				style += " float: right;";
				break;
			case ALIGN_CENTER:
				style += " margin: 0 auto;";
				break;
			case ALIGN_LEFT_FLOAT:
				style += " float: left;";
				break;
			case ALIGN_RIGHT_FLOAT:
				style += " float: right;";
				break;
		}

		var imgTag = '<img src="{0}" style="display: block;{1}" alt="{2}" width="{3a}" height="{3b}" class="image_fix" data-filepath="{4}" data-croppedfilepath="{5}" data-width="{6}" data-height="{7}" data-alignment="{8}" align="{9}"/>';
		return imgTag
			.replace('{0}', imgSrc)
			.replace('{1}', style)
			.replace('{2}', $("#alternativeText").val())
			.replace('{3a}', me.currentSize.width)
			.replace('{3b}', me.currentSize.height)
			.replace('{4}', me.filePath)
			.replace('{5}', me.croppedFilePath == null ? "" : me.croppedFilePath)
			.replace('{6}', me.originalSize.width)
			.replace('{7}', me.originalSize.height)
			.replace('{8}', me.alignment)
			.replace('{9}', (me.alignment == ALIGN_RIGHT || me.alignment == ALIGN_RIGHT_FLOAT) ? "right" : "left");
	}
	return "";
};