$(document).ready(function () {
    //Dropdownlist Selectedchange event
    var realprice = document.getElementById('realprice');
    $("#price").change(function () {
       /* $("#toursdiv1").empty();*/
        $.ajax({
            type: 'Post',
            url: '/Home/UpdatePrice', // we are calling json method
            //dataType: 'json',
            data: { priceId: $("#price").val() },
            success: function (state) {
                // states contains the JSON formatted list
                // of states passed from the controller
                if (state != undefined) {
                    realprice.innerText = state;
                }

            },
            error: function (ex) {
                alert('Failed to retrieve name.' + ex);
            }
        });
        return false;
    })
});