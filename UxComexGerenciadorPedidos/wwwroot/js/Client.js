$("#frmFilterClient").submit(function (event) {
    event.preventDefault();

    $.ajax({
        url: "/Client/ListClients/Filter?name=" + encodeURIComponent($("#inputFilterClientByNameOrEmail").val() ?? ""),
        method: "GET",
        success: function (data) {
            $("#lstClients").empty();

            var tbl = '<table class="table table-sm table-bordered" id="tbl">';
            tbl += '   <tbody>'
            var TotalItems = 0;

            data.forEach(function (client) {
                tbl += '<tr>';
                tbl += '    <td class="text-center" style="width:320px;">' + client.name + '</td>';
                tbl += '    <td class="text-center" style="width:320px;">' + client.email + '</td>';
                tbl += '    <td class="text-center" style="width:150px;">' + client.telephone + '</td>';
                tbl += '    <td class="text-center" style="width:250px;">' + client.dateRegister + '</td>';
                tbl += '    <td class="p-0s text-center" style="width:150px;">';
                tbl += '        <div class="d-inline-flex col align-content-center">';
                tbl += '            <div class="col-sm-auto p-1">';
                tbl += '                <form method="post" action="/Client/UpdateClient?Id=' + client.id + '">';
                tbl += '                    <input type="submit" value="Update" class="btn-primary form-control form-control-sm m-0 text-decoration-none" />';
                tbl += '                </form>';
                tbl += '            </div>';
                tbl += '            <div class="col-sm-auto p-1">';
                tbl += '                <form method="post" action="/Client/Delete?Id=' + client.id + '">';
                tbl += '                    <input type="submit" value="Delete" class="btn-danger form-control form-control-sm m-0 text-decoration-none" />';
                tbl += '                </form>';
                tbl += '            </div>';
                tbl += '        </div>';
                tbl += '    </td>';
                tbl += '</tr>';
                TotalItems++;
            })

            tbl += '  </tbody>';
            tbl += '  </table>';

            $("#lstClients").html(tbl)
            $("#spTotalItems").text(TotalItems)
        },
        error: function (xhr, status, error) {
            console.error('Erro na requisição:', error);
        }
    });
});

$("#frmNewClient").submit(function (event) {
    event.preventDefault();

    var json = {
        name: $("#inputClientFullName").val(),
        email: $("#inputClientEmail").val(),
        telephone: $("#inputClientTelephone").val()
    };
    alert(JSON.stringify(json))

    $.ajax({
        url: "/Client/New",
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify(json),
        success: function (data) {
            console.log(JSON.stringify(data));
        },
        error: function () {

        }
    });
});