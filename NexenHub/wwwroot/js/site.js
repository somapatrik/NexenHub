

/*const CHART_COLORS = {
    red: 'rgb(255, 99, 132)',
    orange: 'rgb(255, 159, 64)',
    yellow: 'rgb(255, 205, 86)',
    green: 'rgb(75, 192, 192)',
    blue: 'rgb(54, 162, 235)',
    purple: 'rgb(153, 102, 255)',
    grey: 'rgb(201, 203, 207)',
    primary: 'rgb(11,94,215)',
    warning: 'rgb(255,193,7)'
};*/

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

        /*if (searchVal.length == 15 || searchVal.length == 5) {
            window.location.href = "/lot/" + searchVal;
        }*/
    }
}
