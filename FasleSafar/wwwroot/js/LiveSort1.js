function fixPrice(price) {
    if (price == '0') return '<label>تعیین نشده</label>'
    else return '<label>از&nbsp;</label><label>' + price + '</label><span>تومان</span > ';
}
function fixRate(avgScore) {
    var strrate = avgScore.toString();
    var fixedrate = strrate.replace(".", "-");
    return fixedrate;

}
$(document).ready(function () {
    //Dropdownlist Selectedchange event
    $("#input-sort1").change(function () {
        var sortpage = $('#sortpage').val();
        var sorttype = $('#input-sort1').val();
        $("#toursdiv1").empty();
        $.ajax({
            type: 'POST',
            url: '/Home/SortTours?sortType=' + sorttype + '&sortPage=' + sortpage, // we are calling json method
            success: function (states) {
                // states contains the JSON formatted list
                // of states passed from the controller

                $.each(states, function (i, state) {
                    $("#toursdiv1").append(
                        '<div class="product-layout col-lg-6 col-md-6 col-sm-6 col-xs-6"><div class= "product-item-container item" ><div class="item-block so-quickview"><div class="image"><a href="/Home/ViewTour/' + state.tourId + '" target="_self"><img src="/pics/tourpics/' + state.bigImage + '\" alt=\"' + state.tourTitle + ' "class="img-responsive tour1"></a></div><div class="item-content clearfix"><h3><a href="/Home/ViewTour/' + state.tourId + '">' + state.tourTitle + '</a></h3><div class="reviews-content"><div class="star"><span class="star' + fixRate(state.avgScore) + '"></span></div><a href="/Home/ViewTour/' + state.tourId + '" class="review-link" rel="nofollow">(' + state.scoreCount + ' رأی)</a></div><ul><li><i class="fa fa-lock-o" aria-hidden="true"></i> ' + state.daysCount + ' روز</li><li><i class="fa fa-user-circle" aria-hidden="true"></i> ' + state.capacity + ' نفر</li><li><i class="fa fa-calendar" aria-hidden="true" ></i>' + state.startDate + '</li></ul><div class="des">' + state.tourDescription + '</div><div class="item-bot clearfix"><div class="price pull-left">' + fixPrice(state.price) + '</div><a href="/Home/ViewTour/' + state.tourId + '" class="book-now pull-right">مشاهده تور</a></div></div></div></div></div>'
                    );
                    // here we are adding option for States
                });

            },
            error: function (ex) {
                alert('Failed to retrieve list.' + ex);
            }
        });
        return false;
    })
});