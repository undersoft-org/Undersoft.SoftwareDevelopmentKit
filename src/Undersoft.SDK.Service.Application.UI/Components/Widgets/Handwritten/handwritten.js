(function ($) {
    $.extend({
        bb_handwritten: function (el, obj, autoStop, method) {
            document.body.addEventListener('touchmove', function (e) {
                e.preventDefault(); 
            }, { passive: false });  

            new lineCanvas({
                el: el.getElementsByClassName("hw-body")[0], 
                clearEl: el.getElementsByClassName('btn-secondary')[0], 
                saveEl: el.getElementsByClassName('btn-primary')[0], 
                obj: obj
            });

            function lineCanvas(obj) {
                this.linewidth = 1;
                this.color = "#000000";
                this.background = "#fff";
                for (var i in obj) {
                    this[i] = obj[i];
                };
                this.canvas = document.createElement("canvas");
                this.el.appendChild(this.canvas);
                this.cxt = this.canvas.getContext("2d");
                this.canvas.clientTop = this.el.clientWidth;
                this.canvas.width = this.el.clientWidth;
                this.canvas.height = this.el.clientHeight;

                this.cxt.fillStyle = this.background;
                this.cxt.fillRect(2, 2, this.canvas.width, this.canvas.height);

                this.cxt.fillStyle = this.background;
                this.cxt.strokeStyle = this.color;
                this.cxt.lineWidth = this.linewidth;
                this.cxt.lineCap = "round";

                var isSupportTouch = 'ontouchend' in window;
                var that = this;
                var isStart = false;

                var hw_star = function (e) {
                    isStart = true;
                    this.cxt.beginPath();
                    var parentLeft = e.target.offsetParent.offsetLeft;
                    var parentTop = e.target.offsetParent.offsetTop;
                    if (isSupportTouch) {
                        this.cxt.moveTo(e.changedTouches[0].pageX + 2 - parentLeft, e.changedTouches[0].pageY + 2 - parentTop);
                    }
                    else {
                        this.cxt.moveTo(e.pageX + 2 - parentLeft, e.pageY + 2 - parentTop);
                    }
                };
                if (isSupportTouch) {
                    this.canvas.addEventListener("touchstart", hw_star.bind(this), false);
                }
                else {
                    this.canvas.addEventListener('mousedown', hw_star.bind(this), false);
                }

                var hw_move = function (e) {
                    if (isStart) {
                        var parentLeft = e.target.offsetParent.offsetLeft;
                        var parentTop = e.target.offsetParent.offsetTop;
                        if (isSupportTouch) {
                            this.cxt.lineTo(e.changedTouches[0].pageX + 2 - parentLeft, e.changedTouches[0].pageY + 2 - parentTop);
                        }
                        else {
                            this.cxt.lineTo(e.pageX + 2 - parentLeft, e.pageY + 2 - parentTop);
                        }
                        this.cxt.stroke();
                    }
                };
                if (isSupportTouch) {
                    this.canvas.addEventListener("touchmove", hw_move.bind(this), false);
                }
                else {
                    this.canvas.addEventListener('mousedown', hw_move.bind(this), false);
                }

                var hw_end = function () {
                    isStart = false;
                    this.cxt.closePath();
                };
                if (isSupportTouch) {
                    this.canvas.addEventListener("touchend", hw_end.bind(this), false);
                }
                else {
                    this.canvas.addEventListener('mousedown', hw_end.bind(this), false);
                }

                this.clearEl.addEventListener("click", function () {
                    this.cxt.clearRect(2, 2, this.canvas.width, this.canvas.height);
                }.bind(this), false);
                this.saveEl.addEventListener("click", function () {
                    var imgBase64 = this.canvas.toDataURL();
                    return this.obj.invokeMethodAsync(method, imgBase64);
                }.bind(this), false);
            };
        }
    });
})(jQuery);
