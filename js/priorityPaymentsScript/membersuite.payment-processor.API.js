$(document).ready(function () {
    $("input[id$='CardNumber']").attr('maxLength', 16);
    $("input[id$='CCV']").attr('maxLength', 4);
    $("input[id$='CardNumber'],input[id$='CCV']").bind('keypress', function (e) {
        var k = e.which || e.keyCode;
        var allow = [8, 9, 13, 27,36, 37, 38, 39, 40, 46,118];
        var sKey = String.fromCharCode(k);

        if (sKey !== "" && $.inArray(k, allow) < 0 && !sKey.match(/[0-9]/)) {
            e.preventDefault();
        }
    });
});


var membersuite = membersuite || {};
membersuite.ajaxAPI = membersuite.ajaxAPI || {};

membersuite.ajaxAPI = (function ($) {
    function persistToken(parms) {
        if (!!parms.$cardNumberElem) {
            parms.$cardNumberElem.val(parms.vaultToken).hide();
            $('.cc-number-masked').show().val('Not-A-CreditCard-Number');
        } else if (!!parms.$bankAccElement) {
            parms.$bankAccElement.val(parms.vaultToken).hide();
            if (parms.$bankAccElementConfirm.length > 0) {
                parms.$bankAccElementConfirm.val(parms.vaultToken).hide();
            }
            $('.bnk-number-masked').show().val('Not-A-BankAcc-Number');
            $('.bnk-rtn-masked').show().val('Not-A-BankRtn-Number');
        } else {
            parms.postback = undefined;
            parms.saveBtnId = undefined;
        }

        if (!!parms.postback) {
            eval(parms.postback);
        } else if (!!parms.saveBtnId) {
            $(parms.saveBtnId).trigger("click")
        }

        return true;
    };

    return {
        persistToken: persistToken
    }
})(jQuery);