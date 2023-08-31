function setEmoji(leasing) {
    if (leasing == true)
        return '<span>&#9989</span';
    else return ' <span>&#10060</span>';
}
$(document).ready(function () {
        $('#searchtext').on('keyup', function () {
            var searchtext = $('#searchtext').val();
            var searchtype = $('#searchtype').val();
                $.ajax({
                    type: 'post',
                    url: '/Admin/SearchRecords?searchtext=' + searchtext + '&searchtype=' + searchtype,
                    success: function (result) {
                        $('#tbdata').html("");

                        if (result.length == 0) {
                            $('#tbdata').append("<tr><td colspan='4'> چیزی یافت نشد</td></tr>");
                        }
                        else {
                            //debugger;
                            $.each(result, function (index, value) {
                                if (searchtype == '1') {
                                    var data = "<tr><td><img src = \"/pics/destpics/" + value.bigImage + "\" class=\"dest0\" /></td>" +
                                        "<td>" + value.destinationName + "</td>" +
                                        "<td>" + setEmoji(value.isAttraction) + "</td>" +
                                        "<td>" + setEmoji(value.onVitrin) + "</td>" +
                                        "<td><a type=\"button\" class =\"btn btn-pink btn-rounded w-md waves-effect waves-light m-b-5\" href =\"/Admin/ShowDestinationAttractions/" + value.destinationId + "\"> جاذبه های گردشگری</a>" +
                                        "<a type=\"button\" class =\"btn btn-info btn-rounded w-md waves-effect waves-light m-b-5\" href =\"/Admin/ShowDestinationTours/" + value.destinationId + "\"> تورها</a>" +
                                        "<a type=\"button\" class =\"btn btn-warning btn-rounded w-md waves-effect waves-light m-b-5\" href =\"/Admin/EditDestination/" + value.destinationId + "\"> ویرایش</a>" +
                                        "<a type=\"button\" class =\"btn btn-danger btn-rounded w-md waves-effect waves-light m-b-5\" href =\"/Admin/DeleteDestination/" + value.destinationId + "\"> حذف</a></td></tr>";
                                }
                                if (searchtype == '2' || searchtype == '3' || searchtype == '6') {
                                    var data = "<tr><td><img src = \"/pics/tourpics/" + value.bigImage + "\" class=\"dest0\" /></td>" +
                                        "<td>" + value.tourTitle + "</td>" +
                                        "<td>" + value.tourType + "</td>" +
                                        "<td>" + value.transportType + "</td>" +
                                        "<td>" + value.destination.destinationName + "</td>" +
                                        "<td>" + value.capacity + "</td>" +
                                        "<td>" + value.daysCount + "</td>" +
                                        "<td>" + value.startDate + "</td>" +
                                        "<td>" + value.endDate + "</td>" +
                                        "<td>" + value.price + "</td>" +
                                        "<td>" + value.vehicle + "</td>" +
                                        "<td>" + setEmoji(value.isLeasing) + "</td>" +
                                        "<td>" + value.avgScore + "</td>" +
                                        "<td>" + value.openState + "</td>" +
                                        "<td><a type=\"button\" class =\"btn btn-info btn-rounded w-md waves-effect waves-light m-b-5\" href =\"/Admin/ShowTourUsers/" + value.tourId + "\"> خریداران</a>" +
                                        "<a type=\"button\" class =\"btn btn-warning btn-rounded w-md waves-effect waves-light m-b-5\" href =\"/Admin/EditTour/" + value.tourId + "\"> ویرایش</a>" +
                                        "<a type=\"button\" class =\"btn btn-danger btn-rounded w-md waves-effect waves-light m-b-5\" href =\"/Admin/DeleteTour/" + value.tourId + "\"> حذف</a></td></tr>";
                                }
                                if (searchtype == '4' || searchtype == '5') {
                                    var data = "<tr><td>" + value.fullName + "</td>" +
                                        "<td>" + value.email + "</td>" +
                                        "<td>" + value.mobileNumber + "</td>" +
                                        "<td>" + value.password + "</td>" +
                                        "<td>" + value.isAdmin + "</td>" +
                                        "<td><a type=\"button\" class =\"btn btn-purple btn-rounded w-md waves-effect waves-light m-b-5\" href =\"/Admin/AddMessage/" + value.userId + "\"> ارسال پیامک</a>" +
                                        "<a type =\"button\" class =\"btn btn-info btn-rounded w-md waves-effect waves-light m-b-5\" href =\"/Admin/ShowUserTours/" + value.userId + "\"> تورها</a>" +
                                        "<a type=\"button\" class =\"btn btn-pink btn-rounded w-md waves-effect waves-light m-b-5\" href =\"/Admin/ShowUserOrders/" + value.userId + "\"> خرید ها</a>" +
                                        "<a type =\"button\" class =\"btn btn-custom btn-rounded w-md waves-effect waves-light m-b-5\" href =\"/Admin/ShowUserReqTrips/" + value.userId + "\"> درخواست های سفر</a>" +
                                        "<a type =\"button\" class =\"btn btn-inverse btn-rounded w-md waves-effect waves-light m-b-5\" href =\"/Admin/ShowUserMessages/" + value.userId + "\"> پیامک ها</a>" +
                                        "<a type=\"button\" class =\"btn btn-warning btn-rounded w-md waves-effect waves-light m-b-5\" href =\"/Admin/EditUser/" + value.userId + "\"> ویرایش</a>" +
                                        "<a type=\"button\" class =\"btn btn-danger btn-rounded w-md waves-effect waves-light m-b-5\" href =\"/Admin/DeleteUser/" + value.userId + "\"> حذف</a></td></tr>";
                                }
                                if (searchtype == '7' || searchtype == '8') {
                                    var data = "<tr><td>" + value.orderId + "</td>" +
                                        "<td>" + value.tour.tourTitle + "</td>" +
                                        "<td>" + value.price + "</td>" +
                                        "<td>" + value.adultCount + "</td>" +
                                        "<td>" + value.childCount + "</td>" +
                                        "<td>" + value.babyCount + "</td>" +
                                        "<td>" + value.user.fullName + "</td>" +
                                        "<td>" + value.createDate + "</td>" +
                                        "<td>" + value.isFinaly + "</td>" +
                                        "<td><a type=\"button\" class =\"btn btn-primary btn-rounded w-md waves-effect waves-light m-b-5\" href =\"/Admin/ReadOrder/" + value.orderId + "\"> نمایش</a>" +
                                        "<a type=\"button\" class =\"btn btn-warning btn-rounded w-md waves-effect waves-light m-b-5\" href =\"/Admin/EditState/" + value.orderId + "\"> تغییر وضعیت</a>" +
                                        "<a type=\"button\" class =\"btn btn-danger btn-rounded w-md waves-effect waves-light m-b-5\" href =\"/Admin/DeleteOrder/" + value.orderId + "\"> حذف</a></td></tr>";
                                }
                                if (searchtype == '9' || searchtype == '10') {
                                    var data = "<tr><td>" + value.user.fullName + "</td>" +
                                        "<td>" + value.destinationName + "</td>" +
                                        "<td>" + value.startDate + "</td>" +
                                        "<td>" + value.endDate + "</td>" +
                                        "<td>" + value.passengersCount + "</td>" +
                                        "<td>" + value.transportType + "</td>" +
                                        "<td>" + value.createDate + "</td>" +
                                        "<td><a type=\"button\" class =\"btn btn-primary btn-rounded w-md waves-effect waves-light m-b-5\" href =\"/Admin/ReadReqTrip/" + value.reqTripId + "\"> نمایش</a>" +
                                        "<a type=\"button\" class =\"btn btn-danger btn-rounded w-md waves-effect waves-light m-b-5\" href =\"/Admin/DeleteReqTrip/" + value.reqTripId + "\"> حذف</a></td></tr>";
                                }
                                if (searchtype == '11' || searchtype == '12') {
                                    var data = "<tr><td>" + value.user.fullName + "</td>" +
                                        "<td>" + value.user.mobileNumber + "</td>" +
                                        "<td>" + value.messageText + "</td>" +
                                        "<td>" + value.sentDate + "</td>" +
                                        "<td>" + value.sentState + "</td>" +
                                        "<td><a type=\"button\" class =\"btn btn-primary btn-rounded w-md waves-effect waves-light m-b-5\" href =\"/Admin/ReadMessage/" + value.messageId + "\"> نمایش</a>" +
                                        "<a type=\"button\" class =\"btn btn-danger btn-rounded w-md waves-effect waves-light m-b-5\" href =\"/Admin/DeleteMessage/" + value.messageId + "\"> حذف</a></td></tr>";
                                }
                                if (searchtype == '13' || searchtype== '14') {
                                    var data = "<tr><td><img src = \"/pics/attrpics/" + value.bigImage + "\" class=\"dest0\" /></td>" +
                                        "<td>" + value.attractionName + "</td>" +
                                        "<td>" + value.destination.destinationName + "</td>" +
                                        "<td><a type=\"button\" class =\"btn btn-warning btn-rounded w-md waves-effect waves-light m-b-5\" href =\"/Admin/EditAttraction/" + value.attractionId + "\"> ویرایش</a>" +
                                        "<a type=\"button\" class =\"btn btn-danger btn-rounded w-md waves-effect waves-light m-b-5\" href =\"/Admin/DeleteAttraction/" + value.attractionId + "\"> حذف</a></td></tr>";
                                }
                                $('#tbdata').append(data);
                                
                            })
                        }
                    }
                })
           

        });
    });
