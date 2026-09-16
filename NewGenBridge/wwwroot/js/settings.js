let passwordButton = document.getElementById('passwordButton');
passwordButton.addEventListener('click', (e) => {
    let value1 = document.getElementById('labelC').value;
    let value2 = document.getElementById('labelN').value;
    let warning = document.querySelector('.form-validation');
    if (value1 != value2) {
        e.preventDefault();
        warning.innerText = 'Passwords do not match';
        warning.style.display = 'block';
    }
    else {
        warning.style.display = 'none';
    }
});

let pictureInputButton = document.querySelector(".profile-image-input");
let pictureInput = document.querySelector("[name='picture']");
pictureInputButton.addEventListener('click', () => {
    $(pictureInput).trigger('click');
});

pictureInput.addEventListener('change', () => { 
    let value = pictureInput.value;
    document.querySelector('.profile-image-name').innerText = value.split('\\').pop();
    document.querySelector('.profile-image-warning').style.display = 'none';
});
let profileFormButton = document.querySelector(".profile-form-button");
let profileForm = document.querySelector(".profile-image-form");
profileFormButton.addEventListener('click', (e) => {
    e.preventDefault();
    if(pictureInput.value == ''){
        location.reload();
    }
    else{
        $(profileForm).trigger('submit');
    }
});