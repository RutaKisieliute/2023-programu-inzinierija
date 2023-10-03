var pageHost;
var pagePath;
var mainNoteContainerDiv;
var mainClearNoteDataButton;
var mainNoteAreaTextarea;

/**
 * Entry function.
 *
 * @author  AluminumAlman
 */
function startup()
{
    pageHost = window.location.protocol + "//" + window.location.host;
    pagePath = window.location.pathname;

    mainNoteContainerDiv = document.getElementById("main-note-container");

    mainClearNoteDataButton = document.getElementById("main-clear-note-data");
    mainClearNoteDataButton.setAttribute("onclick", "clearNoteAreaData(this, mainNoteAreaTextarea);");

    mainNoteAreaTextarea = document.getElementById("main-note-area");
    mainNoteAreaTextarea.setAttribute("oninput", "updateNoteAreaHTML(this, this.value);");
    mainNoteAreaTextarea.setAttribute("onchange", "saveNoteArea(this);");
    loadNoteArea(mainNoteAreaTextarea);
}

/**
 * Updates the HTML contents of a textArea (assumed to be a noteArea).
 *
 * @author  AluminumAlman
 * @param   {textarea}  noteArea    Which note area (textarea) to update.
 * @param   {string}    noteText    With what to update the note area.
 */
function updateNoteAreaHTML(noteArea, noteText)
{
    noteArea.setHTML(noteText);
    //noteArea.replaceChildren(...createElementFromHTML(noteText));
    console.log(noteText);
}

/**
 * Saves the note area (currently only) into local storage.
 *
 * @author  AluminumAlman
 * @param   {textarea}  noteArea    Which note area (textarea) to save.
 */
function saveNoteArea(noteArea)
{
    let noteText = noteArea.value;
    localStorage.setItem(noteArea.id, noteText);
    let postBody = {
        Id: noteArea.id,
        Contents: noteText
    };
    fetch(pageHost + pagePath + "/NotePost", {
        method: "POST",
        body: JSON.stringify(postBody)
    })
        .then((response) => console.log(response),
            (response) => console.warn(response));
}

/**
 * Loads the note area (currently only) from local storage.
 *
 * @author  AluminumAlman
 * @param   {textarea}  noteArea    Which note area (textarea) to load.
 */
function loadNoteArea(noteArea)
{
    let noteText = localStorage.getItem(noteArea.id);
    updateNoteAreaHTML(noteArea, noteText);
}

/**
 * Clears the saved data of a note area.
 *
 * @author  AluminumAlman
 * @param   {button}    clearButton     Which button called the function.
 * @param   {textarea}  noteArea        Which note area's data to clear.
 */
function clearNoteAreaData(clearButton, noteArea)
{
    const clearedText = "";
    noteArea.value = clearedText;
    updateNoteAreaHTML(noteArea, clearedText);
    saveNoteArea(noteArea);
}

/**
 * Adds to an attribute of a HTML tag
 *
 * @author  AluminumAlman
 * @param   {object}    tag         HTML tag to add to.
 * @param   {string}    attribute   Attribute to add to.
 * @param   {string}    addition    The value to add to the attribute.
 */
function addTagAttribute(tag, attribute, addition)
{
    tag.setAttribute(attribute,
        tag.getAttribute(attribute) + " " + addition);
    return tag;
}

/**
 * Gets an element from a list by a selector function.
 *
 * @author  AluminumAlman
 * @param   {object}    list    List-like object to iterate over;
 *                              must have either getLength() or length
 *                              and at(integer), item(integer) or [integer].
 * @param   {f}         func    Function called by func(i) while iterating over the object;
 *                              iterating ends when it returns a 'true' value for if(...)
 *                              (not 0, or not null, or a boolean true).
 */
function getFromListBy(list, func)
{
    let length = (list.getLength && list.getLength()) || list.length;
    if(list.at) for(let i = 0; i < length; ++i)
    {
        if(func(list.at(i)))
        {
            return list.at(i);
        }
    }
    else if(list.item) for(let i = 0; i < length; ++i)
    {
        if(func(list.item(i)))
        {
            return list.item(i);
        }
    }
    else for(let i = 0; i < length; ++i)
    {
        if(func(list[i]))
        {
            return list[i];
        }
    }
}

startup();
