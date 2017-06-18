
// External preloader for every page

//use the following to set the array
//var imageArray = ['/images/mainnav/home-over.gif','/images/mainnav/news-over.gif'];

function IM_preloadImages() { //v3.0
  if(arguments[0] instanceof Array) {
    return IM_preloadImages.apply(null, arguments[0]);
  }
  var d=document;
  if(d.images){ 
    if(!d.MM_p) {
      d.MM_p=new Array();
    }
    var i,j=d.MM_p.length,a=IM_preloadImages.arguments;
    for(i=0; i<a.length; i++) {
      if (a[i].indexOf("#")!=0){
        d.MM_p[j]=new Image; d.MM_p[j++].src=a[i];
      }
    }
  }
}
  
//If using the array, call the function with the array  
//IM_preloadImages(imageArray);


function TrackTiming(category, variable, opt_label) {
  this.category = category;
  this.variable = variable;
  this.label = opt_label ? opt_label : undefined;
  this.startTime;
  this.endTime;
  return this;
}

TrackTiming.prototype.startTime = function () {
  this.startTime = new Date().getTime();
  return this;
}

TrackTiming.prototype.endTime = function () {
  this.endTime = new Date().getTime();
  return this;
}

TrackTiming.prototype.send = function () {
  var timeSpent = this.endTime - this.startTime;
  window._gaq.push(['_trackTiming', this.category, this.variable, timeSpent, this.label, 50]);
  return this;
}

// Browser Height
function browserHeight() {
  var browserHeightValue = $(window).height();
  $('.browserHeight').css({'height': browserHeightValue});
}
$(window).on("load resize", function() {
  browserHeight();
});