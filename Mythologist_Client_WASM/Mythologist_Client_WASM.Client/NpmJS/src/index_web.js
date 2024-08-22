//Used to delete the loading div. See Routes.razor
window.removeElementById = function (id) {
    var element = document.getElementById(id);
    element.parentNode.removeChild(element);
}

window.getElementWidth = function (elementId) {
    var element = document.getElementById(elementId);
    if (element) {
        return element.offsetWidth;
    }
    return 0;
}
