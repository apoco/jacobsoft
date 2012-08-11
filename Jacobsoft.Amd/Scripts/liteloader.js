(function (exports) {

    var modules = {};
    var callbacks = {};

    var config = function (configuration) {
    };

    var define = function(id, deps, factory) {
        modules[id] = { deps: deps, factory: factory, exports: undefined };
        if (callbacks[id]) {
            for (var i = 0; i < callbacks[id].length; i++) {
                callbacks[id][i]();
            }
            delete callbacks[id];
        }
    }

    var resolve = function (module, callback) {
        if (module.exports === undefined) {
            module.exports = null;
            module.exports = require(module.deps, module.factory, callback);
        }
        return module.exports;
    };

    var require = function (deps, factory, callback) {

        callback = callback || function () { require(deps, factory); };

        var args = [];
        for (var i = 0; i < deps.length; i++) {
            var depId = deps[i];

            var module = modules[depId];
            if (!module) {
                callbacks[depId] = callbacks[depId] || [];
                callbacks[depId].push(callback);
                return;
            }

            var moduleExport = resolve(module, callback);
            if (moduleExport === undefined) {
                return;
            }

            args.push(moduleExport);
        }

        if (factory) {
            return (typeof (factory) == 'function')
                ? factory.apply(factory, args)
                : factory;
        }
    };

    exports.require = require;
    exports.require.config = config;
    exports.define = define;

})(window);
