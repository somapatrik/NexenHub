

document.addEventListener('readystatechange', event => {
    switch (document.readyState) {
        case "loading":
            break;
        case "interactive":
            refreshGui();
            break;
        case "complete":
            setInterval(refreshGui, 10000);
            break;
    }
});

async function refreshGui() {
    await CurrentTBMProduction();
    await CurrentTBMPlan();
}

async function CurrentTBMPlan()
{
    var planLabel = document.getElementById('currentTBMPlan');

    await fetch(window.location.origin + '/api/prod/TBMPlanCurrent/')
        .then(response => response.json())
        .then(data => planLabel.innerHTML = data);
}

async function CurrentTBMProduction() {
    var planLabel = document.getElementById('currentTBMProd');

    await fetch(window.location.origin + '/api/prod/CurrentProd/T')
        .then(response => response.json())
        .then(data => planLabel.innerHTML = data);
}