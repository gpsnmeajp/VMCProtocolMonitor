"use strict";
var view = document.getElementById ("view");

if (!window.Worker) {
	alert("Web Worker disabled! Editor won't work!")
}var worker;

try {
	worker = new Worker("worker.js");
}catch (e) {
	addStat("Exception!(UI): "+e.message);
}

worker.onmessage = function(e) {
		view.innerHTML = e.data;
};

setInterval(function(){
	worker.postMessage("Doit");
}, 500);
