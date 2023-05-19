const charcolors = [
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

const chartOkColor = ['rgb(75, 192, 192)', 'rgb(75, 192, 192,.7)'];
const chartBadColor = ['rgb(255, 99, 132,.7)', 'rgb(255, 99, 132,1)'];
const chartGrayColor = ['rgb(201, 203, 207)', 'rgb(201, 203, 207,.7)', 'rgb(201, 203, 207,.3)'];
const chartOrangeColor = ['rgb(255, 159, 64,.7)', 'rgb(255, 159, 64,1)'];
const chartPmColor = ['rgb(255, 193, 7,.7)', 'rgb(255, 193, 7,1)'];
const chartVioletColor = ['rgb(153, 0, 204,.7)', 'rgb(153, 0, 204)'];
const chartCalenderColor = ['rgb(255, 255, 0)', 'rgb(255, 255, 0,.5)', 'rgb(255, 255, 0,.3)'];
const chartShitColor = ['rgb(153, 102, 51)', 'rgb(153, 102, 51,.7)','rgb(153, 102, 51,.3)'];
const chartExtColor = ['rgb(14, 95, 198)', 'rgb(14, 95, 198,.7)', 'rgb(14,95,198,.3)'];
const chartBeadColor = ['rgb(223, 126, 0)', 'rgb(223, 126, 0,.7)', 'rgb(223, 126, 0,.3)'];
const chartNexenColor = ['rgb(157, 28, 157)', 'rgb(157, 28, 157,.7)', 'rgb(157, 28, 157,.3)'];

async function GetFromAPI(link) {
    return await fetch(window.location.origin + link)
        .then(response => response.json());
}

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
