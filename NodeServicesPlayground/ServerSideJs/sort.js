var lodash = require('lodash');

module.exports =
    (callback, data) => {
        console.log(data);
        console.log(data.array);

        var result = lodash.sortBy(data.array);
        callback(null, result);
    };