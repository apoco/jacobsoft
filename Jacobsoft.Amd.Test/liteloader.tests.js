/// <reference path="../Jacobsoft.Amd/Scripts/liteloader.js" />

describe('liteloader', function () {
    it('provides a define function', function () {
        expect(typeof(define)).toBe('function');
    });

    it('provides a require function', function () {
        expect(typeof (require)).toBe('function');
    });
});

describe('the define function', function () {

    it('can register an object', function () {
        var obj = { foo: 'bar' };
        define('a', [], obj);
        require(['a'], function (a) {
            expect(a).toBe(obj);
        });
    });

    it('can register a factory', function () {
        var obj = { foo: 'bar' };
        define('b', [], function () { return obj; });
        require(['b'], function (b) {
            expect(b).toBe(obj);
        });
    });

    it('can declare dependencies', function () {
        define('c', ['d'], function (d) { return d.foo; });
        define('d', [   ], function ( ) { return { foo: 'bar' }; });

        require(['c'], function (c) {
            expect(c).toBe('bar');
        });
    });
});

describe('the require function', function () {
    
    it('can just execute a defined module', function () {
        var hasRun = false;

        define('e', [], function () { hasRun = true; });
        expect(hasRun).toBe(false);

        require(['e']);
        expect(hasRun).toBe(true);
    });

    it('can invoke a callback', function () {
        define('f', [], function () { return 'foo'; });
        require(['f'], function (f) { expect(f).toBe('foo'); });
    });

    it('resolves dependencies recursively', function () {
        define('g', ['h'], function (h) { return h.foo; });
        define('h', ['i'], function (i) { return { foo: i.bar }; });
        define('i', [], function () { return { bar: 'bar' }; });
        require(['g'], function (g) { expect(g).toBe('bar'); });
    });

    it('runs factories only once', function () {
        var object1, object2;
        define('j', [], function () { return { foo: 'bar' }; });
        require(['j'], function (j) { object1 = j; });
        require(['j'], function (j) { object2 = j; });
        expect(object1).toBe(object2);
    });

    it('waits for modules to be defined before executing callback', function () {
        var isCalled = false;

        require(['k', 'l'], function () { isCalled = true; });
        expect(isCalled).toBe(false);

        define('k', ['l'], function () { return 'foo'; });
        expect(isCalled).toBe(false);

        define('l', [], function () { return 'bar'; });
        expect(isCalled).toBe(true);
    });

    it('has a config function property', function () {
        expect(typeof (require).config).toBe('function');
    });
});
