/**
 * Global variables
 */
"use strict";
(function () {
    var isNoviBuilder = window.xMode;
    var userAgent = navigator.userAgent.toLowerCase(),
        initialDate = new Date(),
        $document = $(document),
        $window = $(window),
        $html = $("html"),
        isDesktop = $html.hasClass("desktop"),
        isIE =
            userAgent.indexOf("msie") != -1
                ? parseInt(userAgent.split("msie")[1])
                : userAgent.indexOf("trident") != -1
                    ? 11
                    : userAgent.indexOf("edge") != -1
                        ? 12
                        : false,
        isMobile =
            /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(
                navigator.userAgent
            ),
        isSafari = !!navigator.userAgent.match(/Version\/[\d\.]+.*Safari/),
        plugins = {
            bootstrapTooltip: $("[data-toggle='tooltip']"),
            copyrightYear: $(".copyright-year"),
            rdNavbar: $(".rd-navbar"),
            materialParallax: $(".parallax-container"),
            rdMailForm: $(".rd-mailform"),
            rdInputLabel: $(".form-label"),
            regula: $("[data-constraints]"),
            owl: $(".owl-carousel"),
            isotope: $(".isotope"),
            radio: $("input[type='radio']"),
            checkbox: $("input[type='checkbox']"),
            counter: $(".counter"),
            pageLoader: $("#page-loader"),
            captcha: $(".recaptcha"),
            lightGallery: $('[data-lightgallery="group"]'),
            lightGalleryItem: $('[data-lightgallery="item"]'),
            lightDynamicGalleryItem: $('[data-lightgallery="dynamic"]'),
            bootstrapDateTimePicker: $("[data-time-picker]"),
            slick: $(".slick-slider"),
        };

    /**
     * Initialize All Scripts
     */
    $(function () {
        /**
         * @desc Initialize the gallery with set of images
         * @param {object} itemsToInit - jQuery object
         * @param {string} [addClass] - additional gallery class
         */
        function initLightGallery(itemsToInit, addClass) {
            if (!isNoviBuilder) {
                $(itemsToInit).lightGallery({
                    thumbnail: $(itemsToInit).attr("data-lg-thumbnail") !== "false",
                    selector: "[data-lightgallery='item']",
                    autoplay: $(itemsToInit).attr("data-lg-autoplay") === "true",
                    pause:
                        parseInt($(itemsToInit).attr("data-lg-autoplay-delay")) || 5000,
                    addClass: addClass,
                    mode: $(itemsToInit).attr("data-lg-animation") || "lg-slide",
                    loop: $(itemsToInit).attr("data-lg-loop") !== "false",
                });
            }
        }

        /**
         * Slick carousel
         * @description  Enable Slick carousel plugin
         */
        if (plugins.slick.length) {
            var i;
            for (i = 0; i < plugins.slick.length; i++) {
                var $slickItem = $(plugins.slick[i]);

                $slickItem.on("init", function (slick) {
                    initLightGallery(
                        $('[data-lightgallery="group-slick"]'),
                        "lightGallery-in-carousel"
                    );
                    initLightGallery(
                        $('[data-lightgallery="item-slick"]'),
                        "lightGallery-in-carousel"
                    );
                });

                $slickItem
                    .slick({
                        slidesToScroll:
                            parseInt($slickItem.attr("data-slide-to-scroll")) || 1,
                        asNavFor: $slickItem.attr("data-for") || false,
                        dots: $slickItem.attr("data-dots") == "true",
                        infinite: isNoviBuilder
                            ? false
                            : $slickItem.attr("data-loop") == "true",
                        focusOnSelect: false,
                        arrows: $slickItem.attr("data-arrows") == "true",
                        swipe: $slickItem.attr("data-swipe") == "true",
                        autoplay: isNoviBuilder
                            ? false
                            : $slickItem.attr("data-autoplay") == "true",
                        centerMode: $slickItem.attr("data-center-mode") == "true",
                        centerPadding: $slickItem.attr("data-center-padding")
                            ? $slickItem.attr("data-center-padding")
                            : "0.50",
                        mobileFirst: true,
                        responsive: [
                            {
                                breakpoint: 0,
                                settings: {
                                    slidesToShow: parseInt($slickItem.attr("data-items")) || 1,
                                    swipe: $slickItem.attr("data-swipe") || false,
                                },
                            },
                            {
                                breakpoint: 479,
                                settings: {
                                    slidesToShow: parseInt($slickItem.attr("data-xs-items")) || 1,
                                    swipe: $slickItem.attr("data-xs-swipe") || false,
                                },
                            },
                            {
                                breakpoint: 767,
                                settings: {
                                    slidesToShow: parseInt($slickItem.attr("data-sm-items")) || 1,
                                    swipe: $slickItem.attr("data-sm-swipe") || false,
                                },
                            },
                            {
                                breakpoint: 992,
                                settings: {
                                    slidesToShow: parseInt($slickItem.attr("data-md-items")) || 1,
                                    swipe: $slickItem.attr("data-md-swipe") || false,
                                },
                            },
                            {
                                breakpoint: 1200,
                                settings: {
                                    slidesToShow: parseInt($slickItem.attr("data-lg-items")) || 1,
                                    swipe: $slickItem.attr("data-lg-swipe") || false,
                                },
                            },
                        ],
                    })
                    .on("afterChange", function (event, slick, currentSlide, nextSlide) {
                        var $this = $(this),
                            childCarousel = $this.attr("data-child");

                        if (childCarousel) {
                            $(childCarousel + " .slick-slide").removeClass("slick-current");
                            $(childCarousel + " .slick-slide")
                                .eq(currentSlide)
                                .addClass("slick-current");
                        }
                    });
            }
        }

        $(".slick-style-1").on("click", ".slick-slide", function (e) {
            e.stopPropagation();
            var index = $(this).data("slick-index"),
                targetSlider = $(".slick-style-1");
            if (targetSlider.slick("slickCurrentSlide") !== index) {
                targetSlider.slick("slickGoTo", index);
            }
        });

    });
})();
