const checkbox = document.getElementById("agree");

checkbox.addEventListener("change", function () {
    if (this.checked == false) {
        document.getElementById("acceptbtn").setAttribute("disabled", "disabled");
    }
    else {
        validateForm();
    }

});

const state = {
    adultCount: 1,
    childrenCount: 0,
    babyCount: 0,
};

function updateForm() {
    let adultCount = parseInt(document.getElementById("input-adult").value);
    if (adultCount < 1) {
        adultCount = 1;
        document.getElementById("input-adult").value = adultCount;
    }

    const childrenCount = parseInt(document.getElementById("input-childrens").value);
    const babyCount = parseInt(document.getElementById("input-babies").value);

    updateRows(adultCount, state.adultCount, "بزرگسال");
    updateRows(childrenCount, state.childrenCount, "کودک زیر 12 سال");
    updateRows(babyCount, state.babyCount, "کودک زیر 2 سال");

    state.adultCount = adultCount;
    state.childrenCount = childrenCount;
    state.babyCount = babyCount;

    validateForm();
}

function updateRows(count, currentState, ageGroup) {
    const tbody = document.querySelector("#dynamic-table tbody");
    const numRows = tbody.childElementCount;

    if (count > currentState) {
        const numToAdd = count - currentState;
        for (let i = 0; i < numToAdd; i++) {
            const newRow = `
        <tr id="row-${numRows + i}" data-age-group="${ageGroup}">
          <td>${ageGroup}</td>
          <td><input class="form-control" type="text" placeholder="نام" required></td>
          <td><input class="form-control" type="text" placeholder="نام خانوادگی" required></td>
          <td><input class="form-control" type="tel" onkeypress="return isNumber(event);" pattern="\d+" placeholder="کد ملی" required></td>
          <td><input class="form-control" type="tel" onkeypress="return isNumber(event);" pattern="\d+" placeholder="شماره تلفن" required></td>
          <td><input class="form-control datepicker" type="text" placeholder="تاریخ تولد" required></td>
          <td><input class="form-control" type="text" placeholder="میزان تحصیلات" required></td>
          <td><input class="form-control" type="text" placeholder="شغل" required></td>
          <td><input class="form-control" type="text" placeholder="سابقه بیماری خاص"></td>
        </tr>`;
            

            tbody.insertAdjacentHTML("beforeend", newRow);

            applyDatepickerToNewRows();
        }
    } else if (count < currentState) {
        const numToRemove = currentState - count;
        for (let i = 0; i < numToRemove; i++) {
            const lastRowId = `row-${numRows - 1}`;
            const rowToRemove = document.getElementById(lastRowId);
            if (rowToRemove.getAttribute("data-age-group") === ageGroup) {
                rowToRemove.remove();
                numRows--; // کاهش تعداد ردیف‌ها برای شمارش در افزودن ردیف جدید
            } else {
                break;
            }
        }
    }
}

function validateForm() {
    const requiredFields = document.querySelectorAll("#dynamic-table [required]");
    let isValid = true;

  requiredFields.forEach(function (field) {
        if (field.value.trim() === '') {
          field.style.borderColor = 'red';
          isValid = false;
        } else {
          field.style.borderColor = '';
        }
      });

    if (isValid) {
        document.getElementById("error-message").style.display = "none";
        document.getElementById("acceptbtn").removeAttribute("disabled");
        const tableData = getTableData();
        document.getElementById('data-json').value = JSON.stringify(tableData);
    } else {
        document.getElementById("error-message").style.display = "block";
        document.getElementById("acceptbtn").setAttribute("disabled", "disabled");
        checkbox.checked = false;
    }
}

function applyDatepickerToNewRows() {
    $('.datepicker').each(function () {
        if (!$(this).hasClass('hasDatepicker')) {
            $(this).addClass('tour-search-input tarikh hasDatepicker');
            $(this).persianDatepicker({
                altFormat: "X",
                format: "D MMMM YYYY",
                observer: true,
                autoClose: true
            });
        }
    });
}

function getTableData() {
    const tableRows = document.querySelectorAll('#dynamic-table tbody tr');
    const tableData = [];

    tableRows.forEach(function (row) {
        const cells = row.querySelectorAll('td');
        const rowData = {
            ageGroup: cells[0].innerText,
            firstName: cells[1].querySelector('input').value,
            lastName: cells[2].querySelector('input').value,
            nationalCode: cells[3].querySelector('input').value,
            phoneNumber: cells[4].querySelector('input').value,
            birthDate: cells[5].querySelector('input').value,
            educationLevel: cells[6].querySelector('input').value,
            job: cells[7].querySelector('input').value,
            specialDisease: cells[8].querySelector('input').value,
        };
        tableData.push(rowData);
    });

    return tableData;
}

applyDatepickerToNewRows();
// اجرای تابع برای ایجاد جدول با مقادیر پیش‌فرض
updateForm();
