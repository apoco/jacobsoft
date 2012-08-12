define(
    ['site/utils', 'scumtech/rainbow', 'options'],
    function (utils, rainbow, options) {
        utils.tellUser("Hello, " + options.name + "!");
        rainbow.rainbowify(document.getElementById('message'));
    }
);
