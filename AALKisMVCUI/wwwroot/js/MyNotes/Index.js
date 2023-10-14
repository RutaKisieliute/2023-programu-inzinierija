const webOrigin = window.location.protocol + "//" + window.location.host;
const controller = window.location.pathname.split('/')[1];
// Dictionary list [{folderName: x, htmlElementCreate: y}, ]
var htmlCreateElements = [];
const folderCreationDialog = document.getElementById("create-folder-dialog");
const createFolderButton = document.getElementById("create-folder-btn");


function startup() {
    // Refresh page, if navigated with back button, to reflect edited note changes.
    var perfEntries = performance.getEntriesByType("navigation");

    if (perfEntries[0].type === "back_forward") {
        location.reload();
    }
    // Get html create elements with folder names.
    var folders = document.getElementsByClassName("folder");
    for (let folder of folders) {
        const folderName = folder.getElementsByClassName("folder-name")[0].innerHTML;
        const createElement = folder.getElementsByClassName("scroller")[0].getElementsByClassName("create-element")[0];
        htmlCreateElements.push({ "folderName": folderName, htmlElementCreate: createElement });
    }

    // Set on click listeners
    for (let dict of htmlCreateElements) {
        dict.htmlElementCreate.addEventListener("click", function () { onNoteClick(dict.folderName); })
    }
    createFolderButton.addEventListener("click", (onCreateFolderClick));
    folderCreationDialog.addEventListener("click", (onDialogClick));
    folderCreationDialog.getElementsByTagName("button")[0]
        .addEventListener("click", function () { onDialogButtonClick(folderCreationDialog.getElementsByTagName("input")[0].value) });
        
}

function onNoteClick(folderName) {
    createEmptyNote(folderName);
}

function onCreateFolderClick() {
    folderCreationDialog.showModal();
}

function onDialogClick(e) {
    if (e.target.tagName !== 'DIALOG') //This prevents issues with forms
        return;

    const rect = e.target.getBoundingClientRect();

    const clickedInDialog = (
        rect.top <= e.clientY &&
        e.clientY <= rect.top + rect.height &&
        rect.left <= e.clientX &&
        e.clientX <= rect.left + rect.width
    );

    if (clickedInDialog === false)
        e.target.close();
}

function onDialogButtonClick(folderName) {
    if (isValidFolderName(folderName)) {
        createEmptyFolder(folderName);
    }
    else window.alert("Invalid folder name.");
}


function isValidFolderName(folderName) {
    var pattern = /^[a-zA-Z0-9 _-]{1,255}$/;
    return pattern.test(folderName);
}
function createEmptyNote(folderName) {
    fetch(webOrigin + "/" + controller + "/CreateEmptyNote/" + folderName, {
        method: "POST",
        body: JSON.stringify({ folderName: folderName }),
        headers: {
            "Content-Type": "application/json"
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("Js createEmptyNote error");
            }
            return response.json();
        })
        .then(data => {
            // Access the redirectToUrl property from the parsed JSON data sent by MVC
            window.location.href = data.redirectToUrl;
        })
        .catch(error => {
            console.error("There was a problem with the fetch operation:", error);
        });
}

function createEmptyFolder(folderName) {
    fetch(webOrigin + "/" + controller + "/CreateEmptyFolder/" + folderName, {
        method: "POST",
        body: JSON.stringify({ folderName: folderName }),
        headers: {
            "Content-Type": "application/json"
        }
    })
        .then(response => {
            if (!response.ok) {
                if (response.status === 400)
                    window.alert("Folder with such name already exists.")
                else
                    throw new Error("Js createEmptyFolder error");

            } else {
                location.reload();
            }
        })
        .catch(error => {
            console.error("There was a problem with the fetch operation:", error);
        });
}

startup();
