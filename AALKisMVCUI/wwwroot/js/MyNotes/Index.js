const webOrigin = window.location.protocol + "//" + window.location.host;
const controller = window.location.pathname.split('/')[1];
const $createFolderDialog = $("#create-folder-dialog");
const $moveNoteToFolderDialog = $("#move-note-to-folder-dialog");
const $exportNotesDialog = $("#export-notes-dialog");
const $importNotesDialog = $("#import-notes-dialog");
const $dropArea = $("#drop-area");
var overflowSelectedNoteId = null;

var singleNote = {title: null, content: null};




function main() {
    enableRefreshOnNavigateBack();
    enablePopOvers();
    renderNotesContentAsInnerHtml();

    setOnClickListenersForCreateElements();
    setOnClickListenersForFolderNameTexts();
    setOnClickListenersForOverflowButtons();

    setOnClickListenerForMenuButtons();

    setOnClickListenerForCreateFolderDialog();
    setOnClickListenerForMoveNoteToFolderDialog();
    setOnClickListenerForExportNotesDialog();
    setOnClickListenerForImportNotesDialog();

    setDropAreaListeners();
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
            createNote(folderId);
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
                    archiveNote(noteId);
                })
            })
        })

    });
}
function setOnClickListenerForMenuButtons() {
    $("#create-folder-btn").on("click", function () {
        $createFolderDialog[0].showModal();
    });
    $("#export-notes-btn").on("click", function () {
        $exportNotesDialog[0].showModal();
    });
    $("#import-notes-btn").on("click", function () {
        $importNotesDialog[0].showModal();
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

function setOnClickListenerForExportNotesDialog() {
    $exportNotesDialog.on("click", (dismissDialogOnOutsideClick));
    $exportNotesDialog.find("button")
        .on("click", function () {
            exportNotes();
        });
}
function setOnClickListenerForImportNotesDialog() {
    $importNotesDialog.on("click", (dismissDialogOnOutsideClick));
    $("#import-notes-dialog-btn-file")
        .on("click", function () {
            importSingleNote();
        });
    $("#import-notes-dialog-btn-folder")
        .on("click", function () {
            importMultipleNotes();
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

function setDropAreaListeners() {
    $dropArea[0].addEventListener(
        "dragover",
        function (e) {
            e = e || event;
            e.preventDefault();
        },
        false
    );

    $dropArea[0].addEventListener(
        "drop",
        function (event) {
            event.preventDefault();
            var items = event.dataTransfer.items;
            if (items.length > 1) {
                window.alert("Please import 1 file/folder at a time.");
                return;
            }

            getFilesDataTransferItems(items).then(files => {
                console.log(files);
                filterAndReadTextFiles(files).then(filteredFiles => {
                    console.log(filteredFiles);
                    // IT IS THE DESIGN OF OUR BACKEND!
                    if (filteredFiles.length === 0) {
                        window.alert("Your provided folder contains no notes.");
                    }
                    else if (filteredFiles.length === 1 && occ(filteredFiles[0].filename, "/") === 0) {
                        // Case 1
                        var dict = getFolderNamesIdsDictionary();
                        var dropDownOptions = `<option value="" selected disabled>-</option>`;
                        for (var obj of dict)
                                dropDownOptions += `<option data-folder-id="${obj.folderId}">${obj.folderName}</option>`;

                        $("#import-note-folder-selector").html(dropDownOptions);
                        let notename = filteredFiles[0].filename.replace(/\.[^/.]+$/, ""); // remove extension if there is one
                        $("#import-file-note-name").html(notename);

                        singleNote = { title: notename, content: filteredFiles[0].content };

                        $("#drop-area").attr("hidden", "");
                        $("#import-file-container").removeAttr("hidden");
                        return;
                    }
                    let containsNoteInLevelOne = false;
                    for (let file of filteredFiles) {
                        if (occ(file, "/") === 1) {
                            containsNoteInLevelOne = true;
                            break;
                        }
                    }
                    /*
                    let folders = [];
                    if (containsNoteInLevelOne) {
                        // Case 2
                        folders.push({ foldername: filteredFiles[0].filename.split("/")[0], notes: filteredFiles });
                    }
                    else {
                        // Case 3
                        for (let file in filteredFiles) {
                            let notefolder = file.filename.split("/")[1];
                            let contains = false;
                            for (let folder in folders) {
                                if (folder.foldername === notefolder) {
                                    contains = true;
                                    folder.notes.push(file);
                                    break;
                                }
                            }
                            if (!contains) {
                                folders.push({ foldername: notefolder, notes: [file] });
                            }
                        }
                    }
                    */

                });
                /*
                    [{
                        name: "foldername"
                        subfolder: [
                            File {filpath, size, name}
                            Object {name, subfolder}
                        ]
                    }]
                    THREE CASES
                    CASE 1 - Uploaded single file
                    Give dropdown to select existing folder, to which to upload the note.

                    CASE 2 - Uploaded folder, that contains at least one note
                    Create folder with that name, if it doesn't exist already and flatten all subfolders, upload their notes too.

                    CASE 3 - Uploaded folder, that contains only other folders that contains notes.
                    Create many folders and upload to them their corresponding notes. If there are further subfolders, that contain notes, flatten them.
                */
            });
        },
        false
    );
}

// Utility methods
function occ(str, substr) {
    let regex = new RegExp(substr, "g");
    return (str.match(regex) || []).length;
}
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

function sanitizeFilename(filename) {
    const forbiddenCharsRegExp = /[<>:"\/\\|?*\x00-\x1F]/g;
    return filename.replace(forbiddenCharsRegExp, '');
}

// Drag n drop
// 3 methods purely for aesthetics
function allowDrop(ev) {
    $(ev.target).attr("drop-active", true);
    ev.preventDefault();
}
function leaveDropZone(ev) {
    $(ev.target).removeAttr("drop-active");
}
function drop(ev) {
    ev.preventDefault();
    $(ev.target).removeAttr("drop-active");
}


function getFilesDataTransferItems(dataTransferItems) {
    function traverseFileTreePromise(item, path = "", folder) {
        return new Promise(resolve => {
            if (item.isFile) {
                item.file(file => {
                    file.filepath = (path || "") + "" + file.name;
                    folder.push(file);
                    resolve(file);
                });
            } else if (item.isDirectory) {
                let dirReader = item.createReader();
                dirReader.readEntries(entries => {
                    let entriesPromises = [];
                    subfolder = [];
                    folder.push({ name: item.name, subfolder: subfolder });
                    for (let entr of entries)
                        entriesPromises.push(
                            traverseFileTreePromise(entr, (path || "") + item.name + "/", subfolder)
                        );
                    resolve(Promise.all(entriesPromises));
                });
            }
        });
    }

    let files = [];
    return new Promise((resolve, reject) => {
        let entriesPromises = [];
        for (let it of dataTransferItems)
            entriesPromises.push(
                traverseFileTreePromise(it.webkitGetAsEntry(), null, files)
            );
        Promise.all(entriesPromises).then(entries => {
            resolve(files);
        });
    });
}

function filterAndReadTextFiles(files) {
    function filterFiles(files) {
        let filteredFiles = [];
        for (var file of files) {
            if (file instanceof File) {
                if (file.type === 'text/plain' || (file.name.endsWith('.txt') || file.name.endsWith('.md'))) {
                    filteredFiles.push(file);
                }
            }
            else {
                var subdirFiles = filterFiles(file.subfolder);
                for (var subdirFile of subdirFiles) {
                    filteredFiles.push(subdirFile);
                }
            }
        }
        return filteredFiles;
    }

    return new Promise(async (resolve, reject) => {
        

        let filteredFiles = filterFiles(files);
        let filesData = [];
        for (let file of filteredFiles) {
            const content = await readFileContent(file);
            filesData.push({ filename: file.filepath, content: content });
        }

        resolve(filesData);
    });
}

function readFileContent(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => resolve(reader.result);
        reader.onerror = reject;
        reader.readAsText(file);
    });
}

// Export - import
function crawlNotesForExport() {
    /*
    returns: 
    [{
        folderName: ..., notes: 
        [{
            title: ...,
            content: ...
        }]
    }]
    */


    // dict folder-id checked
    // dict note-id folder-id checked

    var folderCheckboxes = $('input[type="checkbox"][data-folder-id]');
    var folderDict = folderCheckboxes.map(function () {
        return {
            folderId: $(this).data('folder-id'),
            checked: $(this).prop('checked')
        };
    }).get();

    var noteCheckboxes = $('input[type="checkbox"][data-note-id][data-parent-folder-id]');
    var noteDict = noteCheckboxes.map (function () {
        return {
            folderId: $(this).data('parent-folder-id'),
            noteId: $(this).data('note-id'),
            checked: $(this).prop('checked')
        };
    }).get();

    var notesForExport = [];
    for (var folderInfo of folderDict) {
        if (folderInfo.checked) {
            //const folderName = $(`div[data-folder-id=${folderInfo.folderId}] p`).html();
            const folderName = $(`label[for="folder-${folderInfo.folderId}"]`).html();
            var folderNotes = [];
            for (var noteInfo of noteDict) {
                if (noteInfo.folderId === folderInfo.folderId) {
                    if (noteInfo.checked) {
                        const noteTitle = $(`label[for="note-${noteInfo.noteId}"]`).html();
                        const noteContent = $(`div[data-note-id=${noteInfo.noteId}] > a > div.paragraph`).html();
                        folderNotes.push({ title: noteTitle, content: noteContent });
                    }
                    else {
                        break;
                    }
                }
            }
            notesForExport.push({ folderName: folderName, notes: folderNotes });
        }
    }
    return notesForExport;
    

}
function exportNotes() {

    const notesForExport = crawlNotesForExport();
    var zip = new JSZip();

    for (var folderNotes of notesForExport) {
        zip.folder(sanitizeFilename(folderNotes.folderName));
        for (var note of folderNotes.notes) {
            zip.file(`${sanitizeFilename(folderNotes.folderName)}/${sanitizeFilename(note.title)}.md`, note.content);
        }
    }
    
    const filename = "exported-notes.zip";
    // Generate the zip file
    zip.generateAsync({ type: "blob" })
        .then(function (content) {
            // Trigger the download
            if (window.navigator && window.navigator.msSaveOrOpenBlob) {
                // For Edge or Internet explorer
                window.navigator.msSaveOrOpenBlob(content, filename);
            } else {
                // For other browsers
                var a = document.createElement('a');
                a.download = filename;
                a.href = window.URL.createObjectURL(content);
                a.dataset.downloadurl = ['application/zip', a.download, a.href].join(':');
                var e = document.createEvent('MouseEvents');
                e.initEvent('click', true, false, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);
                a.dispatchEvent(e);
            }
        });
}

function importSingleNote() {
    //const selectedFolderName = $("#import-note-folder-selector").value;
    const selectedFolderId = $("#import-note-folder-selector option:selected").data("folder-id");
    if (selectedFolderId !== null && selectedFolderId !== undefined) {
        createNote(selectedFolderId, singleNote.title, singleNote.content);
    }
    else window.alert("Select folder to move your note.");

}

function importMultipleNotes() {
    window.alert("To be implemented multiple note!");
}

// API Calls (To MVC endpoints, not API endpoints)
function createNote(folderId, title = "Untitled", content = "") {
    fetch(webOrigin + "/" + controller + "/CreateNote/" + folderId, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ title: title, content: content })
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
function archiveNote(noteId) {
    fetch(webOrigin + "/" + controller + "/ArchiveNote/" + noteId, {
        method: "POST",
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
