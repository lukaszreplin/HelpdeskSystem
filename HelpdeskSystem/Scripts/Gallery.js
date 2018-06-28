﻿var modal = document.getElementById('imgModal');

var img = document.getElementById('attachmentImg');
var modalImg = document.getElementById("img01");
var captionText = document.getElementById("caption");
img.onclick = function () {
    modal.style.display = "block";
    modalImg.src = this.src;
    captionText.innerHTML = this.alt;
}

var span = document.getElementsByClassName("close")[0];

document.getElementById('x').onclick = function () {
    modal.style.display = "none";
}
document.getElementById('img01').onclick = function () {
    modal.style.display = "none";
}