var copy = document.getElementById("copy");
function DoCopy() {
    var dummy = document.createElement('input'),
        text = window.location.href;

    document.body.appendChild(dummy);
    dummy.value = text;
    dummy.select();
    document.execCommand('copy');
    document.body.removeChild(dummy);
}
copy.addEventListener('click', () => {
    DoCopy();
});