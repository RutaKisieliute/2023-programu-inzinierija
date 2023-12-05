const webOrigin = window.location.protocol + "//" + window.location.host;
const controller = window.location.pathname.split('/')[1];
const $createFolderDialog = $("#create-folder-dialog");
const $moveNoteToFolderDialog = $("#move-note-to-folder-dialog");
var overflowSelectedNoteId = null;




function main() {
    enableRefreshOnNavigateBack();
    enablePopOvers();
    renderNotesContentAsInnerHtml();
    setOnClickListenersForCreateElements();
    setOnClickListenersForOverflowButtons();
    setOnClickListenerForCreateFolderButton()
    setOnClickListenerForCreateFolderDialog();
    setOnClickListenerForMoveNoteToFolderDialog();
    setOnClickListenersForFolderNameTexts();
}

// Start methods
function enableRefreshOnNavigateBack() {
    var perfEntries = performance.getEntriesByType("navigation");
    if (perfEntries[0].type === "back_forward") {
        location.reload();
    }
}
function enablePopOvers() {
    const content =
        `<div class="popover-container">
            <div class="popover-option popover-option-top">
                Move Note To Folder
            </div>
            <div class="popover-option popover-option-bottom">
                Unarchive Note
            </div>
        </div>`
    $('[data-bs-toggle="popover"]').each(function () {
        new bootstrap.Popover(this, {
            trigger: 'focus',
            content: content,
            html: true
        });
    });
}
function renderNotesContentAsInnerHtml() {
    $('.paragraph').each(function () {
        var doc = new DOMParser().parseFromString($(this).html(), "text/html");
        var content = doc.documentElement.textContent;
        $(this).html(content);
    });
}
function setOnClickListenersForCreateElements() {
    $(".folder").each(function () {
        const $createElement = $(this).find(".create-element");
        const folderId = $(this).data("folder-id");
        $createElement.on("click", function () {
            createEmptyNote(folderId);
        })
    });
}
function setOnClickListenersForOverflowButtons() {
    $(".folder").each(function () {
        const $noteElements = $(this).find(".scroller-element").slice(1);
        const folderName = $(this).find(".folder-name").html();

        $noteElements.each(function () {
            const $overflowButton = $(this).find(".overflow-btn");
            const noteName = $(this).find(".title").html();
            const noteId = $(this).data("note-id")
            $overflowButton.on("click", function (event) {
                event.stopPropagation();
            })
            $overflowButton.on("shown.bs.popover", function () {
                $(".popover-option-top").off("click");
                $(".popover-option-bottom").off("click");
                $(".popover-option-top").on("click", function () {
                    overflowSelectedNoteId = noteId;
                    showMoveNoteToFolderDialog(folderName, noteName);
                })
                $(".popover-option-bottom").on("click", function () {
                    unarchiveNote(noteId);
                })
            })
        })

    });
}
function setOnClickListenerForCreateFolderButton() {
    $("#create-folder-btn").on("click", function () {
        $createFolderDialog[0].showModal();
    });
}
function setOnClickListenerForCreateFolderDialog() {
    $createFolderDialog.on("click", (dismissDialogOnOutsideClick));
    $createFolderDialog.find("button")
        .on("click", function () {
            const folderName = $createFolderDialog.find("input")[0].value;
            if (isValidFolderName(folderName) && folderName !== undefined && folderName !== null) {
                createEmptyFolder(folderName);
            }
            else window.alert("Invalid folder name.");
        });
}
function setOnClickListenerForMoveNoteToFolderDialog() {
    $moveNoteToFolderDialog.on("click", (dismissDialogOnOutsideClick));
    $moveNoteToFolderDialog.find("button")
        .on("click", function () {
            const selectedFolderName = $moveNoteToFolderDialog.find("select")[0].value;
            const selectedFolderId = $moveNoteToFolderDialog.find("select option:selected").data("folder-id");
            if (selectedFolderName !== null && selectedFolderName !== undefined) {
                moveNoteToFolder(selectedFolderId, overflowSelectedNoteId);
            }
            else window.alert("Select folder to move your note.");
        });
}
function setOnClickListenersForFolderNameTexts() {
    $(".folder-name").dblclick(function () {
        if ($(this).find('input').length > 0) {
            return;
        }
        const oldName = $(this).text();
        const folderId = $(this).parent().data("folder-id");
        var input = $("<input>", {
            type: "text",
            value: oldName,
            blur: function () {
                const newName = $(this).val();
                if (isValidFolderName(newName)) {
                    $(this).parent().html(newName);
                    if (oldName !== newName) {
                        renameFolder(folderId, newName);
                    }
                }
                else {
                    $(this).parent().html(oldName);
                    window.alert("Invalid folder name.");
                }
            },
            keypress: function (e) {
                if (e.which === 13) { // 13 = ENTER
                    $(this).blur();
                }
            }
        });

        $(this).html("");
        $(this).append(input);
        input.focus();
    });
}
// Utility methods
function dismissDialogOnOutsideClick(e) {
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
function isValidFolderName(folderName) {
    var pattern = /^[a-zA-Z0-9 _-]{1,255}$/;
    return pattern.test(folderName.trim());
}
function getFolderNamesIdsDictionary() {
    var dict = [];
    $(".folder").each(function () {
        const folderName = $(this).find(".folder-name").html();
        const folderId = $(this).data("folder-id");
        dict.push({ folderName: folderName, folderId: folderId });
    });
    return dict;
}
function showMoveNoteToFolderDialog(folderName, noteName) {
    $moveNoteToFolderDialog[0].showModal();

    var dict = getFolderNamesIdsDictionary();
    var dropDownOptions = `<option value="" selected disabled>${folderName}</option>`;
    for (var obj of dict)
        if (obj.folderName !== folderName)
            dropDownOptions += `<option data-folder-id="${obj.folderId}">${obj.folderName}</option>`;
    
    $("#folder-selector").html(dropDownOptions);
    $moveNoteToFolderDialog.find("h6").html(`Move note '<span>${noteName}</span>' to folder`);
}




// API Calls (To MVC endpoints, not API endpoints)
function createEmptyNote(folderId) {
    fetch(webOrigin + "/" + controller + "/CreateEmptyNote/" + folderId, {
        method: "POST",
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
function unarchiveNote(noteId) {
    fetch(webOrigin + "/" + controller + "/UnarchiveNote/" + noteId, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("Js unarchiveNote error");
            } else {
                location.reload();
            }
        })
        .catch(error => {
            console.error("There was a problem with the fetch operation:", error);
        });
}
function moveNoteToFolder(folderId, noteId) {
    fetch(webOrigin + "/" + controller + "/MoveNoteToFolder/" + folderId + "/" + noteId, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("Js moveNoteToFolder error");
            } else {
                location.reload();
            }
        })
        .catch(error => {
            console.error("There was a problem with the fetch operation:", error);
        });
}

function renameFolder(folderId, newName) {
    fetch(webOrigin + "/" + controller + "/RenameFolder/" + folderId + "/" + newName, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("Js renameFolder error");
            } else {
                console.log("Renamed. " + folderId + " " + newName);
            }
        })
        .catch(error => {
            console.error("There was a problem with the fetch operation:", error);
        });
}

main();
