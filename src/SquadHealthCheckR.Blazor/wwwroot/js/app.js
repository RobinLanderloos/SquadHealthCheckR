// Create a function that sets the "data-theme" attribute to a string on the html element
function setTheme(theme) {
    document.documentElement.setAttribute("data-theme", theme);
}

function getTheme(){
    return document.documentElement.getAttribute("data-theme");
}