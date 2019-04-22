var filter = function(text, length, tail) {
  // Create new div element
  var node = document.createElement("div");
  // Add the text to the newly created div element
  node.innerHTML = text;
  // Get the text content to be potentially modified
  var content = node.textContent;
  // If the content length is too long slice it and append a tail to the text
  // Else, just return the content unmodified
  return content.length > length ? content.slice(0, length) + tail : content;
};

export { filter };
