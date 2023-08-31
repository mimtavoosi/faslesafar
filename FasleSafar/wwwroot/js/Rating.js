var rating1 = document.getElementById("rating1");
var rating2 = document.getElementById("rating2");
var rating3 = document.getElementById("rating3");
var rating4 = document.getElementById("rating4");
var rating5 = document.getElementById("rating5");
var ratebox = document.getElementById("ratebox");

function hideratebox() {
    ratebox.style.display = 'none';
}

function hideBox() {
    const box3 = document.getElementById('result');
    box3.style.display = 'none';
}

function showBox() {
    const box3 = document.getElementById('result');
    box3.style.display = 'block';
}
//دقیقه

function startTimer(timer) {
     var ptimer = timer;
     var timerOn = setInterval(function () {
        if (--ptimer < 0) {
            hideBox();
            clearInterval(timerOn);
        }
    }, 1000);
    return false;
}

function DoRating(rate) {
    var tourId = $('#tourId').val();
    $.ajax({
        type: 'Post',
        url: '/Home/Rating?tourId=' + tourId + '&rating=' + rate, // we are calling json method
        success: function (state) {
            hideratebox();
        },
        error: function (ex) {
            alert('Failed to retrieve name.' + ex);
        }
    });
}
rating1.addEventListener('click', () => {
    DoRating($('#rating1').val());
    showBox();
    var timer = 3, minutes, seconds;
    startTimer(timer);
});
rating2.addEventListener('click', () => {
    DoRating($('#rating2').val());
    showBox();
    var timer = 3, minutes, seconds;
    startTimer(timer);
});
rating3.addEventListener('click', () => {
    DoRating($('#rating3').val());
    showBox();
    var timer = 3, minutes, seconds;
    startTimer(timer);
});
rating4.addEventListener('click', () => {
    DoRating($('#rating4').val());
    showBox();
    var timer = 3, minutes, seconds;
    startTimer(timer);
});
rating5.addEventListener('click', () => {
    DoRating($('#rating5').val());
    showBox();
    var timer = 3, minutes, seconds;
    startTimer(timer);
});
