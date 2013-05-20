
var doNotFire = false;
var isInitializing = false;
function prepareChildDropDown(parentDropDown, childDropDown, childShadowDropDown) {

    // First, let's copy all of the child drop down values
    var parent = document.getElementById(parentDropDown);
    var child = document.getElementById(childDropDown);
    var childShadow = document.getElementById(childShadowDropDown);

    var ddl = document.getElementById('ctl00_body_PageLayoutType');
    var originalValue = null;

    if (child.selectedIndex > 0)
        originalValue = child.options[child.selectedIndex].value;
    for (i = child.options.length - 1; i >= 0; i--) {

        var option = child.options[i];
        childShadow.options[i] = new Option(option.text, option.value);
        child.remove(i);

    }

    // now update the dropdown based on the value
    updateCascadingDropDown(parent);

    // now, can we find the value?
    if (originalValue != null)
        for (i = 0; i < child.options.length; i++)
            if (originalValue == child.options[i].value) {
                child.options[i].selected = true;

                if (child.onchange)
                    child.onchange();
                break;
            }


}



function hideDropDown(dropDownToHide) {


    dropDownToHide.style.display = 'none';

}

function disableDropDown(dropDownToHide, disableText) {


    dropDownToHide.options[0] = new Option(disableText, '');
    dropDownToHide.selectedIndex = 0;
    dropDownToHide.disabled = true;
}

function hideElement(elementToHide) {


    elementToHide.style.display = 'none';


}
function updateCascadingDropDownWithValues(parentDropDown, childDropDown, childShadowDropDown, valuesToShow, showAllListed, whenNoChildValues, whenNoChildValuesDisableText, elementToHide) {


    var parent = document.getElementById(parentDropDown);
    var child = document.getElementById(childDropDown);
    var childShadow = document.getElementById(childShadowDropDown);

    // first, show the child
    child.style.display = '';
    child.disabled = false;

    // show the element, if possible
    var elementToHide = document.getElementById(elementToHide);

    if (elementToHide != null)
        elementToHide.style.display = '';

    // clear the child
    for (i = child.options.length - 1; i >= 0; i--)
        child.remove(i);

    // now, let's figure move the shadow options

    var childCount = 0;
    if (showAllListed) {
        for (i = 0; i < valuesToShow.length; i++) {
            var indexToShow = valuesToShow[i];
            if (indexToShow < 0) // always skip
                continue;


            var shadowOption = childShadow.options[valuesToShow[i]];

            if (shadowOption != null) {
                var newOption = new Option(shadowOption.text, shadowOption.value);

                child.options[childCount] = newOption;
                childCount++;
            }

        }
    }
    else    // we should show all that are exluded
    {
        for (i = 0; i < childShadow.options.length; i++) {

            var dontShowValue = false;
            for (j = 0; j < valuesToShow.length; j++)
                if (valuesToShow[j] == i) {
                    dontShowValue = true;
                    break;
                }

            if (dontShowValue)
                continue;

            var shadowOption = childShadow.options[i];
            var newOption = new Option(shadowOption.text, shadowOption.value);
            child.options[childCount] = newOption;
            childCount++;
        }
    }




    if (!isInitializing && child.onchange)
        child.onchange();

    if (childCount == 0)  // there are no child values
    {
        switch (whenNoChildValues) {
            case 'DisableChildDropDown':
                disableDropDown(child, whenNoChildValuesDisableText);
                break;

            case 'HideChildDropDown':
                hideDropDown(child);
                break;

            case 'HideElement':

                hideElement(elementToHide);
                break;
        }

    }




}