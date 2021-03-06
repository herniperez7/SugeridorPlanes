﻿var gbPlanToEdit;
var gbPlanToEditRut = "";
var defPlansList;
var currentDeviceAmount = 0;
var billingGap = 0;

$(document).ready(function () {

    $(function () {
        $("#tablaPlanes").tablesorter();
    });

    $(function () {
        $("#tableMobiles").tablesorter();
    });

    $("#movilesDdl").select2({
        placeholder: "Seleccionar un equipo"
    });

    $("#calculateSubsidyTxt").mask("#.##0", { reverse: true });
    $("#calculateIncomeTxt").mask("#.##0", { reverse: true });
    unfocusInput();
    //focusInput();

    $("#clientRutBtn").prop('disabled', true);
    $('#clientRutTxt1').keyup(function () {
        if ($(this).val().length !== 0)
            $("#clientRutBtn").prop('disabled', false);
        else
            $("#clientRutBtn").prop('disabled', true);
    });


    $('#tablaPlanes tbody tr').on('click', function () {
        selectPlan(this);
    });

    $('#tableMobiles tbody tr').on('click', function () {
        selectMobile(this);
    });


    if (gbCurrentClient !== 'null') {
        $("#clientSelect").val(gbCurrentClient);
    }

    $("#pagoEquiposTxt").keypress(function (e) {
        
        var code = (e.keyCode ? e.keyCode : e.which);
        if (code === 13) {
            calculatePayBack();
        }
    });

    $("#pagoEquiposTxt").focusout(function () {
        calculatePayBack();
    });

     $(".tableFixHead").on("scroll", function () {
         $(".tableFixHead:not(this)").scrollTop($(this).scrollTop());   
     });

    $(function () {
        $('[data-toggle="tooltip"]').tooltip();
    });


    $("#sendMailBtn").prop('disabled', true);   
    $('.inputMail').keyup(function () {
        if ($("#subjectTxt").val().length !== 0 && $("#toTxt").val().length !== 0)
            $("#sendMailBtn").prop('disabled', false);
        else
            $("#sendMailBtn").prop('disabled', true);
    });


});

function selectPlan(selectedPlan) {
    $('#tablaPlanes tbody tr').removeClass("selectedOfertPlan");
    selectedPlan.classList.add("selectedOfertPlan");
}

function selectMobile(selectedMobile) {
    $('#tableMobiles tbody tr').removeClass("selectedOfertPlan");
    selectedMobile.classList.add("selectedOfertPlan");
}

function confirmSelectedMobile() {

    var rows = $('#tableMobiles tbody tr');
    var mobileSelected;
    var i = 0;
    while (i < rows.length && mobileSelected === undefined) {
        var element = rows[i];
        if (element.classList.contains("selectedOfertPlan")) {
            mobileSelected = element;
        }
        i++;
    }
    var mobileId = mobileSelected.cells[0].innerText.toString();
    AddDevice(mobileId, true, true);
}

function confirmSelectPlan() {
   
    if (gbPlanToEdit !== undefined) {
        var rows = $('#tablaPlanes tbody tr');
        var planSelected;
        var i = 0;
        while (i < rows.length && planSelected === undefined) {
            var element = rows[i];
            if (element.classList.contains("selectedOfertPlan")) {
                planSelected = element;
            }
            i++;
        }

        var planUpdate = { PlanToEditRut: gbPlanToEditRut, PlanToEdit: gbPlanToEdit, Plan: planSelected.cells[0].innerText, TMM: planSelected.cells[1].innerText, Bono: planSelected.cells[2].innerText, Roaming: planSelected.cells[3].innerText };

        $.ajax({
            type: "POST",
            url: gbUpdateDefinitivePlanUrl,
            contentType: "application/json",
            data: JSON.stringify(planUpdate),
            processData: true,
            success: function (recData) { loadDefinitivePlans(recData); },
            error: function () { alert('A error'); }
        });
    }
}

function establisPlanToEdit(planToEdit) {    
    gbPlanToEdit = planToEdit;
}

