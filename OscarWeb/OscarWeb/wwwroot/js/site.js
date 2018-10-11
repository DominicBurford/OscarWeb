// Write your Javascript code.

const DocumentOwner = "You are the owner of this document";
const DocumentSubscriber = "You are a subscriber of this document";

// <summary>
// Function is invoked when the user clicks the Details button from the home page (Index.cshtml)
// </summary>
function loadDocumentManagerForDocument(document) {

    onDocumentViewInit();

    if (document) {
        onDocumentViewNode(document);

        //AJAX call to Razor page handler to fetch the document / folder details

        $.ajax({
            type: "GET",
            url: `/DocumentManager/DocumentManager?handler=document&id=${document.id}`,
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                onDocumentViewSuccess(response);
            },
            failure: function (response) {
                onDocumentViewFailure(response);
            }
        });
    }
}

// <summary>
// Event that is fired as the user selects documents from the document treeview control
// from the DocumentManager.cshtml page (implemented in Pages\Components\Documents\Default.cshtml)
// </summary>
function onSelect_DocumentManagerItem(e) {
    var data = $('#documentview').data('kendoTreeView').dataItem(e.node);

    onDocumentViewInit();

    if (data) {
        onDocumentViewNode(data);

        //AJAX call to Razor page handler to fetch the document / folder details

        $.ajax({
            type: "GET",
            url: `/DocumentManager/DocumentManager?handler=document&id=${data.id}`,
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                onDocumentViewSuccess(response);
            },
            failure: function (response) {
                onDocumentViewFailure(response);
            }
        });
    }
}

// <summary>
// Resets the Document Manager toolbar items to initial values.
// Invoked from script on the Components\Documents\Default.cshtml page.
// </summary>
function onDocumentViewInit() {
    DisableToolbarButtons();

    $("#document_details_name")[0].innerHTML = "";
    $("#document_details_id")[0].innerHTML = "";
  //  $("#document_details_uid")[0].innerHTML = "";
    $("#document_details_children")[0].innerHTML = "";
    $("#document_details_description")[0].innerHTML = "";
    $("#document_details_lastviewed")[0].innerHTML = "";
    $("#document_details_uploadedby")[0].innerHTML = "";
    $("#document_details_type")[0].innerHTML = "";
    $("#document_details_category")[0].innerHTML = "";
    $("#document_details_created")[0].innerHTML = "";
    $("#document_details_ownersubscriber")[0].innerHTML = "";
}

function onDocumentViewNode(data) {
    $("#document_details_name")[0].innerHTML = data.text;
    $("#document_details_id")[0].innerHTML = data.id;
 //   $("#document_details_uid")[0].innerHTML = data.uid;

    //Set the values for the form handler parameters on the DocumentManager page.
    //These elements are created dynamically on the page DocumentManager.cshtml
    //e.g. <input id=@toolbar.PageHandler@toolbar.Method@toolbar.DisplayText type="hidden" name="currentDocumentId" value="0"/>
    $("#downloadpostDownload")[0].value = data.id;
    $("#vieweventspostView")[0].value = data.id;
    if (typeof ($("#editpostEdit")[0]) !== "undefined") {
        $("#editpostEdit")[0].value = data.id;
    }
    if (typeof ($("#deletepostDelete")[0]) !== "undefined") {
        $("#deletepostDelete")[0].value = data.id;
    }
    if (typeof ($("#uploadpostUpload")[0]) !== "undefined") {
        $("#uploadpostUpload")[0].value = data.id;
    }
    if (typeof ($("#createfolderpostCreate")[0]) !== "undefined") {
        $("#createfolderpostCreate")[0].value = data.id;
    }
}

