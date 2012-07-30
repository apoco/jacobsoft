(function (exports) {

    circularRef = {};

    modules = {};

    function define(name, deps, factory) {
        modules[name] = { deps: deps, factory: factory, exports: undefined };
    }

    function require(deps, factory) {
        var args = [];
        for (var i = 0; i < deps.length; i++) {
            var module = modules[deps[i]];
            args.push(module ? resolve(module) : undefined);
        }
        if (factory) {
            return (typeof (factory) == 'function')
                ? factory.apply(factory, args)
                : factory;
        }
    }

    function resolve(module) {
        if (module.exports === undefined) {
            module.exports = null;
            module.exports = require(module.deps, module.factory);
        }
        return module.exports;
    }

    exports.define = define;
    exports.require = require;

})(window);
