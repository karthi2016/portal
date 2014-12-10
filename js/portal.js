function onDropDownOpening(sender, eventArgs) {

    return; // don't wait for the first letter
    eventArgs.set_cancel(!sender.allowOpenDropDown);
}

function onItemsRequesting(sender) {

    return; // don't wait for the first letter
    if (!sender.get_dropDownVisible()) {
        sender.allowOpenDropDown = true;
        sender.showDropDown();
        sender.allowOpenDropDown = false;
    }
}

function GetRadComboBoxValue(comboBoxId) {
    if (comboBoxId == null) return;
    var cb = $find(comboBoxId);
    if (cb == null)
        return;

    return cb.get_value();
}

function TimeoutTick(dtSessionExpiration, hasSession) {
    if (!hasSession)
        return;

    var dtNow = new Date();
    var timeLeft = dtSessionExpiration - dtNow;
    var secondsLeft = Math.floor(timeLeft / 1000);
    var timeLeftSpan = document.getElementById("secondsLeft");

    timeLeftSpan.innerHTML = secondsLeft;

    if (secondsLeft < 90)
        ShowThisRadWindow();

    if (secondsLeft > 0)
        setTimeout('TimeoutTick(new Date(' + dtSessionExpiration.getTime() + '), ' + hasSession + ')', 1000);
    else
        CloseThisRadWindow();
}

function GetThisRadWindow()
{
var oWindow = null;
if (window.radWindow) oWindow = window.radWindow;

else if (window.frameElement.radWindow)
 oWindow = window.frameElement.radWindow; 

return oWindow;
}

function HideThisRadWindow() {
    //get a reference to the RadWindow
    var oWnd = GetThisRadWindow();
    //close the RadWindow
    oWnd.moveTo(-10000, -10000);
}

function CloseThisRadWindow() {
    //get a reference to the RadWindow
    var oWnd = GetThisRadWindow();
    //close the RadWindow
    oWnd.close();
}

function ShowThisRadWindow() {
    //get a reference to the RadWindow
    var oWnd = GetThisRadWindow();
    //close the RadWindow
    oWnd.center();
}

function OnTimeoutWindowClose() {
    window.location.href = "/Login.aspx?logout=true";
}


function setElementDisplay(id, display) {
    var o = document.getElementById(id);

    if (o != null) {
        o.style.display = (display == true) ? '' : 'none';
    }

    return false;
}


function FileUploadCoordinator_ChooseADifferentFile(FileUploadCoordinator) {

    setElementDisplay(FileUploadCoordinator + '_spanView', false);
    setElementDisplay(FileUploadCoordinator + '_spanUpload', true);
    var hiddenState = document.getElementById(FileUploadCoordinator + '_state');

    // set the status
    hiddenState.value = 'NEW';



}

function FileUploadCoordinator_CancelUpload(FileUploadCoordinator) {

    setElementDisplay(FileUploadCoordinator + '_spanView', true);
    setElementDisplay(FileUploadCoordinator + '_spanUpload', false);
    var hiddenState = document.getElementById(FileUploadCoordinator + '_state');

    // setElementDisplay the state
    hiddenState.value = 'NONE';



}

function GetItemsIfNeeded(sender, eventArgs) {
    if (sender.get_items()._array.length <= 1)
        sender.requestItems(sender.get_text(), false);
}  

function JumpToExternalLink(tokenUrl, nextUrl, newWindow) {
    var newForm = jQuery('<form>', {
        'action': '/JumpToExternal.aspx',
        'target': newWindow ? '_blank' : '_top'
    }).append(jQuery('<input>', {
        'name': 'TargetUrl',
        'value': tokenUrl,
        'type': 'hidden'
    })).append(jQuery('<input>', {
        'name': 'NextUrl',
        'value': nextUrl,
        'type': 'hidden'
    }));

    newForm.appendTo('body').submit();
}