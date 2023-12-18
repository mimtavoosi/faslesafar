updatePriceOption();

function updatePriceOption() {
    var realprice1 = document.getElementById('realprice1');
    var realprice2 = document.getElementById('realprice2');
    var realprice3 = document.getElementById('realprice3');

    $.ajax({
        type: 'Post',
        url: '/Home/UpdatePrice', // we are calling json method
        //dataType: 'json',
        data: { priceId: $("#price").val() },
        success: function (state) {
            // states contains the JSON formatted list
            // of states passed from the controller
            if (state != undefined) {
                realprice1.innerText = state.adultPrice;
                realprice2.innerText = state.childPrice;
                realprice3.innerText = state.babyPrice;
            }

        },
        error: function (ex) {
            alert('Failed to retrieve name.' + ex);
        }
    });
}