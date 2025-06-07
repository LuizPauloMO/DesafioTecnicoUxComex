$("#frmFilterOrder").submit(function (event) {
    event.preventDefault();

    $.ajax({
        url: "/Order/ListOrders/Filter?key=" + encodeURIComponent($("#txtFilterOrderByName").val() ?? "") + "&status=" + encodeURIComponent($("#cbOrderStatus option:selected").val() ?? ""),
        method: "GET",
        success: function (data) {
            $("#lstOrders").empty();
            console.log(data)

            var tbl = ' <table class="table table-sm table-bordered">';
            tbl += '   <tbody>'
            var TotalItems = 0;

            data.forEach(function (order) {

                //const dateOrderFormat = new Date(order.dateOrder).toLocaleString('en-US', { hourCycle: "h23" });//.replace(',', ' - ');

                const dateOrder = new Date(order.dateOrder);
                const day = dateOrder.getDate().toString().padStart(2, '0');
                const month = dateOrder.getMonth().toString().padStart(2, '0');
                const year = dateOrder.getFullYear();

                const hour = dateOrder.getHours().toString().padStart(2, '0')
                const min = dateOrder.getMinutes().toString().padStart(2, '0')
                const sec = dateOrder.getSeconds().toString().padStart(2, '0')

                const dateOrderFormat = `${year}/${month}/${day} at ${hour}:${min}:${sec}`;

                tbl += '<tr>';
                tbl += '    <td class="w-50 text-center">' + order.clientName +'</td>';
                tbl += '    <td class="text-center" style="width:220px;font-size:12px;">' + dateOrderFormat + '</td>';
                tbl += '    <td class="text-center" style="width:230px;">' + order.totalValue + '</td>';
                tbl += '    <td class="text-center" style="width:230px;">' + order.status + '</td>';
                tbl += '    <td class="p-0 text-center" style="width:150px;">';
                tbl += '        <div class="d-inline-flex col align-content-center">';
                tbl += '            <div class="col-sm-auto p-1">';
                tbl += '                <form method="POST" action="/Order/UpdateOrder?Id=' + order.id + '">';
                tbl += '                    <input type="submit" value="Update" class="btn-primary form-control form-control-sm m-0 text-decoration-none" />';
                tbl += '                </form>';
                tbl += '            </div>';
                tbl += '        </div>';
                tbl += '    </td>';
                tbl += '</tr>';

                TotalItems++;
            })

            tbl += '  </tbody>';
            tbl += '  </table>';

            $("#lstOrders").html(tbl);
            $("#spTotalItems").text(TotalItems)
        },
        error: function (xhr, status, error) {
            console.error('Erro na requisição:', error);
        }
    });

});