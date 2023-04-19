﻿const charcolors = [
    'rgb(75, 192, 192)',
    'rgb(54, 162, 235)',
    'rgb(153, 102, 255)',
    'rgb(201, 203, 207)',
    'rgb(11,94,215)',
    'rgb(255,193,7)',
    'rgb(255, 99, 132)',
    'rgb(255, 159, 64)',
    'rgb(255, 205, 86)',
];

const chartOkColor = ['rgb(75, 192, 192,.7)', 'rgb(75, 192, 192,1)'];
const chartBadColor = ['rgb(255, 99, 132,.7)', 'rgb(255, 99, 132,1)'];
const chartGrayColor = ['rgb(201, 203, 207,.7)', 'rgb(201, 203, 207,1)'];
const chartOrangeColor = ['rgb(255, 159, 64,.7)', 'rgb(255, 159, 64,1)'];
const chartPmColor = ['rgb(255, 193, 7,.7)', 'rgb(255, 193, 7,1)'];

function SetPageTitle(title) {
    document.title = title;
}

function searchMat(x, e)
{

    if (x.keyCode == 13)
    {
        var lotReg = /^\d{11}[a|c]\d{3}$/i;
        var cartReg = /^[a|b|c|d|e|t|w]\d{4}$/i;
        var tireReg = /^40\d{8}$/i;

        var searchVal = e.value;
        var lotResult = lotReg.test(searchVal);
        var cartResult = cartReg.test(searchVal);
        var tireResult = tireReg.test(searchVal);



        if (lotResult || cartResult)
            window.location.href = "/lot/" + searchVal;
        else if (tireResult)
            window.location.href = "/tire/" + searchVal;

    }
}

function toogleFullscreen(e)
{
    e.classList.toggle('fullscreen');
}
