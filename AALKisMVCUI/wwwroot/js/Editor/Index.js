const miliBeforeSave = 1000;
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

const tagList = document.getElementById("tags");

var tags = tagList.querySelectorAll(".tag");

const tagsTextArea = document.getElementById("tags-textarea");
var tagText;

const tagsAddButton = document.getElementById("add-button");

function startup()
{
    editorTextArea.innerHTML = editorTextArea.innerHTML.replaceAll("\n", "<br>");
    titleTextArea.innerHTML = titleTextArea.innerHTML.replaceAll("\n", "<br>");

    spanEditHTML = editorTextArea.innerHTML;

    shouldUpdateSpanViewHTML = true;

    editorTextArea.innerHTML = spanEditHTML;

    tagsAddButton.addEventListener("click", (onAddButtonClick));

    tagsTextArea.addEventListener("focus", (onTagsFocus));

    setTagListeners(tags);

    //setInterval((fetchTextArea), miliBetweenFetches, editorTextArea)
    //
    editorTextArea.addEventListener("paste", (onPaste));

    editorTextArea.addEventListener("input", (onEditorInput));
    editorTextArea.addEventListener("focus", (onEditorFocus));
    editorTextArea.addEventListener("click", (onEditorClick));
    editorTextArea.addEventListener("dblclick", (onEditorDoubleClick));
    editorTextArea.addEventListener("focusout", (onEditorFocusOut));
    editorTextArea.addEventListener("onmouseleave", (saveContents));
    onEditorFocusOut();

    titleTextArea.addEventListener("focus", (onTitleFocus));
    titleTextArea.addEventListener("focusout", (onTitleFocusOut));

    scrollToHref();

    /*(event) => {
        saveTextArea(event.target);
    });*/
}

const getKeywordId = (keyword) => { return "keyword!" + keyword; };

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
            "body": JSON.stringify({ "Content": spanEditHTML.replaceAll("<br>", "\n") }),
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

    console.log(event);

    //editorTextArea.innerHTML = editorTextArea.innerHTML.replaceAll("<br>", "\n");

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
    // so `execCommand` stays.
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

    // Commented out because of mmd
    
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

function parseAndMarkKeywords(text)
{
    var keywords = new Map();

    // Capture all keyword definitions
    return text.replaceAll(/\${0,1}([A-z]+)/gi, (...match) => {
        if(match[0][0] == '$')
        {
            //console.log(match);
            //console.log("matched as keyword");
            let keyword = match[1].toLowerCase();

            keywords.set(keyword, getKeywordId(keyword));

            return "<div class=\"keyword\" id=\"" + keywords.get(keyword) + "\">"
                + match[0]
                + "</div>";
        }
        var keyword = keywords.get(match[0].toLowerCase());
        if(!keyword)
        {
            return match[0];
        }
        //console.log(match);
        //console.log("matched as used keyword");
        return "<a href=\"#" + keyword + "\" onclick=\"scrollToHref(this)\">"
            + match[0]
            + "</a>";
    });
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

    // Prepare for updating
    spanViewHTML = DOMPurify.sanitize(spanEditHTML);//new Option(spanEditHTML).innerHTML;

    spanViewHTML = parseAndMarkKeywords(spanViewHTML);
    spanViewHTML = marked.parse(spanViewHTML);

    if(spanViewHTML == "<p></p>")
    {
        spanViewHTML = ""
    }

    //spanViewHTML.replaceAll("&amp;", "&");

    // Finally port the changes to innerHTML
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
                "body": JSON.stringify({ "Title": newTitle.replaceAll("<br>", "\n") }),
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

function onAddButtonClick()
{
    if(tagsTextArea.innerHTML != "add more tags" && tagsTextArea.innerHTML != "")
    {
        fetch(webOrigin + "/" + controller + "/PostNote/"
            + note,
            {
                "method": "POST",
                "body": JSON.stringify({ "Tags": ["++" + tagsTextArea.innerHTML]}),
                "headers": {"content-type": "application/json"}
            }).then((response) => { if(!response.ok) showFailureStatus(); else showSuccessStatus(); },
                (showFailureStatus));
        tagList.innerHTML += "<kbd class=\"tag\" oncontextmenu=\"return false;\">" + tagsTextArea.innerHTML + "</kbd>";
        tags = tagList.querySelectorAll(".tag");
        setTagListeners(tags);
        tagsTextArea.innerHTML = "";
        //location.reload();
    }
}

function onTagsFocus(event)
{
    if(tagsTextArea.innerHTML == "add more tags") tagsTextArea.innerHTML = "";
}

function setTagListeners(array)
{
    array.forEach(element => {
        element.addEventListener("contextmenu", function(ev){
            ev.preventDefault();
            fetch(webOrigin + "/" + controller + "/PostNote/"
            + note,
            {
                "method": "POST",
                "body": JSON.stringify({ "Tags": ["--" + ev.target.innerHTML]}),
                "headers": {"content-type": "application/json"}
            }).then((response) => { if(!response.ok) showFailureStatus(); else showSuccessStatus(); },
                (showFailureStatus));
            ev.target.remove();
            return false;
        })        
    });
}

startup();
