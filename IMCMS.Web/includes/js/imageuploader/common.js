//Class to wrap util functions
function CommonClass() {

}

if (!window.Common) {
	window.Common = new CommonClass();
}

CommonClass.prototype.ajaxSync = function (url, type, json, success, contentType) {
	$.ajaxSetup({ async: false });
	this.ajax(url, type, json, success, contentType);
	$.ajaxSetup({ async: true });
};

CommonClass.prototype.ajax = function (url, type, json, success, contentType) {
	contentType = contentType || 'application/x-www-form-urlencoded; charset=UTF-8';
	$.ajax({
		url: url,
		type: type,
		data: json,
		contentType: contentType,
		success: function (response) {
			if (response.Error)
				alert(response.Error);
			else if (response.Url)
				window.location.href = response.Url;
			else if (success)
				success(response);
		},
		error: function (x, s, e) {
			alert("Error on ajax call: " + x.statusText + "\nStatus code: " + x.status)
		}
	});
};
