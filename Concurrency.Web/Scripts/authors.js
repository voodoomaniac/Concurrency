$(function () {
    authorManager.UpdateAuthorsList();
});

$.template("authorsRowTemplate", "<tr id='author${Id}'><td>${Id}</td><td class='firstName'>${FirstName}</td><td class='lastName'>${LastName}</td>"
               + " <td> <a href='javascript: ;' onclick=\"authorManager.OpenForEditing('${Id}')\">Edit</a></td>"
               + " <td> <a href='javascript: ;' onclick=\"authorManager.DeleteAuthor('${Id}')\">Remove</a></td></tr>");

$.template("authorConflictErrorMessageTemplate", "<div class='has-error'><span class='help-block'>Value in database is <a href='javascript: ;' " +
    "onclick='authorManager.ApplyValueFromErrorMessage(\"${value}\", this)'>${value}</a> Click it to apply or leave your value.</span></div>");

$.template("applyAllConflictsButtonTemplate", "<a id='applyAllConflicts' class='btn btn-default' href='javascript: ;' onclick='authorManager.ApplyAllValuesFromErrorMessages()'> Apply all values from DB </a>");


var authorManager = {
    UpdateAuthorsList: function () {
        $.getJSON("/api/authors", function (data) {
            $("#authorsListRows")[0].innerHTML = "";
            $("#add_firstName")[0].value = "";
            $("#add_lastName")[0].value = "";
            $.tmpl("authorsRowTemplate", data).appendTo("#authorsListRows");
        });
    },

    AddAuthor: function () {
        var author = {
            FirstName: $("#add_firstName")[0].value,
            LastName: $("#add_lastName")[0].value
        };

        $.ajax({
            url: '/api/authors/',
            type: 'POST',
            data: JSON.stringify(author),
            contentType: "application/json",
            success: function (data) {
                alert('Author added successfully');
                authorManager.UpdateAuthorsList();
            },
            error: function (xhr) {
                alert('Author not added: ' + xhr.responseText);
            }
        });

        return false;
    },

    OpenForEditing: function (authorId) {

        $.getJSON('/api/authors/' + authorId, function (data) {

            $("#editAuthorBlockTitle")[0].innerText = "Edit author id " + data.Id;
            authorManager.ClearAllErrors();

            $("#edit_id")[0].value = data.Id;
            $("#edit_firstName")[0].value = data.FirstName;
            $("#edit_lastName")[0].value = data.LastName;
            $("#edit_original_firstName")[0].value = data.OriginalFirstName;
            $("#edit_original_lastName")[0].value = data.OriginalLastName;

            $("#listOfAuthorsBlock").hide();
            $("#editAuthorBlock").show();

            $("#editAuthorForm")[0].scrollIntoView();
        });
    },

    EditAuthor: function () {
        var author = {
            Id: $("#edit_id")[0].value,
            FirstName: $("#edit_firstName")[0].value,
            LastName: $("#edit_lastName")[0].value,
            OriginalFirstName: $("#edit_original_firstName")[0].value,
            OriginalLastName: $("#edit_original_lastName")[0].value
        };

        $.ajax({
            type: "PUT",
            url: "/api/authors",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(author),
            success: function (data) {
                alert('Author edited successfully');
                $("#listOfAuthorsBlock").show();
                $("#editAuthorBlock").hide();
                authorManager.UpdateAuthorsList();
            },
            error: function (xhr) {
                alert('Author not edited: ' + xhr.statusText);
                // Handle concurrency error
                if (xhr.statusText.indexOf('Concurrency') !== -1) {
                    authorManager.AuthorEditUpdateAfterConcurrency(xhr.responseJSON);
                }
            }
        });
    },

    AuthorEditUpdateAfterConcurrency: function (author) {
        $("#editAuthorBlockTitle")[0].innerText = "Concurrency conflict resolver for author id " + author.Id;
        $.tmpl("applyAllConflictsButtonTemplate", {}).appendTo("#editAuthorForm");

        $("#edit_id")[0].value = author.Id;
        $("#edit_original_firstName")[0].value = author.FirstName;
        $("#edit_original_lastName")[0].value = author.LastName;

        authorManager.CheckAndAssignFieldError(author.FirstName, "#edit_firstName");
        authorManager.CheckAndAssignFieldError(author.LastName, "#edit_lastName");
    },

    CheckAndAssignFieldError: function (authorFieldValue, inputId) {
        if (authorFieldValue !== $(inputId)[0].value) {
            $.tmpl("authorConflictErrorMessageTemplate", { value: authorFieldValue }).appendTo($(inputId).parent());
        }
    },

    ApplyValueFromErrorMessage: function (value, selector) {
        $(selector).closest("div.form-group").find("input")[0].value = value;
    },

    ApplyAllValuesFromErrorMessages: function () {
        var formGroups = $("#editAuthorForm div.form-group");
        for (var i = 0; i < formGroups.length; i++) {
            var errorBlock = $(formGroups[i]).find("div.has-error")[0];
            if (errorBlock) {
                var valueFromErrorBlock = $(errorBlock).find("a")[0].innerText;
                $(formGroups[i]).find("input")[0].value = valueFromErrorBlock;
            }
        }
    },

    ClearAllErrors: function () {
        // Remove apply all conflicts button
        var applyAllConflictsButton = $("#applyAllConflicts");
        if (applyAllConflictsButton) {
            applyAllConflictsButton.remove();
        }

        // Remove all errors
        var formGroups = $("#editAuthorForm div.form-group");
        for (var i = 0; i < formGroups.length; i++) {
            var errorBlock = $(formGroups[i]).find("div.has-error")[0];
            if (errorBlock) {
                errorBlock.remove();
            }
        }
    },

    DeleteAuthor: function (authorId) {
        var author = {
            Id: authorId,
            FirstName: $("#author" + authorId).find(".firstName").text(),
            LastName: $("#author" + authorId).find(".lastName").text()
        };

        $.ajax({
            type: "DELETE",
            url: "/api/authors",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(author),
            success: function (data) {
                alert('Author deleted successfully');
                document.getElementById('author' + authorId).remove();
            },
            error: function (xhr) {
                alert('Author not deleted: ' + xhr.responseText);
            }
        });
    }
};