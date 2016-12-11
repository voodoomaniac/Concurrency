$(function () {
    bookManager.UpdateBooksList();
    $('#datetimepicker1').datetimepicker();
    $('#datetimepicker2').datetimepicker();
});

$.template("booksRowTemplate", "<tr id='book${Id}'><td>${Id}</td><td class='name'>${Name}</td><td class='publishDate'>${PublishDate}</td>"
               + "<td class='isLocked'>${IsLocked}</td><td class='lockedBy'>${LockedBy}</td>"
               + " <td> <a href='javascript: ;' onclick=\"bookManager.OpenForEditing('${Id}')\">Edit</a></td>"
               + " <td> <a href='javascript: ;' onclick=\"bookManager.DeleteBook('${Id}')\">Remove</a></td></tr>");


var bookManager = {
    UpdateBooksList: function () {
        $.getJSON("/api/books", function (data) {
            $("#booksListRows")[0].innerHTML = "";
            $("#add_name")[0].value = "";
            $("#add_publishDate")[0].value = "";
            $.tmpl("booksRowTemplate", data).appendTo("#booksListRows");
        });
    },

    AddBook: function () {
        var book = {
            Name: $("#add_name")[0].value,
            PublishDate: $("#add_publishDate")[0].value
        };

        $.ajax({
            url: '/api/books/',
            type: 'POST',
            data: JSON.stringify(book),
            contentType: "application/json",
            success: function (data) {
                alert('Book added successfully');
                bookManager.UpdateBooksList();
            },
            error: function (xhr) {
                alert('Book not added: ' + xhr.responseText);
            }
        });

        return false;
    },

    OpenForEditing: function (bookId) {
        $.ajax({
            url: '/api/books/' + bookId,
            type: 'GET',
            data: { IsForEdit: true },
            contentType: "application/json",
            success: function (data) {
                $("#editBookBlockTitle")[0].innerText = "Edit book id " + data.Id;

                $("#edit_id")[0].value = data.Id;
                $("#edit_name")[0].value = data.Name;
                $("#edit_publishDate")[0].value = data.PublishDate;

                $("#listOfBooksBlock").hide();
                $("#editBookBlock").show();

                $("#editBookForm")[0].scrollIntoView();
            },
            error: function (xhr) {
                if (xhr.statusText !== null) {
                    alert('Book not available: ' + xhr.statusText);
                } else {
                    alert('Book not available: ' + xhr.responseText);
                }
            }
        });
    },

    EditBook: function () {
        var book = {
            Id: $("#edit_id")[0].value,
            Name: $("#edit_name")[0].value,
            PublishDate: $("#edit_publishDate")[0].value
        };

        $.ajax({
            type: "PUT",
            url: "/api/books",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(book),
            success: function (data) {
                alert('Book edited successfully');
                $("#listOfBooksBlock").show();
                $("#editBookBlock").hide();
                bookManager.UpdateBooksList();
            },
            error: function (xhr) {
                if (xhr.statusText !== null) {
                    alert('Book not edited: ' + xhr.statusText);
                } else {
                    alert('Book not edited: ' + xhr.responseText);
                }
            }
        });
    },

    DeleteBook: function (bookId) {
        $.ajax({
            type: "DELETE",
            url: "/api/books/" + bookId,
            contentType: "application/json",
            dataType: "json",
            success: function (data) {
                alert('Book deleted successfully');
                document.getElementById('book' + bookId).remove();
            },
            error: function (xhr) {
                alert('Book not deleted: ' + xhr.responseText);
            }
        });
    }
};