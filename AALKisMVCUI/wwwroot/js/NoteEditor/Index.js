const miliBeforeSave = 1000;
const miliBeforeStatusClear = 2000;
//const miliBetweenFetches = 1000;
const webOrigin = window.location.protocol + "//" + window.location.host;
const controller = window.location.pathname.split('/')[1];
const category = window.location.pathname.split('/')[2];
const note = window.location.pathname.split('/')[3];

const spanTextArea = document.getElementById("editor-textarea");
var spanEditHTML;
var shouldUpdateSpanViewHTML;
var spanViewHTML;

const statusDiv = document.getElementById("editor-status");
var saveTimeoutId;
var statusClearTimeoutId;

function startup()
{
    spanEditHTML = spanTextArea.innerHTML.replaceAll("\n", "<br>");

    shouldUpdateSpanViewHTML = true;

    spanTextArea.innerHTML = spanEditHTML;

    //setInterval((fetchTextArea), miliBetweenFetches, spanTextArea)

    spanTextArea.addEventListener("input", (onInput));
    spanTextArea.addEventListener("focus", (onFocus));
    spanTextArea.addEventListener("dblclick", (onDoubleClick));
    spanTextArea.addEventListener("focusout", (onFocusOut));

    onFocusOut();
    scrollToHref();

    /*(event) => {
        saveTextArea(event.target);
    });*/
}

const getKeywordId = (keyword) => { return "keyword-" + keyword; };

// Helper function to scroll to element
function scrollToHref(anchorTag)
{
    let targetQuery = (anchorTag ? anchorTag.href : window.location.href).match(/#[A-z-_]+/);
    if(targetQuery)
    {
        document.querySelector(targetQuery).scrollIntoView({ "behavior": "smooth" });
    }

    return false;
}


function saveTextArea()
{
    // Executed a save; clear the save timer
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
            "body": JSON.stringify({ "text": spanEditHTML }),
            "headers": {"content-type": "application/json"}
        }).then((success), (failure));
}

//async function fetchTextArea(spanTextArea)
//{
//    let result = await fetch(webOrigin + "/" + controller + "/GetNoteRecord/"
//        + category + "/" + note);
//}


function onInput(event)
{
    // Clear the timer to save
    clearTimeout(saveTimeoutId);

    // Update the HTML
    if(spanTextArea.innerHTML == '<br>')
    {
        spanTextArea.innerHTML = '';
    }

    spanEditHTML = spanTextArea.innerHTML;

    shouldUpdateSpanViewHTML = true;

    // Set the timer to save
    saveTimeoutId = setTimeout((saveTextArea), miliBeforeSave);
}

function onFocus()
{
    spanTextArea.innerHTML = spanEditHTML;
}

function onDoubleClick()
{
    // Allow editing on double click
    spanTextArea.setAttribute("contentEditable", "true");

    spanTextArea.focus({ "focusVisible": "true" });
}

function onFocusOut()
{
    // Disable editing after losing focus
    // Can't click hyperlinks when content is editable
    spanTextArea.setAttribute("contentEditable", "false");
    // Early return
    if(!shouldUpdateSpanViewHTML)
    {
        spanTextArea.innerHTML = spanViewHTML;
        return;
    }

    // Updating the span view means we don't do it again unless we need to
    shouldUpdateSpanViewHTML = false;

    // We only use regular expressions
    // So we don't need to use spanTextArea.innerHTML
    spanViewHTML = spanEditHTML;

    var keywords = new Map();

    // Capture all keyword definitions
    spanViewHTML = spanViewHTML
        .replaceAll(/#(\w+)/gi, (...match) => {
            let keyword = match[1].toLowerCase();

            keywords.set(keyword, getKeywordId(keyword));

            return "<b id=\"" + keywords.get(keyword) + "\">"
                + match[0]
                + "</b>";
        });

    // Don't process if there are no keywords
    if(keywords.size == 0)
    {
        spanTextArea.innerHTML = spanViewHTML;
        return;
    }

    // Helper function to convert an iterator into an array
    // (FireFox lacks one)
    function iteratorToArray(iterator)
    {
        let result = [];
        for(let element = iterator.next().value;
                element;
                element = iterator.next().value)
        {
            result.push(element);
        }
        return result;
    }


    // Create a regular expression which captures all uses of keywords
    // Without their definitions
    var targetRegExp = new RegExp("([-#]|\\w+)*"
            + "("
                + "(" + iteratorToArray(keywords.keys()).join(")|(") + ")"
            + ")"
            + "(\\w+)*",
        "gi");

    // Capture all used keywords
    spanViewHTML = spanViewHTML
        .replaceAll(targetRegExp, (...match) => {
            if(match[0] != match[2])
            {
                return match[0];
            }
            return "<a href=\"#" + keywords.get(match[0].toLowerCase()) + "\" onclick=\"scrollToHref(this)\">"
                + match[0]
                + "</a>";
        });

    // Finally update the innerHTML
    spanTextArea.innerHTML = spanViewHTML;

}

startup();
