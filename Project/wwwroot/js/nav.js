$(function () {
    $(".menu-women, #menu-women").on("mouseenter", function () {
      $("#menu-women").stop().slideDown();
      $(".menu-women").css({
        "border-bottom": "1px solid #ddd",
      });
    });
    $(".menu-women, #menu-women").on("mouseleave", function () {
      $("#menu-women").stop().slideUp();
      $(".menu-women").css({
        "border-bottom": "none",
      });
    });
    $(".menu-men, #menu-men").on("mouseenter", function () {
      $("#menu-men").stop().slideDown();
      $(".menu-men").css({
        "border-bottom": "1px solid #ddd",
      });
    });
    $(".menu-men, #menu-men").on("mouseleave", function () {
      $("#menu-men").stop().slideUp();
      $(".menu-men").css({
        "border-bottom": "none",
      });
    });
    $(".menu-kids, #menu-kids").on("mouseenter", function () {
      $("#menu-kids").stop().slideDown();
      $(".menu-kids").css({
        "border-bottom": "1px solid #ddd",
      });
    });
    $(".menu-kids, #menu-kids").on("mouseleave", function () {
      $("#menu-kids").stop().slideUp();
      $(".menu-kids").css({
        "border-bottom": "none",
      });
    });
    $(".menu-style, #menu-style").on("mouseenter", function () {
        $("#menu-style").stop().slideDown();
        $(".menu-style").css({
            "border-bottom": "1px solid #ddd",
        });
    });

    $(".menu-style, #menu-style").on("mouseleave", function () {
        $("#menu-style").stop().slideUp();
        $(".menu-style").css({
            "border-bottom": "none",
        });
    });
    $(".owl-carousel").owlCarousel({
        loop: true,
        margin: 10,
        nav: false,
        responsive: {
            0: {
                items: 1,
            },
            600: {
                items: 3,
            },
            1000: {
                items: 5,
            },
        },
    });

    $("#tshirt").on("mouseenter", function () {
        $(this).css({
            transform: "scale(1.1)",
        });
    });
    $("#tshirt").on("mouseleave", function () {
        $(this).css({
            transform: "scale(1)",
        });
    });
    $("#longTshirt").on("mouseenter", function () {
        $(this).css({
            transform: "scale(1.1)",
        });
    });
    $("#longTshirt").on("mouseleave", function () {
        $(this).css({
            transform: "scale(1)",
        });
    });
    $("#shortPolo").on("mouseenter", function () {
        $(this).css({
            transform: "scale(1.1)",
        });
    });
    $("#shortPolo").on("mouseleave", function () {
        $(this).css({
            transform: "scale(1)",
        });
    });
    $("#sweatshirt").on("mouseenter", function () {
        $(this).css({
            transform: "scale(1.1)",
        });
    });
    $("#sweatshirt").on("mouseleave", function () {
        $(this).css({
            transform: "scale(1)",
        });
    });
    $("#shirt").on("mouseenter", function () {
        $(this).css({
            transform: "scale(1.1)",
        });
    });
    $("#shirt").on("mouseleave", function () {
        $(this).css({
            transform: "scale(1)",
        });
    });
    $("#coat").on("mouseenter", function () {
        $(this).css({
            transform: "scale(1.1)",
        });
    });
    $("#coat").on("mouseleave", function () {
        $(this).css({
            transform: "scale(1)",
        });
    });
});