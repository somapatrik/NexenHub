



async function readInputRfid(EQID)
{
    var data = await fetch('http://172.15.2.31:1880/rfid/' + EQID)
        .then(response => response.json());

    return data;
}