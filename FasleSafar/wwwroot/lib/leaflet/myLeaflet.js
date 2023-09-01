var geo = $('#geo').val();
var geoarr = geo.split(',');
var longitude = parseFloat(geoarr[0]);
var latitude = parseFloat(geoarr[1]);
var mymap = L.map('map').setView([longitude, latitude], 25);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/">OpenStreetMap</a> contributors'
    }).addTo(mymap);
L.marker([longitude, latitude]).addTo(mymap);