function loadDefinitivePlans(planList) {

    if (!defPlansList) {
        defPlansList = planList;

    }
    if (planList.length > 0) {

        //se llama a metodo apara calcular gaps
        calculateGaps(gbPlanToEditRut);

        var totalTmm = 0;
        var totalBono = 0;
        var roamingCount = 0;
        var bono = 0;
        $('#tablaPlanesDefi tbody').html("");
        for (var i = 0; i < planList.length; i++) {
            var plan = planList[i];
            totalTmm += plan.tmM_s_iva;
            totalBono += plan.bono;
            bono = plan.bono;

            if (plan.roaming.toString().toLowerCase() !== "no") {
                roamingCount++;
            }

            var element = "";
            element += "<tr>";
            element += "<td>" + plan.recomendadorId + "</td>";
            element += "<td>" + plan.plan + "</td>";
            element += "<td>" + "$" + plan.tmmString + "</td>";
            element += "<td>" + bono + " Gb</td>";
            element += "<td data-toggle='tooltip' data-placement='top' title=" + plan.roaming +">" + plan.roaming + "</td>";

            element += '<td class="editRow"><a data-toggle="modal" onclick="establisPlanToEdit(' + plan.recomendadorId + ')" href="#plansModal" class="btn btn-outline-success my-2 my-sm-0">Editar</a></td>';
            element += "</tr>";
            $('#tablaPlanesDefi tbody').append(element);
        }


        totalTmm = Number(totalTmm.toFixed(2));
        var totalRow = "<tr><td class='total-column' colspan='2' style='text-align: left;'>Totales</td><td class='total-column'>" + "$ " + formatNumberStr(totalTmm) + "</td>  <td class='total-column'>" + totalBono + " Gb</td> <td class='total-column'>" + roamingCount + "</td> </tr>";

        $('#tablaPlanesDefi tbody').append(totalRow);
        $("#incomeDivValue").html(totalTmm);
        calculatePayBack();
    }
}


//Logica moviles

function confirmDevices() {
    var total = 0;
    $.ajax({
        url: gbGetMovilListUrl,
        success: function (data) {
            if (data.status === "ok") {
                $.each(data.result, function (key, value) {
                    total += value.precioSinIva;
                });

                total = Number(total.toFixed(1));
                currentDeviceAmount = total;
                var inputValue = "$ " + formatNumberStr(total);
                $("#subsidioTxt").val(inputValue);
                calculatePayBack();
            } else {
                showToastError(data.message);  
            }
        }
    });
}


function movileChange() {
    var val = $("#movilesDdl").val();
    if (val === "0") {
        $("#addDeviceBtn").prop('disabled', true);
    } else {
        $("#addDeviceBtn").prop('disabled', false);
    }

    $.ajax({

        url: gbGetMovilInfoUrl + '?code=' + val,
        success: function (data) {
            if (data.status === "ok") {

                if (data.result) {
                    var precio = "$ " + formatNumberStr(data.result.precioSinIva);
                    $("#landedValue").html(precio);

                    //este valor se comenta por que aun no esta resuelto el stock (descomentar la etiqueta en la vista tambien)
                    //$("#stockValue").html(data.result.stock); 
                } else {
                    $("#landedValue").html("");
                    //$("#stockValue").html("");
                }
            } else {
                showToastError(data.message);  
            }
        }
    });
}


// si isModal esta en true, es por que viene el id desde el modal de moviles o de la tabla generada automaticamente
function AddDevice(val, add, isModal = false) {
    var id = val;
    var url = gbDeleteMovilUrl;

    //si se agrega el movil, el valor del id se saca del combo box
    if (add) {
        url = gbAddMovilDecivesUrl;
        id = $("#movilesDdl").val();
        if (isModal) {
            id = val;
        }
    }

    $.ajax({
        type: "POST",
        url: url + '?code=' + id,
        success: function (data) {

            if (data.status === "ok") {

                $("#movilTableBody").html("");
                var devicesCount = 0;
                var devicesAmount = 0;
                $.each(data.result, function (key, value) {

                    var brand = value.marca !== null ? value.marca : "";
                    var model = value.nombre !== null ? value.nombre : "";
                    var labelName = brand + " " + model;

                    var mobileName = labelName;
                    if (labelName.length > 20) {
                        mobileName = labelName.substr(0, 15) + "...";
                    }

                    devicesCount++;
                    var precio = formatNumberStr(value.precioSinIva);
                    var trashIcon = "<i class='fa fa-times fa-lg' aria-hidden='true'></i>";
                    var tr = "<tr id='row" + value.id + "' ><td data-toggle='tooltip' data-placement='top' title='" + labelName + "'>" + mobileName + "</td><td>$" + precio + "</td><td id='deleteTd" + value.id + "' onclick='AddDevice(" + value.id + "," + false + ")'>" + trashIcon + "</td></tr>";
                    $("#movilTableBody").append(tr);

                    devicesAmount += value.precioSinIva;
                    devicesAmount = Number(devicesAmount.toFixed(1));
                });

                var totalRow = "<tr id='totalRow'><td class='total-column'>" + devicesCount + "<td class='total-column'>$ " + formatNumberStr(devicesAmount) + "<td class='total-column'></td> </tr>";
                $("#movilTableBody").append(totalRow);

            } else {
                showToastError(data.message);  
            }
        }
    });

}




