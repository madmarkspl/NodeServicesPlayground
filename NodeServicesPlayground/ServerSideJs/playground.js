var playgroundLibrary = require('');

module.exports =
    (callback) => {
        var result = playgroundLibrary.doSth();
        callback(null, result);
    };