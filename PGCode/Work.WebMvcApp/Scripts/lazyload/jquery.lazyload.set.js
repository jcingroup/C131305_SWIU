$(function() {
          $("img").lazyload({
          	   placeholder : "../album/images/grey.gif",
          	   threshold :0,
          	   //failure_limit : 10,
              effect : "fadeIn"
          });          
      });