function deleteRow(val) {


    $.ajax({
        type: "POST",
        url: gbDeleteMovilUrl + '?code=' + val,
        success: function (data) {
            if (data.status === "ok") {
                $("#movilTableBody tr:last").remove();
                if (data.result) {
                    var rowId = "row" + val;
                    $("#" + rowId).remove();
                    devicesCount--;
                    devicesAmount -= data.result.precioSinIva;
                    var totalRow = "<tr id='totalRow'><td><b>" + devicesCount + "</b></td><td><b>$ " + formatNumber(devicesAmount) + "</b></td><td></td> </tr>";
                    $("#movilTableBody").append(totalRow);

                    if (devicesCount === 0) {
                        $("#movilTableBody tr").remove();
                    }
                }
            } else {
                showToastError(data.message);  
            }
        }
    });

}

//logica payback

function calculatePayBack() {

    var devicePayment = $("#pagoEquiposTxt").val();
    if (isNaN(devicePayment)) devicePayment = 0;

    $.ajax({
        url: gbCalculatePayBack + '?devicePayment=' + devicePayment,
        success: function (data) {
            if (data.status === "ok") {
                $("#paybackTxt").val(data.result);
            } else {
                showToastError(data.message);  
            }
        }
    });
}

//logica gaps

function calculateGaps(val) {

    $.ajax({
        url: gbCalculateGap + '?rut=' + val,
        success: function (data) {
            if (data.status === "ok") {
                var billingGapValue = "$ " + formatNumberStr(data.result.billingGap);
                var fixedGapValue = "$ " + formatNumberStr(data.result.fixedGap);

                $("#gapValue").html(fixedGapValue);
                $("#gapBilingValue").html(billingGapValue);
                billingGap = data.result.billingGap;

                $("#divbillingStatus").html("");

                var superiorBillingDiv = "<div class='superiorBillingMain'><div class='superiorBillingChild'><i class='fa fa-check-circle fa-lg' aria-hidden='true'><span class='spanSatus'> Facturación superior </span></i></div></div>";
                var lowerBillingDiv = "<div class='lowerBillingMain'><div class='lowerBillingChild' ><div style='font-weight:bold; color:#FF0000'><i class='fa fa-times fa-lg' aria-hidden='true'><span class='spanSatus'> Facturación inferior </span></i></div></div></div>";
                var equalBillingDiv = "<div class='equalBillingMain'><div class='equalBillingMainChild'><div style='font-weight:bold; color:#FFD200'><i class='fa fa-exclamation-circle fa-lg' aria-hidden='true'><span class='spanSatus'> Misma facturación </span></i></div></div></div>";

                switch (data.result.billingStatus.toString()) {
                    case "0":
                        $("#divbillingStatus").append(superiorBillingDiv);
                        break;
                    case "1":
                        $("#divbillingStatus").append(equalBillingDiv);
                        break;
                    case "2":
                        $("#divbillingStatus").append(lowerBillingDiv);
                        break;
                }

            }
        }
    });
}


function formatNumber(n) {
    n = String(n).replace(/\D/g, "");
    return n === '' ? n : Number(n).toLocaleString();
}





function formatNumberStr(nStr) {
    nStr += '';
    var x = nStr.split('.');
    var x1 = x[0];
    var x2 = x.length > 1 ? ',' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + '.' + '$2');
    }
    return x1 + x2;
}


function alternateBugdetFields(val) {
    if (val === 1) {
        $("#IndexesDiv").hide();
        $("#calculateDiv").show();
    } else {
        $("#IndexesDiv").show();
        $("#calculateDiv").hide();
    }

}

function calculateIndexes(val) {

    var $subsidy = $("#calculateSubsidyTxt");
    var $payback = $("#calculatePaybackTxt");
    var $income = $("#calculateIncomeTxt");

    var subsidyValue = cleanFormat($subsidy.val());
    var paybackValue = $payback.val();
    var incomeValue = cleanFormat($income.val());

    if (val === 1) {
        var subsidy = correctFormatInverse(incomeValue) * correctFormatInverse(paybackValue);
        subsidy = correctFormat(subsidy.toFixed(1));
        $subsidy.val(formatNumberStr(subsidy));

    } else if (val === 2) {
        var payback = correctFormatInverse(subsidyValue) / correctFormatInverse(incomeValue);
        payback = correctFormat(payback.toFixed(2));
        $payback.val(payback);
    } else {
        var income = correctFormatInverse(subsidyValue) / correctFormatInverse(paybackValue);
        income = correctFormat(income.toFixed(1));
        $income.val(formatNumberStr(income));
        calculateStatus(income);
    }

    $("#calulatedSubsidy").html("$ " + $subsidy.val());
    $("#calulatedPayBack").html($payback.val());
    $("#calulatedIncome").html("$ " + $income.val());


    calculateSecondGaps();


}