function onDocumentViewSuccess(response) {
    $("#document_details_description")[0].innerHTML = response.Description;
    var lastViewDate = response.LastView;
    $("#document_details_lastviewed")[0].innerHTML = lastViewDate;
    if (lastViewDate !== "Never") {
        $("#document_details_lastviewed")[0].innerHTML = withoutTime(response.LastView);
    }
    $("#document_details_uploadedby")[0].innerHTML = response.UploadedByUsername;
    $("#document_details_type")[0].innerHTML = response.DocumentTypeDescription;
    $("#document_details_category")[0].innerHTML = response.DocumentCategoryDescription;
    $("#document_details_created")[0].innerHTML = withoutTime(response.Created);
    $("#document_details_ownersubscriber")[0].innerHTML = response.OwnerSubscriber;
    var documentFolder = response.DocumentFolder;
    if (documentFolder === "Document") {
        $("#document_details_children")[0].innerHTML = "Document";
        SetDocumentToolbar("Document");
    }
    else if (documentFolder === "Folder") {
        $("#document_details_children")[0].innerHTML = "Folder";
        $("#document_details_lastviewed")[0].innerHTML = "NA";
        SetDocumentToolbar("Folder");
    }

    //Set the toolbar items dependant on whether the  user is the owner or subscriber
    SetToolbarButtons(response.OwnerSubscriber);

    var listBox = $("#document_details_subscribers_view").data("kendoListBox");

    //update the subscriber list
    var data = [];
    if (typeof (response.Subscribers) === "object" && response.Subscribers.length > 0) {
        data = new kendo.data.DataSource({ data: response.Subscribers });
        listBox.setDataSource(data);
    } else {
        data = new kendo.data.DataSource({ data: ["None"] });
        listBox.setDataSource(data);
    }
}

function onDocumentViewFailure(response) {
    console.log(`failure ${JSON.stringify(response)}`);
}

function ProfileDropdown() {
    $("#profiledropdown").css({ top: "70px" });
    $("#profiledropdown").css({ transition: "1s" });
    $("#profile_open_button").css({ display: "none" });
    $("#profile_close_button").css({ display: "inherit" });
}

function ProfileDropdown_Shut() {
    $("#profiledropdown").css({ top: "-20%" });
    $("#profiledropdown").css({ transition: "1s" });
    $("#profile_open_button").css({ display: "inherit" });
    $("#profile_close_button").css({ display: "none" });
}

function showdocumentlist() {
    $("#informationbox").css({ display: "inline-block" });
}

// <summary>
// Heartbeat function to maintain the application's session state
// </summary>
function KeepSessionAlive() {
    $.ajax({
        type: "GET",
        url: "/Index?handler=session",
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
            //nothing to do!
        },
        failure: function (response) {
            console.log(`failure ${JSON.stringify(response)}`);
        }
    });
}

// <summary>
// Generic error handler function used by all Kendo UI grids
// </summary>
function error_handler_kendo_grid(e) {
    if (e.errors) {
        var message = "Errors:\n";
        $.each(e.errors, function (key, value) {
            if ("errors" in value) {
                $.each(value.errors, function () {
                    message += this + "\n";
                });
            }
        });
        //alert(message);
        console.log(message);
    }
}

// <summary>
// Return a formatted date for display
// e.g. Mon Apr 20 2018
// </summary>
function withoutTime(date){
    var d = new Date(date);
    return d.toDateString();
}

// <summary>
// Enable / disable the toolbar items on the Document Manager page
// </summary>
function SetDocumentToolbar(documenttype) {
    var toolbarItems = document.getElementsByClassName("btn-default");
    if (toolbarItems && toolbarItems.length > 0) {
        if (documenttype === "Folder") {
            for (var x = 0; x < toolbarItems.length; x++) {
                if (toolbarItems[x].classList.contains("Download")) {
                    toolbarItems[x].disabled = true;
                }
                if (toolbarItems[x].classList.contains("Upload")) {
                    toolbarItems[x].disabled = false;
                }
                if (toolbarItems[x].classList.contains("Create")) {
                    toolbarItems[x].disabled = false;
                }
            }
        } else {
            for (var y = 0; y < toolbarItems.length; y++) {
                if (toolbarItems[y].classList.contains("Download")) {
                    toolbarItems[y].disabled = false;
                }
                if (toolbarItems[y].classList.contains("Upload")) {
                    toolbarItems[y].disabled = true;
                }
                if (toolbarItems[y].classList.contains("Create")) {
                    toolbarItems[y].disabled = true;
                }
            }
        }
    }
}

