(function () {
    'use strict';
    function ready() {
        var root = document.querySelector('[data-mth-slider]');
        if (!root) return;
        var slides = Array.prototype.slice.call(root.querySelectorAll('[data-mth-slide]'));
        var prev = document.querySelector('[data-mth-prev]');
        var next = document.querySelector('[data-mth-next]');
        var dots = Array.prototype.slice.call(document.querySelectorAll('[data-mth-dot]'));
        if (slides.length < 2) return;
        var current = 0;
        var timer = null;
        var delay = 5000;
        var reduceMotion = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;
        function switchTo(idx) {
            current = (idx + slides.length) % slides.length;
            slides.forEach(function (slide, i) {
                var active = i === current;
                slide.classList.toggle('is-active', active);
                slide.setAttribute('aria-hidden', active ? 'false' : 'true');
                var links = slide.querySelectorAll('a, button, input');
                for (var j = 0; j < links.length; j++) {
                    if (active) links[j].removeAttribute('tabindex');
                    else links[j].setAttribute('tabindex', '-1');
                }
            });
            dots.forEach(function (dot, i) {
                var active = i === current;
                dot.classList.toggle('is-active', active);
                if (active) dot.setAttribute('aria-current', 'true');
                else dot.removeAttribute('aria-current');
            });
        }
        function stop() { if (timer) { window.clearInterval(timer); timer = null; } }
        function play() {
            stop();
            if (!reduceMotion && !document.hidden) timer = window.setInterval(function () { switchTo(current + 1); }, delay);
        }
        function goTo(idx) { switchTo(idx); play(); }
        if (prev) prev.addEventListener('click', function () { goTo(current - 1); });
        if (next) next.addEventListener('click', function () { goTo(current + 1); });
        dots.forEach(function (dot, i) { dot.addEventListener('click', function () { goTo(i); }); });
        root.addEventListener('mouseenter', stop);
        root.addEventListener('mouseleave', play);
        root.addEventListener('focusin', stop);
        root.addEventListener('focusout', function (e) { if (!root.contains(e.relatedTarget)) play(); });
        document.addEventListener('visibilitychange', function () { if (document.hidden) stop(); else play(); });
        switchTo(0);
        play();
    }
    if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', ready);
    else ready();
})();
