web3.eth.defaultAccount = web3.eth.accounts[0];

    

var ContractAbi = web3.eth.contract([{ 'constant': false, 'inputs': [{ 'name': '_id', 'type': 'uint256' }, { 'name': '_fileHash', 'type': 'string' }, { 'name': '_date', 'type': 'string' }], 'name': 'AddFiles', 'outputs': [{ 'name': 'success', 'type': 'bool' }], 'payable': false, 'stateMutability': 'nonpayable', 'type': 'function' }, { 'anonymous': false, 'inputs': [{ 'indexed': false, 'name': 'ID', 'type': 'uint256' }, { 'indexed': false, 'name': 'FILEHASH', 'type': 'string' }, { 'indexed': false, 'name': 'DATE', 'type': 'string' }], 'name': 'FileUploadEvent', 'type': 'event' }]);


const DecoderAbi = [{ 'inputs': [{ 'name': '_id', 'type': 'uint256' }, { 'name': '_fileHash', 'type': 'string' }, { 'name': '_date', 'type': 'string' }], 'name': 'AddFiles', 'outputs': [{ 'name': 'success', 'type': 'bool' }], 'payable': false, 'stateMutability': 'nonpayable', 'type': 'function' }, { 'anonymous': false, 'inputs': [{ 'indexed': false, 'name': 'ID', 'type': 'uint256' }, { 'indexed': false, 'name': 'FILEHASH', 'type': 'string' }, { 'indexed': false, 'name': 'DATE', 'type': 'string' }], 'name': 'FileUploadEvent', 'type': 'event' }];

abiDecoder.addABI(DecoderAbi);


var datalistdecrypted = [];

var images = [{}];
var jsonImage = new Array();


//    var block = web3.eth.getBlock(617);
//    var reciept = web3.eth.getTransactionReceipt(block["transactions"][0]);
//    if (reciept["logs"].length != 0) {
//        var ndatalistdecrypted = abiDecoder.decodeLogs(reciept["logs"]);
//        var nimages = ndatalistdecrypted[0]["events"];

//        if (nimages[0]["value"] == $("#id").val()) {
//            jsonImage.push(new Object({
//                id: nimages[0]["value"],
//                fileid: nimages[1]["value"],
//                filehash: nimages[2]["value"],
//                date: nimages[3]["value"]
//            }));
//        }

//    }

//var data = JSON.stringify(jsonImage);

$.ajax({
    url: "GetTransactions",
    type: "POST",
    success: function (result) {
        if (result)
        {
            var count = result["TransactionList"].length;
            for (i = 0; i<result["TransactionList"].length; i++)
            {
                var reciept = web3.eth.getTransactionReceipt(result["TransactionList"][i]["thash"]);
                var ndatalistdecrypted = abiDecoder.decodeLogs(reciept["logs"]);
                var nimages = ndatalistdecrypted[0]["events"];

            jsonImage.push(new Object({
                id: nimages[0]["value"],
                filehash: nimages[1]["value"],
                date: nimages[2]["value"]
                }));

            }

            $.ajax({
                url: "ValidateImages",
                type: "POST",
                data: { "data": JSON.stringify(jsonImage) },
                success: function (JTransaction) {
                    if (JTransaction)

                        for (var i = 0; i < JTransaction["JTransaction"].length; i++) {

                            $("#transactiontable").append("<tr><td>" + JTransaction["JTransaction"][i]["id"] + "</td><td>" + JTransaction["JTransaction"][i]["filename"] + "</td> <td>  <img src=/images/" + JTransaction["JTransaction"][i]["status"] + " style='width:50px; height:50px'> </td> <td>" + JTransaction["JTransaction"][i]["date"] + "</td> </tr>");

                        }

                }
            });
        }
     
}
});

