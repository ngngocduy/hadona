$(document).ready(function () {
    $("#NVKD").attr('disabled', 'disabled');
    $("#Cat").attr('disabled', 'disabled');
    //input value to form submit
    $("#btn_export").submit(function (e) {
        if ($("#selectdatetype").val() == "chidinh") {
            if ($("#tungay").val() == "" || $("#denngay").val() == "") {
                $(".inline").show("fast");
                e.preventDefault();
            }
        }
        if ($("#selectexporttype").val() == "doanhthusaler") {
            if ($("#NVKD").val() == "") {
                $(".help-inline1").show("fast");
                e.preventDefault();
            }
        }
        $("#gettungay").val($("#tungay").val());
        $("#getdenngay").val($("#denngay").val());

        $("#getexporttype").val($("#selectexporttype").val());
        $("#getproducttype").val($("#Cat").val());

        $("#getdatetype").val($("#selectdatetype").val());
        $("#getsalername").val($("#NVKD").val());
        $("#getcuahang").val($("#RolesList").val());

    });
    $("#selectdatetype").change(function () {
        if ($(this).val() != "chidinh") {
            $(".inline").css("display","none");
            $("#tungay").attr('disabled', 'disabled');
            $("#denngay").attr('disabled', 'disabled');
            $("#tungay").val("");
            $("#denngay").val("");
            $(".tungay").hide();
            $(".denngay").hide();
        }
        else {
            $("#tungay").removeAttr('disabled');
            $("#denngay").removeAttr('disabled');
            $(".tungay").show();
            $(".denngay").show();
        }

    });
    $("#selectexporttype").change(function () {
        if ($(this).val() == "doanhthusaler") {
            $("#NVKD").removeAttr('disabled');
            $("#cuahang_block").hide();
            $("#producttype_block").hide();
            $("#Cat").attr('disabled', 'disabled');
            $("#getexporttype").val("doanhthusaler");
            $("#salername_block").show();
            $(".tungay").show();
            $(".denngay").show();
            $("#selectdatetype").show();
        }
        else {
            $("#NVKD").attr('disabled', 'disabled');

            if ($(this).val() == "baocaohangton") {
                $("#producttype_block").hide();
                $("#cuahang_block").hide();
                $("#salername_block").hide();
                $("#Cat").attr('disabled', 'disabled');
                $("#getexporttype").val("baocaohangton");
                $("#selectdatetype").hide();
                $(".tungay").hide();
                $(".denngay").hide();
                $("#selectdatetype").val("hangnam");//set cai nay cho khoi bi e.prentdefault
            }
            if ($(this).val() == "baocaohangnhap") {
                $("#producttype_block").hide();
                $("#salername_block").hide();
                $("#Cat").attr('disabled', 'disabled');
                $("#getexporttype").val("baocaohangnhap");
                $(".tungay").show();
                $(".denngay").show();
                $("#selectdatetype").show();
            }
            if ($(this).val() == "mathang") {
                $("#producttype_block").show();
                $("#getexporttype").val("mathang");
                $("#cuahang_block").hide();
                $("#salername_block").hide();
                $("#Cat").removeAttr('disabled');
                $("#getproducttype").val($("#Cat").val());
                $(".tungay").show();
                $(".denngay").show();
                $("#selectdatetype").show();
            }
            if ($(this).val() == "totalrevenue") {
                $("#producttype_block").hide();
                $("#cuahang_block").hide();
                $("#salername_block").hide();
                $("#Cat").attr('disabled', 'disabled');
                $("#getexporttype").val("totalrevenue");
                $(".tungay").show();
                $(".denngay").show();
                $("#selectdatetype").show();
            }
            if ($(this).val() == "cuahang") {
                $("#cuahang_block").show();
                $("#producttype_block").hide();
                $("#salername_block").hide();
                $("#Cat").attr('disabled', 'disabled');
                $("#getexporttype").val("cuahang");
                $(".tungay").show();
                $(".denngay").show();
                $("#selectdatetype").show();
            }
        }
    });
});

