window.scumtech = window.scumtech || {};

var colors = ['red', 'orange', 'yellow', 'green', 'blue', 'purple'];
var numColors = colors.length;

scumtech.rainbow = {
    rainbowify: function (elem) {

        var text = elem.innerText;
        scumtech.clear(elem);

        for (var i = 0; i < text.length; i++) {
            var letter = text.charAt(i);

            var span = document.createElement('span');
            span.setAttribute('style', 'color: ' + colors[i % numColors]);
            span.appendChild(document.createTextNode(letter));

            elem.appendChild(span);
        }
    }
};
