var memerSuitePortal = memerSuitePortal || {};
memerSuitePortal.CreateAccountBasicInfoViewModel = function() {
    var self = this;

    self.preventNewContinueDoubleClick = function(btn) {
        $(btn).prop('disabled', true);
        return true;
    };

    self.init = function() {
        $('.btn-new-continue').removeAttr('disabled');
    };
};


$(function () {
    memerSuitePortal.createAccountBasicInfo = new memerSuitePortal.CreateAccountBasicInfoViewModel();
    memerSuitePortal.createAccountBasicInfo.init();
});