//se calculan los gaps ubicados en el fieldset de calculo de indices (herramienta de calculo)
function calculateSecondGaps() {
    var billingAmout = $("#billingDivValue").text();
    var tmmPrestasion = $("#tmmPrestasionDivValue").text();
    var incomeAmount = cleanFormat($("#calculateIncomeTxt").val());
    var billingGapValue = parseFloat(incomeAmount) - parseFloat(billingAmout);
    billingGapValue = "$ " + formatNumberStr(billingGapValue);

    var fixedGapValue = parseFloat(incomeAmount) - parseFloat(tmmPrestasion);
    fixedGapValue = "$ " + formatNumberStr(fixedGapValue);


    $("#gapBilingCalculatedValue").html(billingGapValue);
    $("#gapCalculatedValue").html(fixedGapValue);
}

//metodo para inportar los valores actuales a los inputs de calculos
async function importValues() {
    var $incomes = $("#incomeDivValue").text();

    var $subsidy = currentDeviceAmount;
    var $payback = $("#paybackTxt").val();
    var incomeInt = parseInt($incomes);
    var regex = /[.\s]/g;

    $("#calculateSubsidyTxt").val(formatNumberStr($subsidy));
    $("#calculatePaybackTxt").val($payback.replace(regex, ''));
    $("#calculateIncomeTxt").val(formatNumberStr($incomes));

    calculateSecondGaps();
    calculateStatus(incomeInt);

}

function resetValues() {
    var emptyBillingDiv = "<div class='emptyBillingDataMain'><div class='emptyBillingDataChild'><div style='font-weight:bold; color:#666666'>Sin datos</div></div></div>";
    var $divSatus = $("#divClaculatedBillingStatus");
    $divSatus.html("");
    $("#divClaculatedBillingStatus").append(emptyBillingDiv);
    $("#calculateSubsidyTxt").val("");
    $("#calculatePaybackTxt").val("");
    $("#calculateIncomeTxt").val("");

}

//esta funcion se activa cuando se cliquea fuera del input de los ingresos en la herramienta de calculo
function unfocusInput() {

    $("#calculateIncomeTxt").focusout(function () {
        var incomeAmount = $("#calculateIncomeTxt").val();
        calculateStatus(incomeAmount);
    });

}

function focusInput() {
    $("#calculateSubsidyTxt").focus(function () {
        var $subsidyVal = $("#calculateSubsidyTxt").val();
        var regex = /[.\s]/g;
        $subsidyVal = $subsidyVal.replace(regex, '');
        $("#calculateSubsidyTxt").val($subsidyVal);
    });
}

function cleanFormat(val) {
    var regex = /[.\s]/g;
    val = val.replace(regex, '');
    return val;
}


//funcion para cambiar el punto por coma
function correctFormat(val) {
    var regex = /[.\s]/g;
    val = val.replace(regex, ',');
    return val;
}

//funcion para cambiar una compa por un punto
function correctFormatInverse(val) {
    var regex = /[,\s]/g;
    val = val.replace(regex, '.');
    return val;
}


//setea el status del fieldet de calculo de indices
function calculateStatus(val) {

    val = cleanFormat(val.toString()); //le saco el punto de miles en caso de que venga con ese formato
    var billingAmout = $("#billingDivValue").text();
    var billingGap = parseInt(val) - parseInt(billingAmout);


    var $divSatus = $("#divClaculatedBillingStatus");
    $divSatus.html("");
    var superiorBillingDiv = "<div class='superiorBillingMain'><div class='superiorBillingChild'><i class='fa fa-check-circle fa-lg' aria-hidden='true'><span class='spanSatus'> Facturación superior </span></i></div></div>";
    var lowerBillingDiv = "<div class='lowerBillingMain'><div class='lowerBillingChild' ><div style='font-weight:bold; color:#FF0000'><i class='fa fa-times fa-lg' aria-hidden='true'><span class='spanSatus'> Facturación inferior </span></i></div></div></div>";
    var equalBillingDiv = "<div class='equalBillingMain'><div class='equalBillingMainChild'><div style='font-weight:bold; color:#FFD200'><i class='fa fa-exclamation-circle fa-lg' aria-hidden='true'><span class='spanSatus'> Misma facturación </span></i></div></div></div>";

    switch (true) {
        case billingGap > 0:
            $("#divClaculatedBillingStatus").append(superiorBillingDiv);
            break;
        case billingGap === 0:
            $("#divClaculatedBillingStatus").append(equalBillingDiv);
            break;
        case billingGap < 0:
            $("#divClaculatedBillingStatus").append(lowerBillingDiv);
            break;
    }

}


