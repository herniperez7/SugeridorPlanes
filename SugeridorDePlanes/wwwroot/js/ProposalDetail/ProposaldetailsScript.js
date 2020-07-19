$(document).ready(function () {

    $("#sendMailBtn").prop('disabled', true);
    $('.inputMail').keyup(function () {
        if ($("#subjectTxt").val().length !== 0 && $("#toTxt").val().length !== 0)
            $("#sendMailBtn").prop('disabled', false);
        else
            $("#sendMailBtn").prop('disabled', true);
    });

});


function proposalMenu() {
    window.location.href = gbUserProposals;
}


function exportPdf() {

    $("#loaderDiv").show();

    var companyName = $("#companyNameTitle").text();
    $.ajax({
        type: "GET",
        url: gbExportPdf,
        success: function (data) {
            if (data.status === "ok") {
                var pdfBase64 = base64ToArrayBuffer(data.result);
                saveByteArray("presupuesto-" + companyName, pdfBase64);
                $("#loaderDiv").hide();
            } else {
                $("#loaderDiv").hide();
                $('#errorModal').modal('show');
            }
        }
    });
}


function openEmailModal() {
    $('#emailModal').modal('show');
}


function base64ToArrayBuffer(base64) {
    var binaryString = window.atob(base64);
    var binaryLen = binaryString.length;
    var bytes = new Uint8Array(binaryLen);
    for (var i = 0; i < binaryLen; i++) {
        var ascii = binaryString.charCodeAt(i);
        bytes[i] = ascii;
    }
    return bytes;
}

function saveByteArray(reportName, byte) {
    var blob = new Blob([byte], { type: "application/pdf" });
    var link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    var fileName = reportName;
    link.download = fileName;
    link.click();
}


function sendMail() {  

    var toText = $("#toTxt").val();
    var subjectText = $("#subjectTxt").val();
    var bodyText = $("#bodytxt").val();    
    $("#loaderDiv").show();

    $.ajax({
        type: "POST",
        url: gbSendMail + '?to=' + toText + '&subject=' + subjectText + '&bodytext=' + bodyText,
        success: function (data) {
            if (data.status === "ok") {
                $("#loaderDiv").hide();
            } else {
                $("#loaderDiv").hide();
                $('#errorModal').modal('show');
            }
        }
    });
}


