



async function readInputRfid(EQID)
{
    try
    {
        return await fetch('http://172.15.2.31:1880/rfid/' + EQID)
            .then(response => response.json());
    }
    catch
    {
        return '';
    }
}

async function getInputPositions(EQID)
{
    try
    {
        return await fetch(window.location.origin + '/api/cockpit/inputPositions/' + EQID)
            .then(response => response.json());
    }
    catch
    {
        return '';
    }
}