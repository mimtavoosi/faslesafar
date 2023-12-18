var count = document.getElementById('count');
var oneprice = document.getElementById('oneprice');
var maliat = document.getElementById('maliat');
var totalprice = document.getElementById('totalprice');
var leasingval = $('#leasingval').val();;
var factorreqval = $('#factorreqval').val();;
var priceId = $('#priceId').val();
function UpdatePrice(input) {
    var adult = $('#input-adult').val();
    var child = $('#input-childrens').val();
    var baby = $('#input-babies').val();
        $.ajax({
            type: 'Post',
            url: '/Card/ChangePrice?adult=' + adult + '&child=' + child + '&baby=' + baby + '&priceId=' + priceId, // we are calling json method
            success: function (state) {
                // states contains the JSON formatted list
                // of states passed from the controller
                if (leasingval != 'on') {
                    if (state != undefined && state != null) {
                        if (child > 0 || baby> 0) {
                            if (child > 0 && baby <= 0) {
                                count.innerText = state.adultCount + " بزرگسال و " + state.childCount + " کودک زیر 12 سال";
                            }
                            if (baby > 0 && child <= 0) {
                                count.innerText = state.adultCount + " بزرگسال و " + state.babyCount + " کودک زیر 2 سال";
                            }
                            if (child > 0 && baby > 0) {
                                count.innerText = state.adultCount + " بزرگسال و " + state.childCount + " کودک زیر 12 سال و " + state.babyCount + " کودک زیر 2 سال";
                            }
                        }
                        else {
                            count.innerText = state.adultCount + " بزرگسال";
                        }
                        oneprice.innerText = state.onePrice + " تومان";
                        if (factorreqval == 'on') {
                            maliat.innerText = state.maliat + " تومان";
                        }
                            totalprice.innerText = state.totalPrice + " تومان";
                    }
                }
                else {
                    var deposit = document.getElementById('depositlbl');
                    if (state != undefined && state != null) {
                        oneprice.innerText = state.onePrice + " تومان";
                        deposit.innerText = state.finPrice + " تومان";
                        if (factorreqval == 'on') {
                            maliat.innerText = state.maliat + " تومان";
                        }
                        totalprice.innerText = state.totalPrice + " تومان";
                        if (child > 0 || baby > 0) {
                            if (child > 0 && baby <= 0) {
                                count.innerText = state.adultCount + " بزرگسال و " + state.childCount + " کودک زیر 12 سال";
                            }
                            if (baby > 0 && child <= 0) {
                                count.innerText = state.adultCount + " بزرگسال و " + state.babyCount + " کودک زیر 2 سال";
                            }
                            if (child > 0 && baby > 0) {
                                count.innerText = state.adultCount + " بزرگسال و " + state.childCount + " کودک زیر 12 سال و " + state.babyCount + " کودک زیر 2 سال و ";
                            }
                        }
                        else {
                            count.innerText = state.adultCount + " بزرگسال";
                        }
                    }
                }
               

            },
            error: function (ex) {
                alert('Failed to retrieve name.' + ex);
            }
        });
}