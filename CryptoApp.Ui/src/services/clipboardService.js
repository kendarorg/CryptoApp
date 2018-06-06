import errorsService from '@/services/errorsService'

export default {
	name:'clipBoardService',
	copyTextToClipboard:function(text){
		  if (!navigator.clipboard || true) {
		    this.fallbackCopyTextToClipboard(text);
		    return;
		  }
		  navigator.clipboard.writeText(text).then(function() {
		    console.log('Async: Copying to clipboard was successful!');
		  }, function(err) {
		    console.error('Async: Could not copy text: ', err);
		  });
	},
	fallbackCopyTextToClipboard:function(text) {
		console.log("fallback");
	  var textArea = document.createElement("textarea");
	  textArea.value = text;
	  document.body.appendChild(textArea);
	  textArea.focus();
	  textArea.select();

	  try {
	    var successful = document.execCommand('copy');
	    var msg = successful ? 'successful' : 'unsuccessful';
	    console.log('Fallback: Copying text command was ' + msg);
	  } catch (err) {
	    console.error('Fallback: Oops, unable to copy', err);
	  }

	  document.body.removeChild(textArea);
	}
}