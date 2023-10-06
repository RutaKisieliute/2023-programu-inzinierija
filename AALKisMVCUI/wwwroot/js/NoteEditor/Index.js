const miliBeforeSave = 1000;
//const miliBetweenFetches = 1000;
const webOrigin = window.location.protocol + "//" + window.location.host;
const controller = window.location.pathname.split('/')[1];
const category = window.location.pathname.split('/')[2];
const note = window.location.pathname.split('/')[3];

var spanTextArea = document.body.getElementsByClassName("textarea")[0];
var saveTimeoutId;

spanTextArea.setAttribute("oninput", "onTextAreaChange(this)")

//setInterval((fetchTextArea), miliBetweenFetches, spanTextArea)

spanTextArea.addEventListener("focusout", (event) => {
    saveTextArea(event.target);
});

function saveTextArea(spanTextArea)
{
    clearTimeout(saveTimeoutId);
    fetch(webOrigin + "/" + controller + "/PostNoteRecord/"
        + category + "/" + note,
        {
            "method": "POST",
            "body": JSON.stringify({ text: spanTextArea.innerHTML }),
            "headers": {"content-type": "application/json"}
        });
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
