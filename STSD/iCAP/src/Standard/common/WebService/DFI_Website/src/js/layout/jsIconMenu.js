(function ($) {
    // Main function
    $.contactButtons = function (options) {
        // Define the defaults
        var defaults = {
            effect: 'slide-on-scroll', // slide-on-scroll
            // buttons: {
            //     'facebook': {
            //         class: 'facebook',
            //         use: false,
            //         icon: 'camera ',
            //         link: '',
            //         title: 'Screen Shot',
            //         idI: 'Icon-screenshot'
            //     },
            //     // 'google': {
            //     //     class: 'gplus',
            //     //     use: false,
            //     //     icon: 'google-plus',
            //     //     link: '',
            //     //     title: 'Visit on Google Plus'
            //     // }
            // }
        };

        // Merge defaults and options
        var s,
            settings = options;
        for (s in defaults.buttons) {
            if (options.buttons[s]) {
                settings.buttons[s] = $.extend(defaults.buttons[s], options.buttons[s]);
            }
        }

        // Define the container for the buttons
        var oContainer = $("#contact-buttons-bar");

        // Check if the container is already on the page
        if (oContainer.length === 0) {

            // Insert the container element
            $('body').append('<div id="contact-buttons-bar">');

            // Get the just inserted element
            oContainer = $("#contact-buttons-bar");

            // Add class for effect
            oContainer.addClass(settings.effect);

            // Add show/hide button
            // var sShowHideBtn = '<button class="contact-button-link show-hide-contact-bar"><span class="fa fa-angle-left"></span></button>';
            // oContainer.append(sShowHideBtn);

            var i;
            for (i in settings.buttons) {
                var bs = settings.buttons[i],
                    sLink = bs.link,
                    active = bs.use;

                // Check if element is active
                if (active) {

                    // Change the link for phone and email when needed
                    if (bs.type === 'phone') {
                        sLink = 'tel:' + bs.link;
                    } else if (bs.type === 'email') {
                        sLink = 'mailto:' + bs.link;
                    }

                    // Insert the links
                    var sIcon = '<span class="fa fa-' + bs.icon + '" id="' + bs.idI + '"></span>',
                        sButton = '<a  id="' + bs.id + '" class="contact-button-link cb-ancor ' + bs.class + '" ' +
                            (bs.title ? 'title="' + bs.title + '"' : '') +
                            (bs.extras ? bs.extras : '') +
                            '>' + sIcon + '</a>';
                    oContainer.append(sButton);
                }
            }

            // Make the buttons visible
            setTimeout(function () {
                oContainer.animate({
                    right: 0
                });
            }, 200);

            // Show/hide buttons
            $('body').on('click', '.show-hide-contact-bar', function (e) {
                e.preventDefault();
                e.stopImmediatePropagation();
                $('.show-hide-contact-bar').find('.fa').toggleClass('fa-angle-right fa-angle-left');
                oContainer.find('.cb-ancor').toggleClass('cb-hidden');
            });
        }
    };

    // Slide on scroll effect
    $(function () {

        // Define element to slide
        var el = $("#contact-buttons-bar.slide-on-scroll");

        // Load top default
        el.attr('data-top', el.css('top'));

        // Listen to scroll
        $(window).scroll(function () {
            clearTimeout($.data(this, "scrollCheck"));
            $.data(this, "scrollCheck", setTimeout(function () {
                var nTop = $(window).scrollTop() + parseInt(el.attr('data-top'));
                el.animate({
                    top: nTop
                }, 500);
            }, 250));
        });
    });

}(jQuery));

$.contactButtons({
    effect: 'slide-on-scroll',
    // buttons: {
    //     'facebook': {
    //         class: 'facebook',
    //         use: true,
    //         id: 'a-screenshot'
    //         // link: 'http://',
    //         // extras: 'target="_blank"'
    //     },
    //     // 'line': {
    //     //     class: 'line',
    //     //     use: true,
    //     //     icon: 'comments-o',
    //     //     link: ''
    //     // }
    // }
});