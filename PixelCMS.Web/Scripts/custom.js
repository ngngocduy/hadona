setTimeout(function () { $("#alert-cart").removeClass("in"); }, 1000);

function IsEmail(email) {
    var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    return regex.test(email);
}

// Add wishlist
function AddToWishList(id) {
    $.post("/Common/Addwishlist", { "id": id },
    function (data) {
        if (data.Code == '2') {
            alert(data.Msg);
        }
        if (data.Code == '0') {
            var url = "/Common/UpdateWishListCount";
            $(".wishlistcount").load(url);
            $("#alert-cart").html("<p>Thêm sản phẩm vào danh sách yêu thích thành công</p>").fadeIn("fast").delay(1500).fadeOut("fast");
        }
    });
    return false;
}

function RemoveFromWishList(id) {
    if (confirm('Bạn muốn xóa sản phẩm này khỏi danh sách yêu thích?')) {
        $.post("/Common/RemoveFromWishList", { "id": id },
        function (data) {
            if (data.Code == '0') {
                var url = "/Common/UpdateWishlist";
                $(".shopping-cart-zone").load(url);
                $(".wishlistcount").load("/Common/UpdateWishListCount");
                $("#alert-cart").html("<p>Sản phẩm đã xóa. </p>").fadeIn("fast").delay(1500).fadeOut("fast");
            }
        });
    }
    return false;
}
// Add to Cart
function AddToCart(id, Attribute) {
    var s = "";
    var bool = true;
    //required dropdown
    $("#GroupATT select").each(function (parameters) {
        var val = $(this).val();
        var name = $(this).attr('name');
        if (val == "") {
            s = s + " (" + name + ")";
            bool=false;
        }
    });
    if (s != "") {
        $("#alert-cart").css('background', '#EB4578').html("<p style='color:white'>Vui lòng chọn" + s + "</p>").fadeIn("fast").delay(2000).fadeOut("fast");
        return false;
    }
    //required radiobutom
    $('#GroupATT .Group input[type=radio]').each(function () {
        var name = $(this).attr('name');
        var groupid = $(this).attr('groupid');
        if (validateRadio($('#GroupATT input[Groupid=' + groupid + ']input[type=radio]'))) {
            bool = true;
            return true;
        }
        else {
            $("#alert-cart").css('background', '#EB4578').html("<p style='color:white'>Vui lòng chọn(" + name + ")</p>").fadeIn("fast").delay(2000).fadeOut("fast");
            bool = false;
            return false;
        }
    });
    //ko có required thực hiện submit
    if (bool == true) {
        $.post("/Common/AddToCart", { "id": id, "Attribute": Attribute },
       function (data) {
           if (data.Code == '2') {
               alert(data.Msg);
           }
           if (data.Code == '0') {
              // window.location = "/vi/ShoppingCart";
               var url = "/Common/UpdateCartCount";
               $(".cartcount").load(url);
               $("#alert-cart").css('background', '#d9edf7').html("<p>Thêm sản phẩm vào giỏ hàng thành công.</p>").fadeIn("fast").delay(3000).fadeOut("fast");
           }
       });
        return false;
    }
}
//validate radio
function validateRadio(radios) {
    for (var i = 0; i < radios.length; ++i) {
        if (radios[i].checked) return true;
    }
    return false;
}


function RemoveFromCart(id, Attribute) {
    if (confirm('Bạn muốn xóa sản phẩm này khỏi giỏ hàng?')) {
        $.post("/Common/RemoveFromCart", { "id": id, "Attribute": Attribute },
        function (data) {
            if (data.Code == '0') {
                var url = "/Common/UpdateCart";
                $(".shopping-cart-zone1").load(url);
                $(".cartcount").load("/Common/UpdateCartCount");
                $("#alert-cart").html("<strong>Sản phẩm đã xóa khỏi giỏ hàng.</strong>").fadeIn("fast").delay(3000).fadeOut("fast");
            }
        });
    }
    return false;
}
function UpdateQuantity(id, quantity, Attribute) {
    $.post("/Common/UpdateQuantity", { "id": id, "quantity": quantity, "Attribute": Attribute },
            function (data) {
                if (data.Code == '2') {
                    alert(data.Msg);
                }
                if (data.Code == '0') {
                    // window.location.reload();
                    var url = "/Common/UpdateCart?DistrictId=" + $("#DistrictId").val();
                    $(".shopping-cart-zone1").load(url);
                    $(".cartcount").load("/Common/UpdateCartCount");
                    
                    var urltotal = "/Common/UpdateTotal?DistrictId=" + $("#DistrictId").val();
                    $("#update-total").load(urltotal);
                    $("#alert-cart").html("<p>Cập nhật số lượng sản phẩm thành công.</p>").fadeIn("fast").delay(1500).fadeOut("fast");
                }
            });
    return false;
}

