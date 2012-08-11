define(['spaz/dom'], function ($) {

    return {
        tellUser: function (str) {
            $.withId('message').appendChild(document.createTextNode(str));
        }
    };

});
