

//document.addEventListener('readystatechange', event => {
//    switch (document.readyState) {
//        case "loading":
//            break;
//        case "interactive":
//            refreshGui();
//            break;
//        case "complete":
//            setInterval(refreshGui, 5000);
//            break;
//    }
//});

async function refreshGui() {
    var tbmProd = await CurrentTBMProduction();
    var tbmPlan = await CurrentTBMPlan();
    var tbmProc = Math.round((tbmProd / tbmPlan) * 100);

    document.getElementById('tbmPlanProd').innerHTML = tbmProd + ' / ' + tbmPlan;
    document.getElementById('progressTbm').style.width = tbmProc + '%';

    var tirePlan = await CurrenTirePlan();
    var tireProd = await CurrentTireProduction();
    var tireProc = Math.round((tireProd / tirePlan) * 100);

    document.getElementById('tirePlanProd').innerHTML = tireProd + ' / ' + tirePlan;
    document.getElementById('progressTire').style.width = tireProc + '%';

}

async function CurrenTirePlan()
{
    return await fetch(window.location.origin + '/api/prod/CUREPlanCurrent/')
        .then(response => response.json());
}

async function CurrentTBMPlan()
{
   // var planLabel = document.getElementById('currentTBMPlan');

    return await fetch(window.location.origin + '/api/prod/TBMPlanCurrent/')
        .then(response => response.json());
       // .then(data => planLabel.innerHTML = data + ' / ');
}

async function CurrentTBMProduction() {
    //var planLabel = document.getElementById('currentTBMProd');

    return await fetch(window.location.origin + '/api/prod/CurrentProd/T')
        .then(response => response.json());
        
}

async function CurrentTireProduction() {
    //var planLabel = document.getElementById('currentTBMProd');

    return await fetch(window.location.origin + '/api/prod/CurrentProd/U')
        .then(response => response.json());

}