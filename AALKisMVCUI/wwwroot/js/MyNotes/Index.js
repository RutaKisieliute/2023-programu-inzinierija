const webOrigin = window.location.protocol + "//" + window.location.host;
const controller = window.location.pathname.split('/')[1];
// Dictionary list [{folderName: x, htmlElementCreate: y}, ]
var htmlCreateElements = [];
// Dictionary list [{folderName: x, noteName: y, overflowButton: z}, ]
var overflowButtonElements = [];
const folderCreationDialog = document.getElementById("create-folder-dialog");
const folderChangeDialog = document.getElementById("change-folder-dialog");
const createFolderButton = document.getElementById("create-folder-btn");


function startup() {
    // Refresh page, if navigated with back button, to reflect edited note changes.
    var perfEntries = performance.getEntriesByType("navigation");
    if (perfEntries[0].type === "back_forward") {
        location.reload();
    }

    // Enable popovers
    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'))
    var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl, {
            trigger: 'focus',
            content:
            `<div class="popover-container">
                <div class="popover-option popover-option-top">
                    Change Folder
                </div>
                <div class="popover-option popover-option-bottom">
                    Archive Note
                </div>
            </div>`
            ,
            html: true
        })
    })

    document.querySelectorAll('.paragraph').forEach(paragraph => {
        const content = decodeHtmlEntities(paragraph.innerHTML);
        console.log(content);
        paragraph.innerHTML = content;
    });

    // Get html create elements with folder names.
    var folders = document.getElementsByClassName("folder");
    for (let folder of folders) {
        const folderName = folder.getElementsByClassName("folder-name")[0].innerHTML;
        const createElement = folder.getElementsByClassName("scroller")[0].getElementsByClassName("create-element")[0];
        htmlCreateElements.push({ "folderName": folderName, htmlElementCreate: createElement });

        var scrollerElements = folder.getElementsByClassName("scroller")[0].getElementsByClassName("scroller-element");
        var scrollerElements = Array.from(scrollerElements);
        scrollerElements.shift(); // remove first - create-element
        for (let scrollerElement of scrollerElements) {
            const noteName = scrollerElement.getElementsByClassName("title")[0].innerHTML;
            const overflowButton = scrollerElement.getElementsByClassName("overflow-btn")[0];
            overflowButtonElements.push({ "folderName": folderName, "noteName": noteName, "overflowButton": overflowButton });
        }
    }

    // Set on click listeners
    for (let dict of htmlCreateElements) {
        dict.htmlElementCreate.addEventListener("click", function () { onNoteClick(dict.folderName); })
    }
    for (let dict of overflowButtonElements) {
        dict.overflowButton.addEventListener("click", function (event) { onOverflowClick(event, dict.folderName, dict.noteName); })
        dict.overflowButton.addEventListener('shown.bs.popover', function () {
            //dict.overflowButton.getElementsByClassName("popover-option")[0].addEventListener("click", function () { onChangeFolderClick(folderName, noteName); });
            //dict.overflowButton.getElementsByClassName("popover-option")[1].addEventListener("click", function () { onArchiveNoteClick(folderName, noteName); });
            $(".popover-option-top").off("click");
            $(".popover-option-bottom").off("click");
            $(".popover-option-top").on("click", function () {
                onChangeFolderClick(dict.folderName, dict.noteName);
            })
            $(".popover-option-bottom").on("click", function () {
                onArchiveNoteClick(dict.folderName, dict.noteName);
            })
        });
    }
    createFolderButton.addEventListener("click", (onCreateFolderClick));
    folderCreationDialog.addEventListener("click", (onDialogClick));
    folderCreationDialog.getElementsByTagName("button")[0]
        .addEventListener("click", function () { onDialogButtonClick(folderCreationDialog.getElementsByTagName("input")[0].value); });
        
}


function decodeHtmlEntities(input) {
    var doc = new DOMParser().parseFromString(input, "text/html");
    return doc.documentElement.textContent;
}
function onNoteClick(folderName) {
    createEmptyNote(folderName);
}

function onOverflowClick(event, folderName, noteName) {
    event.stopPropagation();
    console.log("Overflow: " + folderName + " " + noteName);    
}
function onChangeFolderClick(folderName, noteName) {
    folderChangeDialog.showModal();
    console.log("Change Folder: " + folderName + " " + noteName);
}

function onArchiveNoteClick(folderName, noteName) {
    archiveNote(folderName, noteName);
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

function archiveNote(folderName, noteName) {
    fetch(webOrigin + "/" + controller + "/ArchiveNote/" + folderName + "/" + noteName, {
        method: "POST",
        body: JSON.stringify({ folderName: folderName, noteName: noteName }),
        headers: {
            "Content-Type": "application/json"
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("Js archiveNote error");
            } else {
                //location.reload();
            }
        })
        .catch(error => {
            console.error("There was a problem with the fetch operation:", error);
        });
}

startup();
