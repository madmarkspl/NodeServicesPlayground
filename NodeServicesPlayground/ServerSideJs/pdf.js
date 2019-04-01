module.exports = function (callback, document) {

    var functionBegin = Date.now();
    console.log('FunctionBegin ' + functionBegin);
    var jsreport = require('jsreport-core')();

    try {
        var init = Date.now();
        jsreport.init().then(function () {
            console.log('Init ' + (Date.now() - init));

            var render = Date.now();
            return jsreport.render(
                {
                    template:
                    {
                        content: '{{:_content}}',
                        engine: 'jsrender',
                        recipe: 'phantom-pdf',
                        phantom: {
                            format: document.pageSize,
                            orientation: document.orientation,
                            margin: 0
                        }
                    },
                    data:
                    {
                        _content: document.content
                    }
                }).then(function(resp) {
                console.log('Render ' + (Date.now() - render));

                var d = resp.content.toJSON().data

                console.log('FunctionEnd ' + Date.now());

                callback(null, d);
            });
        }).catch(function (e) {
            callback(e, null);
        });
    } catch (e) {
        callback(null, null);
    }
};
