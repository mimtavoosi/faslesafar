var acceptBtn = document.getElementById('acceptbtn');
var validateBtn = document.getElementById('validatebtn');
var tourId = parseInt($('#tourId').val());

if (document.baseURI.includes('AddTour')) {
    addRow();
}
// تابعی برای پر کردن جدول با داده‌های موجود
function populateTableWithData(existingData) {
    const table = document.getElementById("dynamicTable2").getElementsByTagName('tbody')[0];

    existingData.forEach(function (data) {
        var newRow = table.insertRow(table.rows.length);

        var titleCell = newRow.insertCell(0);
        var adultPriceCell = newRow.insertCell(1);
        var childPriceCell = newRow.insertCell(2);
        var babyPriceCell = newRow.insertCell(3);
        var actionCell = newRow.insertCell(4);
        var idCell = newRow.insertCell(5);

        titleCell.innerHTML = '<input type="text" value="' + data.title + '" class="form-control" required>';
        adultPriceCell.innerHTML = '<input placeholder="قیمت (به تومان)" onkeypress="return isNumber(event);" onkeyup="moneyCommaSep(this);" class="form-control" value="' + data.adultPrice + '">';
        childPriceCell.innerHTML = '<input placeholder="قیمت (به تومان)" onkeypress="return isNumber(event);" onkeyup="moneyCommaSep(this);" class="form-control" value="' + data.childPrice + '">';
        babyPriceCell.innerHTML = '<input placeholder="قیمت (به تومان)" onkeypress="return isNumber(event);" onkeyup="moneyCommaSep(this);" class="form-control"  value="' + data.babyPrice + '">';
        actionCell.innerHTML = '<a type="button" onclick="deleteRow(this)" class="delete-button btn mtgridbtn"><i class="fa fa-trash-o"></i></a>';
        idCell.innerHTML = '<input type="hidden" value="' + data.staringId + '" required>';

        const tableData = getTableData();
        document.getElementById('data-json').value = JSON.stringify(tableData);
    });
}

// دستوری برای دریافت اطلاعات موجود از سمت سرور (مثالی از نحوه دریافت داده)
fetch('/Admin/GetPricesOfTour?tourId=' + tourId)
    .then(response => response.json())
    .then(existingData => {
        populateTableWithData(existingData);
    })
    .catch(error => console.error('Error:', error));


function addRow() {
    var table = document.getElementById("dynamicTable2").getElementsByTagName('tbody')[0];;
    var newRow = table.insertRow(table.rows.length);

    var titleCell = newRow.insertCell(0);
    var adultPriceCell = newRow.insertCell(1);
    var childPriceCell = newRow.insertCell(2);
    var babyPriceCell = newRow.insertCell(3);
    var actionCell = newRow.insertCell(4);
    var idCell = newRow.insertCell(5);

    titleCell.innerHTML = '<input type="text" class="form-control" required>';
    adultPriceCell.innerHTML = '<input placeholder="قیمت (به تومان)" onkeypress="return isNumber(event);" onkeyup="moneyCommaSep(this);" class="form-control">';
    childPriceCell.innerHTML = '<input placeholder="قیمت (به تومان)" onkeypress="return isNumber(event);" onkeyup="moneyCommaSep(this);" class="form-control">';
    babyPriceCell.innerHTML = '<input placeholder="قیمت (به تومان)" onkeypress="return isNumber(event);" onkeyup="moneyCommaSep(this);" class="form-control" >';
    actionCell.innerHTML = '<a type="button" onclick="deleteRow(this)" class="delete-button btn mtgridbtn"><i class="fa fa-trash-o"></i></a>';
    idCell.innerHTML = '<input type="hidden" value="0" required>';

    startValidation();
}

function deleteRow(button) {
    var table = document.getElementById("dynamicTable2"); // اسم جدول خود را وارد کنید
    var row = button.parentNode.parentNode;
    if (table.rows.length > 2) {
        row.parentNode.removeChild(row);
        startValidation();
    }
}

function startValidation() {
    acceptBtn.setAttribute('disabled', 'true');
    validateBtn.style.display='block'
}

function stopValidation() {
    acceptBtn.removeAttribute('disabled');
    validateBtn.style.display = 'none'
}

function validateForm() {
    const requiredFields = document.querySelectorAll("#dynamicTable2 [required]");
    let isValid = true;
    var goOn1 = true;



    requiredFields.forEach(function (field) {
        if (field.value.trim() === '') {
            field.style.borderColor = 'red';
            if (goOn1 == true) {
                goOn1 = false;
            }
        } else {
            field.style.borderColor = '';
        }
    });

    isValid = writeErrors(goOn1);

    if (isValid) {
        acceptBtn.removeAttribute('disabled');
        const tableData = getTableData();
        document.getElementById('data-json').value = JSON.stringify(tableData);
        Swal.fire({
            icon: 'success',
            title: 'موفق',
            text: 'قیمت های تور با موفقیت ذخیره شدند',
            confirmButtonColor: '#00ff80',
            confirmButtonText: 'تایید',
        });
    }
    return isValid;
}

function writeErrors(goOn1) {
    var isValid = true;
    var errText = '';
    var errLbl = document.getElementById("error-message");
    if (!goOn1) {
        isValid = false;
        errText += 'لطفاً تمامی فیلدهای اجباری را پر کنید\n';
    }
    errLbl.innerText = errText;
    return isValid;
}

function getTableData() {
    const tableRows = document.querySelectorAll('#dynamicTable2 tbody tr');
    const tableData = [];

    tableRows.forEach(function (row) {
        const cells = row.querySelectorAll('td');
        const rowData = {
            title: cells[0].querySelector('input').value,
            adultPrice: cells[1].querySelector('input').value,
            childPrice: cells[2].querySelector('input').value,
            babyPrice: cells[3].querySelector('input').value,
            tourId: parseInt($('#tourId').val()),
            staringId: parseInt(cells[5].querySelector('input').value),
        };
        tableData.push(rowData);
    });

    return tableData;
}
