XMLHttpRequest.prototype.open = (function (open) {
    return function (method, url, async) {
        open.apply(this, arguments);
        var token = localStorage.getItem("token");
        if (token != null && token != "") {
            this.setRequestHeader('Authtoken', token);
        }
        
    };
})(XMLHttpRequest.prototype.open);