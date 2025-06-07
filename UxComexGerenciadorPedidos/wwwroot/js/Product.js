
    $("#frmFilterProduct").submit(function (event) {
        event.preventDefault();

        $.ajax({
            url: "/Product/ListProducts/Filter?name=" + encodeURIComponent($("#inputFilterProductByName").val() ?? ""),
            method: "GET",
            success: function (data) {
                $("#lstProducts").empty();

                var tbl = '<table class="table table-sm table-bordered" id = "tbl">';
                tbl += '   <tbody>'
                var TotalItems = 0;

                data.forEach(function (product) {
                    tbl += '    <tr>';
                    tbl += '        <td class="text-center" style="width:400px;">' + product.name + '</td>';
                    tbl += '        <td class="text-center" style="width:400px;">' + product.description + '</td>';
                    tbl += '        <td class="text-center" style="width:100px;">' + product.price.toLocaleString("pt-BR", { style: "currency", currency: "BRL" }) + '</td>';
                    tbl += '        <td class="text-center" style="width:200px;">' + product.quantityOfStock.toLocaleString("pt-BR", { style: "currency", currency: "BRL" }) + '</td>';

                    tbl += '        <td class="p-0 text-center" style="width:150px;">';
                    tbl += '            <div class="d-inline-flex col align-content-center">';
                    tbl += '                <div class="col-sm-auto p-1">';
                    tbl += '                    <form method="post" action = "/Product/Update?Id=' + product.id + '">';
                    tbl += '                         <input type = "submit" value = "Update" class="btn-primary form-control form-control-sm m-0 text-decoration-none" id = "btnFilterProductByName" /> ';
                    tbl += '                    </form>';
                    tbl += '                </div>';
                    tbl += '                <div class="col-sm-auto p-1"> ';
                    tbl += '                    <form method = "post" action = "/Product/Delete?Id=' + product.id + '">';
                    tbl += '                         <input type = "submit" value = "Delete" class="btn-danger form-control form-control-sm m-0 text-decoration-none" id = "btnFilterProductByName" /> ';
                    tbl += '                    </form>';
                    tbl += '                </div>';
                    tbl += '            </div>';
                    tbl += '        </td>';
                    tbl += '    </tr>';
                    TotalItems++;
                })

                tbl += '  </tbody>';
                tbl += '  </table>';

                $("#lstProducts").html(tbl);
                $("#spTotalItems").text(TotalItems)
            },
            error: function (xhr, status, error) {
                console.error('Erro na requisição:', error);
            }
        });

    });