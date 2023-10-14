const webOrigin = window.location.protocol + "//" + window.location.host;
const controller = window.location.pathname.split('/')[1];
// Dictionary list [{folderName: x, htmlElementCreate: y}, ]
var htmlCreateElements = [];


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
        dict.htmlElementCreate.addEventListener("click", function () { onClick(dict.folderName); })
    }
}

function onClick(folderName) {
    createEmptyNote(folderName);
}

function createEmptyNote(folderName) {
    function success(response) {
        console.log(response.redirectToUrl);
    }
    function failure() {
        console.log("CreateEmptyNote request failed.");
    }
    fetch(webOrigin + "/" + controller + "/CreateEmptyNote/" + folderName, {
        method: "POST",
        body: JSON.stringify({ folderName: folderName }),
        headers: {
            "Content-Type": "application/json"
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("Network response was not ok");
            }
            return response.json(); // Parse the JSON data
        })
        .then(data => {
            // Access the redirectToUrl property from the parsed JSON data
            window.location.href = data.redirectToUrl;
        })
        .catch(error => {
            console.error("There was a problem with the fetch operation:", error);
        });
}
startup();
