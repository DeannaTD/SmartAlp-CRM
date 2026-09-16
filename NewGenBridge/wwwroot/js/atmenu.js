var checkCells = document.querySelectorAll('td.presence-check');
checkCells.forEach(cell => {
    cell.addEventListener('click', () => {
        let state = cell.getAttribute('data-state');
        state = (state + 1) % 3;
        cell.setAttribute('data-state', state);
    });
});

var sendButton = document.querySelector('.action-button#attendance-save');
sendButton.addEventListener('click', (e) => {
    e.preventDefault();
    let data = [];
    let rows = document.querySelectorAll('tr:not(:first-child)');
    rows.forEach(row => {
        let id = row.getAttribute('data-id');
        let cells = row.querySelectorAll('td.presence-check');
        cells.forEach(cell => {
            let state = cell.getAttribute('data-state');
            let cellId = cell.getAttribute('data-id');
            let date = cell.getAttribute('data-stamp');
            data.push({ StudentId: id, State: state, Id: cellId, Stamp: date });
        });
    });
    let action = sendButton.getAttribute('data-action');
    $.ajax({
        type: 'POST',
        url: action,
        ataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(data),
        beforeSend: function () {
            e.currentTarget.disabled = true;
            e.currentTarget.classList.add('spin');
        },
        success: function (result) {
            e.target.disabled = false;
            e.target.classList.remove('spin');
            let calert = new AlertBox('Saved!', true);
            if (result > 0) {
            }
        },
        error: function(){
            e.target.disabled = false;
            e.target.classList.remove('spin');
            let calert = new AlertBox('Something went wrong', true);
            calert.show();
        }
    });
});