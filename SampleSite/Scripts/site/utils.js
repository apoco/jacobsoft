define(function () {

    return {
        tellUser: function (str) {
            document.getElementById('message').appendChild(document.createTextNode(str));
        }
    };

});
