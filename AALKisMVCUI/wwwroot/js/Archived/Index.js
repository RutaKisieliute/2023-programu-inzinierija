const webOrigin = window.location.protocol + "//" + window.location.host;
const controller = window.location.pathname.split('/')[1];
var overflowSelectedNoteId = null;




function main() {
    enableRefreshOnNavigateBack();
    enablePopOvers();
    renderNotesContentAsInnerHtml();
    setOnClickListenersForOverflowButtons();
    setAge();
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
                Permanently Delete Note
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

function setOnClickListenersForOverflowButtons() {
    $(".scroller-element").each(function () {
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
                deleteNote(noteId);
            })
            $(".popover-option-bottom").on("click", function () {
                unarchiveNote(noteId);
            })
        })
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

function unarchiveNote(noteId) {
    console.log("trying to unarchive");
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

function deleteNote(noteId) {
    fetch(webOrigin + "/" + controller + "/DeleteNote/" + noteId, {
        method: "DELETE",
        headers: {
            "Content-Type": "application/json"
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("Js DeleteNote error");
            } else {
                location.reload();
            }
        })
        .catch(error => {
            console.error("There was a problem with the fetch operation:", error);
        });
}

function setAge()
{
    var a = document.querySelectorAll(".scroller-element");
    a.forEach(element => {
        const age = element.getAttribute("data-note-age");
        const title = element.querySelector(".title");
        console.log(age);
        var diff = Date.now() - Date.parse(age);
        var days = Math.floor(diff / 8640000);
        console.log(days);
        if(days > 255) days = 255;
        console.log(days);
        var rgb = days.toString(16);
        if(rgb == "0") rgb = "00";
        var ageColor = "#" + rgb + "0000";
        console.log(element);
        console.log(ageColor);
        title.style.color = ageColor;
    });
}

main();
