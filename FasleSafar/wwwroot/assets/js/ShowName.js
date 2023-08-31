var mobLbl = document.getElementById('showMobileNumber');
function ShowMobileNumber(input) {
    if (input.value.length < 11) {
        //  debugger;
        mobLbl.innerText = "";
    }
    else {
        $.ajax({
            type: 'Post',
            url: '/Admin/GetUserNameByMobileNumber', // we are calling json method
            //dataType: 'json',
            data: { mobileNumber: $("#mobileNumber").val() },
            success: function (state) {
                // states contains the JSON formatted list
                // of states passed from the controller
                if (state != undefined) {
                    mobLbl.innerText = state;
                }

            },
            error: function (ex) {
                alert('Failed to retrieve name.' + ex);
            }
        });
    }
}