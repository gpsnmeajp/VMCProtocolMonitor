"use strict";
onmessage = function(e) {
	var xhr = new XMLHttpRequest();
	xhr.open("GET" , "/list.dat", false);//同期Request
	xhr.setRequestHeader("If-Modified-Since", "Thu, 01 Jan 1970 00:00:00 GMT");
	xhr.timeout = 1000;
	try {
		xhr.send();
	}catch (e) {
		postMessage(e.message);
	}
	
	//---return stat---
	if(xhr.readyState != 4)
	{
		postMessage("load failed.");
		return -1;
	}
	if(xhr.status == 0){
		postMessage("internal Error (EMPTY RESPONSE / CONNECTION REFUSED / etc...)");
		return -1;
	}
	postMessage(xhr.responseText);
}	