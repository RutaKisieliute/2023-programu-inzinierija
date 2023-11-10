const webOrigin = window.location.protocol + "//" + window.location.host;
const controller = window.location.pathname.split('/')[1];
const $createFolderDialog = $("#create-folder-dialog");
const $changeFolderDialog = $("#change-folder-dialog");




function main() {
    enableRefreshOnNavigateBack();
    enablePopOvers();
    renderNotesContentAsInnerHtml();
    setOnClickListenersForCreateElements();
    setOnClickListenersForOverflowButtons();
    setOnClickListenerForCreateFolderButton()
    setOnClickListenerForCreateFolderDialog();
    setOnClickListenerForChangeFolderDialog();
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
                Change Folder
            </div>
            <div class="popover-option popover-option-bottom">
                Archive Note
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
            $overflowButton.on("click", function (event) {
                event.stopPropagation();
            })
            $overflowButton.on("shown.bs.popover", function () {
                $(".popover-option-top").off("click");
                $(".popover-option-bottom").off("click");
                $(".popover-option-top").on("click", function () {
                    showChangeFolderDialog(folderName, noteName);
                })
                $(".popover-option-bottom").on("click", function () {
                    archiveNote(folderName, noteName);
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
function setOnClickListenerForChangeFolderDialog() {
    $changeFolderDialog.on("click", (dismissDialogOnOutsideClick));
    $changeFolderDialog.find("button")
        .on("click", function () {
            const toFolderName = $changeFolderDialog.find("select")[0].value;
            if (toFolderName !== null && toFolderName !== undefined) {
                const noteName = $changeFolderDialog.find("h6 span").html();
                const fromFolderName = $changeFolderDialog.find("select option").html();
                changeFolderName(toFolderName, fromFolderName, noteName);
            }
            else window.alert("Select folder to move your note.");

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
    return pattern.test(folderName);
}
function getFolderNames() {
    var folderNames = [];
    $(".folder").each(function () {
        const folderName = $(this).find(".folder-name").html();
        folderNames.push(folderName);
    });
    return folderNames;
}
function showChangeFolderDialog(folderName, noteName) {
    $changeFolderDialog[0].showModal();

    var folderNames = getFolderNames();
    var dropDownOptions = `<option value="" selected disabled>${folderName}</option>`;
    for (var name of folderNames)
        if (name != folderName)
            dropDownOptions += `<option>${name}</option>`;
    
    $("#folder-selector").html(dropDownOptions);
    $changeFolderDialog.find("h6").html(`Move note '<span>${noteName}</span>' to folder`);
}




// API Calls (To MVC endpoints, not API endpoints)
function createEmptyNote(folderId) {
    fetch(webOrigin + "/" + controller + "/CreateEmptyNote/" + folderId, {
        method: "POST",
        body: JSON.stringify({ folderId: folderId }),
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
                location.reload();
            }
        })
        .catch(error => {
            console.error("There was a problem with the fetch operation:", error);
        });
}
function changeFolderName(newFolderName, oldFolderName, noteName) {
    fetch(webOrigin + "/" + controller + "/ChangeFolderName/" + newFolderName + "/" + oldFolderName + "/" + noteName, {
        method: "POST",
        body: JSON.stringify({ newFolderName: newFolderName, oldFolderName: oldFolderName, noteName: noteName }),
        headers: {
            "Content-Type": "application/json"
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("Js archiveNote error");
            } else {
                location.reload();
            }
        })
        .catch(error => {
            console.error("There was a problem with the fetch operation:", error);
        });
}

main();