function exportPdf() {

    var loading = '<span class="spinner-grow spinner-grow-sm" role="status" aria-hidden="true" ></span>';
    var exportText = "Generar Propuesta";
    $("#generarProposalBtn").html(loading);
    $("#generarProposalBtn").prop("disabled", true);
    var devicePayment = $("#pagoEquiposTxt").val();
    if (devicePayment === "") devicePayment = "0";
    $("#loaderDiv").show();

    var companyName = $("#clientSelect option:selected").text();
    $.ajax({
        type: "GET",
        url: gbExportPdf + '?devicePayment=' + devicePayment,
        success: function (data) {
            if (data.status === "ok") {
                var pdfBase64 = base64ToArrayBuffer(data.result);
                saveByteArray("presupuesto-" + companyName, pdfBase64);
                $("#generarProposalBtn").html(exportText);
                $("#generarProposalBtn").prop("disabled", false);
                $("#loaderDiv").hide();
            } else if (data.status === "error"){
                {
                    $("#generarProposalBtn").html(exportText);
                    $("#generarProposalBtn").prop("disabled", false);
                    $("#loaderDiv").hide();
                    showToastError(data.message);
                }
            }
        }
    });
}


function generarProposal() {
    var devicePayment = $("#pagoEquiposTxt").val();
    if (devicePayment === "") devicePayment = "0";
    $("#loaderDiv").show();
    $.ajax({
        type: "POST",
        url: gbGenerateProposal + '?devicePayment=' + devicePayment,
        success: function (data) {
            if (data.status === "ok") {
                $("#loaderDiv").hide();
                window.location.href = gbUserProposals;
            }
            else if (data.status === "error")
            {
                $("#loaderDiv").hide();
                showToastError(data.message);  
                   
            }

        }
    });
}

function sendMail() {

    var loading = '<span class="spinner-grow spinner-grow-sm" role="status" aria-hidden="true" ></span>';
    $("#generarProposalBtn").html(loading);
    $("#generarProposalBtn").prop("disabled", true);
    var buttontext = "Generar propuesta";

    var toText = $("#toTxt").val();
    var subjectText = $("#subjectTxt").val();
    var bodyText = $("#bodytxt").val();
    var devicePayment = $("#pagoEquiposTxt").val();
    $("#loaderDiv").show();

    $.ajax({
        type: "POST",
        url: gbSendMail + '?to=' + toText + '&subject=' + subjectText + '&bodytext=' + bodyText + '&devicePayment=' + devicePayment,
        success: function (data) {
            if (data.status === "ok") {
                $("#generarProposalBtn").html(buttontext);
                $("#generarProposalBtn").prop("disabled", false);
                $("#loaderDiv").hide();
            } else if (data.status === "error"){
                $("#generarProposalBtn").html(buttontext);
                $("#generarProposalBtn").prop("disabled", false);
                $("#loaderDiv").hide();
                showToastError(data.message);  
            }

        }
    });
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

///////////////////////////


function saveProposal() {
    var devicePayment = $("#pagoEquiposTxt").val();
    if (devicePayment === "") devicePayment = "0";
    $("#loaderDiv").show();
    var loading = '<span class="spinner-grow spinner-grow-sm" role="status" aria-hidden="true" ></span>';
    $("#generarProposalBtn").html(loading);
    $("#generarProposalBtn").prop("disabled", true);
    var buttontext = "Generar propuesta";

    $.ajax({
        type: "POST",
        url: gbSaveProposal + '?devicePayment=' + devicePayment,
        success: function (data) {
            if (data.status === "ok") {
                $("#loaderDiv").hide();
                $("#saveProposaltext").html(data.result);
                $("#generarProposalBtn").html(buttontext);
                $("#generarProposalBtn").prop("disabled", false);
                $('#modalPush').modal('show');
            } else {
                $("#generarProposalBtn").html(buttontext);
                $("#generarProposalBtn").prop("disabled", false);
                $("#loaderDiv").hide();
                showToastError(data.message);  
            }
        }
    });
}

function openEmailModal() {
    $('#emailModal').modal('show');
}

function proposalMenu() {
    window.location.href = gbUserProposals;
}

function openMobileModal() {
    $('#mobilesModal').modal('show');
}
