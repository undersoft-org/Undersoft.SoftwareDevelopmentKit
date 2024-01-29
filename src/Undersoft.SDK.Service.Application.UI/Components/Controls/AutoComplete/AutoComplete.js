(function ($) {
    $.extend({
        bb_autoScrollItem: function (el, index) {
            var $el = $(el);
            var $menu = $el.find('.dropdown-menu');
            var maxHeight = parseInt($menu.css('max-height').replace('px', '')) / 2;
            var itemHeight = $menu.children('li:first').outerHeight();
            var height = itemHeight * index;
            var count = Math.floor(maxHeight / itemHeight);

            $menu.children().removeClass('active');
            var len = $menu.children().length;
            if (index < len) {
                $menu.children()[index].classList.add('active');
            }

            if (height > maxHeight) {
                $menu.scrollTop(itemHeight * (index > count ? index - count : index));
            }
            else if (index <= count) {
                $menu.scrollTop(0);
            }
        },
        bb_composition: function (el, obj, method) {
            var isInputZh = false;
            var $el = $(el);
            $el.on('compositionstart', function (e) {
                isInputZh = true;
            });
            $el.on('compositionend', function (e) {
                isInputZh = false;
            });
            $el.on('input', function (e) {
                if (isInputZh) {
                    e.stopPropagation();
                    e.preventDefault();
                    setTimeout(function () {
                        if (!isInputZh) {
                            obj.invokeMethodAsync(method, $el.val());
                        }
                    }, 15);
                }
            });
        },
        bb_setDebounce: function (el, waitMs) {
            var $el = $(el);
            let timer;
            var allowKeys = ['ArrowUp', 'ArrowDown', 'Escape', 'Enter'];

            $el.on('keyup', function (event) {
                if (allowKeys.indexOf(event.key) < 1 && timer) {
                    clearTimeout(timer);
                    event.stopPropagation();

                    timer = setTimeout(function () {
                        timer = null;
                        event.target.dispatchEvent(event.originalEvent);
                    }, waitMs);
                } else {
                    timer = setTimeout(function () { }, waitMs);
                }
            });
        }
    });
})(jQuery);
