const DEBUG = true;

function startup()
{
    // note-area textarea
    {
        let noteAreas = document.body.getElementsByClassName("note-area");
        for(let i = 0; i < noteAreas.length; ++i)
        {
            let noteArea = noteAreas[i];
            noteArea.value = localStorage.getItem(noteAreaIdentifier(noteArea));
            noteAreaUpdateHTML(noteArea);
            noteArea.setAttribute("oninput", "noteAreaUpdateHTML(this);");
            noteArea.setAttribute("onchange", "noteAreaSave(this);");
            //console.log(noteArea);
        }
    }

    // reset-storage button
    {
        let buttons = document.body.getElementsByClassName("reset-storage");
        for(let i = 0; i < buttons.length; ++i)
        {
            let button = buttons[i];
            button.setAttribute("onclick", "resetStorage();");
        }
    }
}

const noteAreaIdentifier = (noteArea) => noteArea.className + "/" + noteArea.id;

// https://stackoverflow.com/questions/494143
function createElementFromHTML(htmlString) {
    let div = document.createElement('div');
    div.innerHTML = htmlString.trim();
    return div.childNodes;
}

function noteAreaUpdateHTML(noteArea)
{
    let noteText = noteArea.value;
    noteArea.setHTML(noteText);
    //noteArea.replaceChildren(...createElementFromHTML(noteText));
    console.log(noteText);
}

function noteAreaSave(noteArea)
{
    let noteText = noteArea.value;
    localStorage.setItem(noteAreaIdentifier(noteArea), noteText);
    //let postBody = {
    //    "identifier": noteAreaIdentifier(noteArea),
    //    "value": noteText
    //};
    //fetch("http://localhost:5001", postBody)
    //    .then((response) => {
    //        if(!response.ok)
    //            return;
    //        console.warn(response);
    //    });
}

function resetStorage()
{
    let noteAreas = document.body.getElementsByClassName("note-area");
    for(let i = 0; i < noteAreas.length; ++i)
    {
        let noteArea = noteAreas[i];
        noteArea.value = "";
        noteAreaUpdateHTML(noteArea);
        noteAreaSave(noteArea);
    }
}

startup();
