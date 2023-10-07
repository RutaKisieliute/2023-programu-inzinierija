const miliBeforeSave = 1000;
const miliBeforeStatusClear = 2000;
//const miliBetweenFetches = 1000;
const webOrigin = window.location.protocol + "//" + window.location.host;
const controller = window.location.pathname.split('/')[1];
const category = window.location.pathname.split('/')[2];
const note = window.location.pathname.split('/')[3];

var spanTextArea = document.getElementById("editor-textarea");
var statusDiv = document.getElementById("editor-status");
var saveTimeoutId;
var statusClearTimeoutId;

spanTextArea.setAttribute("oninput", "onTextAreaChange(this)")

//setInterval((fetchTextArea), miliBetweenFetches, spanTextArea)

spanTextArea.addEventListener("focusout", (event) => {
    saveTextArea(event.target);
});

function saveTextArea(spanTextArea)
{
    clearTimeout(saveTimeoutId);

    function success()
    {
        clearTimeout(statusClearTimeoutId);
        statusDiv.innerHTML = "Saved successfully."
        statusClearTimeoutId = setTimeout(() => { statusDiv.innerHTML = "" }, miliBeforeStatusClear);
    }

    function failure()
    {
        clearTimeout(statusClearTimeoutId);
        statusDiv.innerHTML = "!!FAILED TO SAVE!!"
        statusClearTimeoutId = setTimeout(() => { statusDiv.innerHTML = "" }, miliBeforeStatusClear);
    }

    fetch(webOrigin + "/" + controller + "/PostNoteRecord/"
        + category + "/" + note,
        {
            "method": "POST",
            "body": JSON.stringify({ text: spanTextArea.innerHTML }),
            "headers": {"content-type": "application/json"}
        }).then((success), (failure));
}

//async function fetchTextArea(spanTextArea)
//{
//    let result = await fetch(webOrigin + "/" + controller + "/GetNoteRecord/"
//        + category + "/" + note);
//}


function onTextAreaChange(spanTextArea)
{
    clearTimeout(saveTimeoutId);
    if(spanTextArea.innerHTML == '<br>')
    {
        spanTextArea.innerHTML = '';
    }
    saveTimeoutId = setTimeout((saveTextArea), miliBeforeSave, spanTextArea);
}