// <summary>
// Disable the toolbar items on the Document Manager page.
// Invoked from onDocumentViewInit() each time the user selects a document / folder from the document treeview.
// </summary>
function DisableToolbarButtons() {
    var toolbarItems = document.getElementsByClassName("btn-default");
    if (toolbarItems && toolbarItems.length > 0) {
        for (var x = 0; x < toolbarItems.length; x++) {
            if (toolbarItems[x].classList.contains("Download")) {
                toolbarItems[x].disabled = true;
            }
            if (toolbarItems[x].classList.contains("Edit")) {
                toolbarItems[x].disabled = true;
            }
            if (toolbarItems[x].classList.contains("Delete")) {
                toolbarItems[x].disabled = true;
            }
            if (toolbarItems[x].classList.contains("Upload")) {
                toolbarItems[x].disabled = true;
            }
            if (toolbarItems[x].classList.contains("Create")) {
                toolbarItems[x].disabled = true;
            }
        }
    }
}

// <summary>
// Set the toolbar items dependant on whether the  user is the owner or subscriber
// </summary>
function SetToolbarButtons(ownersubscriber) {
    if ($("#document_details_children")[0].innerHTML === "Document") {
        var toolbarItems = document.getElementsByClassName("btn-default");
        if (toolbarItems && toolbarItems.length > 0) {
            if (ownersubscriber === DocumentSubscriber) {
                for (let y = 0; y < toolbarItems.length; y++) {
                    if (toolbarItems[y].classList.contains("Edit")) {
                        toolbarItems[y].disabled = true;
                    }
                    if (toolbarItems[y].classList.contains("Delete")) {
                        toolbarItems[y].disabled = true;
                    }
                }
            } else {
                for (let y = 0; y < toolbarItems.length; y++) {
                    if (toolbarItems[y].classList.contains("Edit")) {
                        toolbarItems[y].disabled = false;
                    }
                    if (toolbarItems[y].classList.contains("Delete")) {
                        toolbarItems[y].disabled = false;
                    }
                }
            }
        }
    }
}

// <summary>
// Event that is fired as the user selects a row from the User-Roles grid.
// Invoked from the OnChange() event of the grid on the Administration/UserRolesAdmin.cshtml page.
// </summary>
function onChange_UserGrid(arg) {
    var select = this.select();
    //the columns are tab-delimited so need splitting into their individual components
    var split = select[0].innerText.split("\t");
    var [userId, userName, userEmail] = split;
    console.log(`selected [0] ${userId} [1]${userName} [2]${userEmail}`);

    //Invoke the Razor page handler via AJAX to fetch the role(s) for the selected user

    $.ajax({
        type: "GET",
        url: `/Administration/UserRoles/UserRolesAdmin?handler=selecteduser&useremail=${userEmail}`,
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
            console.log(`success ${JSON.stringify(response)}`);
            UpdateUserRoleGrid(response);
        },
        failure: function (response) {
            console.log(`error ${JSON.stringify(response)}`);
        }
    });
}

// <summary>
// Event that is fired as the user selects a row from the Home Page unread documents grid.
// Invoked from the OnChange() event of the grid on the Index.cshtml page.
// </summary>
function onChange_UnreadDocuments(arg) {
    var select = this.select();
    //the columns are tab-delimited so need splitting into their individual components
    var split = select[0].innerText.split("\t");
    var [id, uploadedByUsername, name, description] = split;
    console.log(`selected [0] ${id} [1] ${uploadedByUsername} [2] ${name} [3] ${description}`);
    $("#unreaddocument_download")[0].value = id;
    $("#unreaddocument_details")[0].value = id;
    $("#submitDownload")[0].disabled = false;
    $("#submitDetails")[0].disabled = false;
}

