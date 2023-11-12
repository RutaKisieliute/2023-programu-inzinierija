const miliBeforeSave = 500;
const miliBeforeStatusClear = 2000;
//const miliBetweenFetches = 1000;
const webOrigin = window.location.protocol + "//" + window.location.host;
const controller = window.location.pathname.split('/')[1];
const note = window.location.pathname.split('/')[2];

const editorTextArea = document.getElementById("editor-textarea");
var spanEditHTML;
var caretPositionSpanEditHTML = 0;
var shouldUpdateSpanViewHTML;
var spanViewHTML;

const titleTextArea = document.getElementById("title-textarea");
var oldTitle;

const statusDiv = document.getElementById("editor-status");
var saveContentsTimeoutId;
var statusClearTimeoutId;

function startup()
{
    spanEditHTML = editorTextArea.innerHTML;

    shouldUpdateSpanViewHTML = true;

    editorTextArea.innerHTML = spanEditHTML;

    //setInterval((fetchTextArea), miliBetweenFetches, editorTextArea)
    //
    document.addEventListener("paste", (onPaste));

    editorTextArea.addEventListener("input", (onEditorInput));
    editorTextArea.addEventListener("focus", (onEditorFocus));
    editorTextArea.addEventListener("click", (onEditorClick));
    editorTextArea.addEventListener("dblclick", (onEditorDoubleClick));
    editorTextArea.addEventListener("focusout", (onEditorFocusOut));
    onEditorFocusOut();

    titleTextArea.addEventListener("focus", (onTitleFocus));
    titleTextArea.addEventListener("focusout", (onTitleFocusOut));

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
    if(!targetQuery)
    {
        return false;
    }

    let target = document.querySelector(targetQuery)
    if(!target)
    {
        return false;
    }
    target.scrollIntoView({ "behavior": "smooth" });

    return false;
}

function showSuccessStatus()
{
    clearTimeout(statusClearTimeoutId);
    statusDiv.innerHTML = "Saved successfully."
    statusClearTimeoutId = setTimeout(() => { statusDiv.innerHTML = "" }, miliBeforeStatusClear);
}

function showFailureStatus()
{
    clearTimeout(statusClearTimeoutId);
    statusDiv.innerHTML = "<b>!!FAILED TO SAVE!!</b>"
    statusClearTimeoutId = setTimeout(() => { statusDiv.innerHTML = "" }, miliBeforeStatusClear);
}


function saveContents()
{
    // Executed a save; clear the save timer
    clearTimeout(saveContentsTimeoutId);

    fetch(webOrigin + "/" + controller + "/PostNote/"
        + note,
        {
            "method": "POST",
            "body": JSON.stringify({ "Content": spanEditHTML}),
            "headers": {"content-type": "application/json"}
        }).then((response) => { if(!response.ok) showFailureStatus(); else showSuccessStatus(); },
            (showFailureStatus));
}

//async function fetchTextArea(editorTextArea)
//{
//    let result = await fetch(webOrigin + "/" + controller + "/GetNote/"
//        + folder + "/" + note);
//}

function onEditorInput(event)
{
    // Clear the timer to save
    clearTimeout(saveContentsTimeoutId);

    spanEditHTML = editorTextArea.innerHTML;

    shouldUpdateSpanViewHTML = true;

    // Set the timer to save
    saveContentsTimeoutId = setTimeout((saveContents), miliBeforeSave);
}

function onPaste(event)
{
    // https://stackoverflow.com/a/34876744
    event.preventDefault();

    var text = '';

    if (event.clipboardData || event.originalEvent.clipboardData)
    {
        text = (event.originalEvent || event).clipboardData.getData('text/plain');
    }
    else if (window.clipboardData)
    {
        text = window.clipboardData.getData('Text');
    }

    // `execCommand` is obsolete/deprecated,
    // but there are no alternatives as I've read,
    // so it `execCommand` stays.
    if (document.queryCommandSupported('insertText'))
    {
        document.execCommand('insertText', false, text);
    }
    else
    {
        document.execCommand('paste', false, text);
    }
}

function onEditorFocus(event)
{
    if(!editorTextArea.childNodes[0])
    {
        return;
    }

    // Show editing HTML
    editorTextArea.innerHTML = spanEditHTML;

    // Set the caret position
    // https://stackoverflow.com/a/6249440
    var range = document.createRange();
    var selection = window.getSelection();

    // `setStart` works by offset if node.nodeType == 3 (Text),
    // so `editorTextArea` can't be passed directly,
    // but the first child node of it is a Text node.
    range.setStart(editorTextArea.childNodes[0], caretPositionSpanEditHTML);
    range.setEnd(editorTextArea.childNodes[0], caretPositionSpanEditHTML);
    range.collapse(true);

    selection.removeAllRanges();
    selection.addRange(range);
}

function onEditorClick(event)
{
    // Save the caret position
    try
    {
        var selection = window.getSelection().getRangeAt(0);
        caretPositionSpanEditHTML = selection.startOffset;
    }
    catch (error)
    {
        console.warn(error);
    }
}

function onEditorDoubleClick(event)
{
    // Allow editing on double click
    editorTextArea.setAttribute("contentEditable", "true");
    editorTextArea.focus({ "focusVisible": "true" });
}

function onEditorFocusOut(event)
{
    // Disable editing after losing focus
    // Can't click hyperlinks when content is editable
    editorTextArea.setAttribute("contentEditable", "false");
    // Early return
    if(!shouldUpdateSpanViewHTML)
    {
        editorTextArea.innerHTML = spanViewHTML;
        return;
    }

    // Updating the span view means we don't do it again unless we need to
    shouldUpdateSpanViewHTML = false;

    // We only use regular expressions
    // So we don't need to use editorTextArea.innerHTML
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
        editorTextArea.innerHTML = spanViewHTML;
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
    editorTextArea.innerHTML = spanViewHTML;
}

function onTitleFocus(event)
{
    if(!oldTitle)
        oldTitle = titleTextArea.innerHTML;
}

async function onTitleFocusOut(event)
{
    var newTitle = titleTextArea.innerHTML
    var response;
    try
    {
        response = await fetch(webOrigin + "/" + controller + "/PostNote/" + note,
            {
                "method": "POST",
                "body": JSON.stringify({ "Title": newTitle }),
                "headers": {"content-type": "application/json"}
            });
        if(!response.ok)
            throw new Error();
    }
    catch(exception)
    {
        titleTextArea.innerHTML = oldTitle;
        showFailureStatus();
        return;
    }

    oldTitle = null;

    newTitle = await response.text();
    showSuccessStatus();
}

startup();