$("#checkout-button").click(function () {
    $("#checkout-zone").fadeIn("slow");
});

//function to style checkbox and radio buttons
function inputs_skin() {
    $('.checkbox_skin').each(function () {
        $this = $(this);
        var checkbox_child = $this.children('input').css({ opacity: 0 });

        if ($(checkbox_child).is(':checked')) {
            $(this).css({ backgroundPosition: 'left bottom' });
        } else {
            $(checkbox_child).css({ backgroundPosition: 'left top' });
        }
        $(checkbox_child).change(function () {
            if ($(this).attr("checked")) {
                $(this).parent().css({ backgroundPosition: 'left bottom' });
                return;
            } else {
                $(this).parent().css({ backgroundPosition: 'left top' });
            }
        });
    });

    $('.radio_skin').each(function () {
        var buttons = $(this).find(':radio').css({ opacity: 0 });
        $(buttons).bind('click', function () {
            current_name = $(this).attr('name');
            radios_set = $(":radio[name=" + current_name + "]");
            if ($(radios_set).is(':checked')) {
                $(buttons).parent().css({ backgroundPosition: 'left bottom' });
            }
            $(radios_set).not(':checked').parent().css({ backgroundPosition: 'left top' });
        });
    });

    $('.select_skin select').each(function () {
        var the_select = $(this);
        the_span = $(this).parent().find('.selected_value');
        the_select.css('opacity', 0);
        the_span.text(the_select.find('option:selected').text());
    });

    $('.select_skin select').live('change', function () {
        var the_select = $(this);
        the_span = $(this).parent().find('.selected_value');
        the_span.text(the_select.find('option:selected').text());
    });
}


//function  increase/ decrease product qunatity buttons +/-
function change_qty(container) {
    var container = container;
    var input = $(container).find('input:text');
    default_val = $(input).val();
    j = default_val;
    remove = $(container).find('span.minus_qty');
    add = $(container).find('span.plus_qty');

    add.on('click', function () {
        j++;
        $(input).val(j);
    });

    remove.on('click', function () {
        if (j > 1) {
            j--;
            $(input).val(j);
        } else {
            j = 1;
        }
    });

    input.blur(function () {
        if ($(this).val() <= 0) {
            j = 1;
            $(this).val(1);
        } else {
            j = $(this).val();
        }
    });
}
// format số javascript---------------------------------------------

/**
 * Number.prototype.format(n, x, s, c)
 * 
 * @param integer n: length of decimal
 * @param integer x: length of whole part
 * @param mixed   s: sections delimiter
 * @param mixed   c: decimal delimiter
 */
Number.prototype.format = function (n, x, s, c) {
    var re = '\\d(?=(\\d{' + (x || 3) + '})+' + (n > 0 ? '\\D' : '$') + ')',
        num = this.toFixed(Math.max(0, ~~n));

    return (c ? num.replace('.', c) : num).replace(new RegExp(re, 'g'), '$&' + (s || ','));
};

//demo
/*var numbers = [1, 12, 123, 1234, 12345, 123456, 1234567, 12345.67, 123456.789];

document.write('<p>Classic Format:</p>');
for (var i = 0, len = numbers.length; i < len; i++) {
    document.write('R$ ' + numbers[i].format(2, 3, '.', ',') + '<br />');
}*/

//End format số javascript---------------------------------------------




//back to top
    $(document).ready(function () {
        $(window).scroll(function () {
            if ($(this).scrollTop() > 100) {
                $('#control').addClass("control-fix");
            } else {
                $('#control').removeClass("control-fix");
            }
        });
    });

    $(document).ready(function () {

        // hide #back-top first
        $("#back-top").hide();

        // fade in #back-top
        $(function () {
            $(window).scroll(function () {
                if ($(this).scrollTop() > 100) {
                    $('#back-top').fadeIn();
                } else {
                    $('#back-top').fadeOut();
                }
            });

            // scroll body to 0px on click
            $('#back-top a').click(function () {
                $('body,html').animate({
                    scrollTop: 0
                }, 800);
                return false;
            });
        });

    });