// <summary>
// Helper function that is invoked from the onChange_UserGrid() event
// </summary>
function UpdateUserRoleGrid(response) {

    var source = $("#allroles").data("kendoListBox");

    //update the source roles list
    var datasource;
    if (typeof (response.AllRoles) === "object" && response.AllRoles.length > 0) {
        datasource = new kendo.data.DataSource({ data: response.AllRoles });
        source.setDataSource(datasource);
    } else {
        datasource = new kendo.data.DataSource({ data: [] });
        source.setDataSource(datasource);
    }
    
    var target = $("#selectedroles").data("kendoListBox");

    //update the target roles list
    var datatarget;
    if (typeof (response.UserRoles) === "object" && response.UserRoles.length > 0) {
        datatarget = new kendo.data.DataSource({ data: response.UserRoles });
        target.setDataSource(datatarget);
    } else {
        datatarget = new kendo.data.DataSource({ data: [] });
        target.setDataSource(datatarget);
    }
}

// <summary>
// Opens the Profile menu
// </summary>
function openProfileMenu() {
    document.getElementById('ProfileMenu').style.right = "0px";
    document.getElementById('openProfileMenu').style.display = "none";
    document.getElementById('closeProfileMenu').style.display = "inherit";
}

// <summary>
// Closes the Profile menu
// </summary>
function closeProfileMenu() {
    document.getElementById('ProfileMenu').style.right = "-180px";
    document.getElementById('openProfileMenu').style.display = "inherit";
    document.getElementById('closeProfileMenu').style.display = "none";
}

// <summary>
// Event that is fired as the user drags / drops documents in the document treeview control
// from the DocumentManager.cshtml page (implemented in Pages\Components\Documents\Default.cshtml)
// </summary>
function tree_drop(e) {
    var treeview = $('#documentview').data('kendoTreeView');
    var sourceUid = $(e.sourceNode).data("uid");
    var destinationUid = $(e.destinationNode).data("uid");

    var source = treeview.dataSource.getByUid(sourceUid);
    var destination = treeview.dataSource.getByUid(destinationUid);
    var url = `/DocumentManager/DocumentManager?handler=drop&sourceid=${source.id}&destinationid=${destination.id}`;

    //AJAX call to Razor page handler to update the parent ID of the document(s)
    $.get(url);
}

// <summary>
// Event that is fired when the Kendo grid has completed an operation 
// (Create, Read, Update, Destroy) from the Index.cshtml page
// </summary>
function onRequestEnd_UnreadDocuments(arg) {
    console.log(`Data operation for onRequestEnd_UnreadDocuments ${arg.type}`);
}

// <summary>
// Invoke the Download page handler. Invoked from Index.cshtml.
// NB this function is NOT used but has been left as an example of 
// invoking an ASP.NET Core page handler with a POST request.
// </summary>
function submitDownload() {
    var documentId = $("#unreaddocument_download")[0].value;
    console.log(`Selected document ID for download ${documentId}`);

    $.ajax({
        type: "POST",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        url: `/Index?handler=download&currentDocumentId=${documentId}`,
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
            console.log(`success downloading document${documentId}`);
        },
        failure: function (response) {
            console.log(`Failure downloading document${documentId}`);
            console.log(`Error response ${JSON.stringify(response)}`);
        }
    });
}

// <summary>
// Refresh the list of unread documents. Invoked from the Refresh button
// on the home page (Index.cshtml)
// </summary>
function refreshUnreadDocuments() {
    console.log("Refreshing unread documents");
    var grid = $("#unreaddocumentsgrid").data("kendoGrid");
    grid.dataSource.read();
}

// <summary>
// Open the CompanyAddressAdmin.cshtml page for the selected company.
// Invoked from the CompanyAdmin.cshtml page function addressClicked()
// </summary>
function viewCompanyAddresses(e) {
    // get the current table row (tr)
    var tr = $(e.target).closest("tr");
    // get the data bound to the current table row
    var grid = $("#companygrid").data("kendoGrid");
    var data = grid.dataItem(tr);
    console.log(`Details for: ${data.Id} ${data.Name} ${data.StorageContainerName}`);
    window.open(`CompanyAddressAdmin?companyId=${data.Id}`, "_self");
}
