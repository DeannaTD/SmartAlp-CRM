let menu;
window.addEventListener('load', () => {
    menu = document.querySelector('nav.primary-nav');
    document.querySelector('.nav-toggler').addEventListener('click', () => {
        menu.classList.toggle('hidden');
    });

    // Dropdown menu functionality
    const dropdownToggles = document.querySelectorAll('.dropdown-toggle');
    dropdownToggles.forEach(toggle => {
        toggle.addEventListener('click', (e) => {
            e.preventDefault();
            const parentItem = toggle.closest('.menu-item.has-dropdown');
            parentItem.classList.toggle('open');
        });
    });
});

function closePopUp(id) {
    let popUp = document.getElementById(id);
    popUp.innerHTML = "";
}
function closePopUpUpdate(id) {
    let popUp = document.getElementById(id);
    popUp.innerHTML = "";
    location.reload();
}


function updateOnSuccess(message) {
    if (message) alert(message);
    else alert("Success!");
    location.reload();
}

function updateOnError(message) {
    if (message) alert(message);
    else alert("Something went wrong!");
    location.reload();
}

function submitFormSpin(target){
    let btn = target.querySelector('.spinner-button');
    btn.disabled = true;
    btn.classList.add('spin');
}

function endFormSpin(target, update, message){
    let btn = target.querySelector('.spinner-button');
    btn.disabled = false;
    btn.classList.remove('spin');
    if(update){
        updateOnSuccess(message);
    }
}

class AlertBox{
    constructor(message, isReload, isInfo){
        this.box = document.createElement('div');
        this.wrapper = document.createElement('div');
        this.text = document.createElement('p');
        this.button = document.createElement('button');
        this.wrapper.classList.add('alert-wrapper');
        this.box.classList.add('alert-box');
        this.text.innerText = message;
        this.button.innerText = 'Ok';
        this.wrapper.appendChild(this.box);
        this.box.appendChild(this.text);
        if (!isInfo) {
            this.box.appendChild(this.button);
        }
        this.button.addEventListener('click', ()=>{
            this.box.classList.remove('show');
            setTimeout(() => {
                this.wrapper.remove();
                if(isReload) location.reload();
            }, 500);
        });
    }

    show() {
        document.body.appendChild(this.wrapper);
        setTimeout(()=>{this.box.classList.add('show')}, 50);
    }

    close() {
        this.wrapper.remove();
    }

    update(message) {
        this.text.innerText = message;
    }

    makeButton() {
        this.box.appendChild(this.button);
    }
}

class ModalBox {
    constructor(options, offsetX, offsetY) {
        this.box = document.createElement('div');
        this.box.classList.add('modal-box'); //display: absolute; z-index: 1000;
        this.box.style.top = offsetY + 'px';
        this.box.style.left = offsetX + 'px';

        this.list = options.content.cloneNode(true);
        this.box.appendChild(this.list);
        this.box.addEventListener('click', (e) => { e.stopPropagation(); });
    }

    show() {
        document.body.appendChild(this.box);
    }

    close() {
        this.box.remove();
    }
}

function goToDataLink(element) {
    console.log(element);
    let link = element.getAttribute('data-link');
    if (link) {
        window.location.href = link;
    }
}