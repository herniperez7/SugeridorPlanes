var gbPlanToEdit;
var gbPlanToEditRut = "";
var defPlansList;
var devicesAmount = 0;
var currentDeviceAmount = 0;
var devicesCount = 0;
var billingGap = 0;

$(document).ready(function () {

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
    })


    $('#tablaPlanes tbody tr').on('click', function () {
        selectPlan(this);
    });
});

function selectPlan(selectedPlan) {
    $('#tablaPlanes tbody tr').removeClass("selectedOfertPlan");
    selectedPlan.classList.add("selectedOfertPlan");
}

function confirmSelectPlan() {
    if (gbPlanToEdit !== undefined) {
        var rows = $('#tablaPlanes tbody tr');
        var planSelected;
        var i = 0
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
            success: function (recData) { loadDefinitivePlans(recData) },
            error: function () { alert('A error'); }
        });
    }
}

function establisPlanToEdit(planToEdit, rut) {
    gbPlanToEdit = planToEdit;
    gbPlanToEditRut = rut.toString();
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
            element += "<td>" + plan.roaming + "</td>";

            element += '<td class="editRow"><a data-toggle="modal" onclick="establisPlanToEdit(' + plan.recomendadorId + ', ' + gbPlanToEditRut + ')" href="#plansModal" class="btn btn-outline-success my-2 my-sm-0">Editar</a></td>';
            element += "</tr>";
            $('#tablaPlanesDefi tbody').append(element);
        }



        var totalRow = "<tr><td class='total-column' colspan='3'>" + "$ " + formatNumberStr(totalTmm) + "</td> <td class='total-column'>" + totalBono + " Gb</td> <td class='total-column'>" + roamingCount + "</td> </tr>";

        $('#tablaPlanesDefi tbody').append(totalRow);
        $("#incomeDivValue").html(totalTmm);
        calculatePayBack();
    }
}


//Logica moviles

//sin uso actualmente
function getMovileList() {

    $.ajax({
        url: gbGetMovilListUrl,
        success: function (data) {
            if (data.status === "ok") {                
                return data.result;
            }
        }
    });
}


function confirmDevices() {
    var total = 0;
    $.ajax({
        url: gbGetMovilListUrl,
        success: function (data) {
            if (data.status === "ok") {
                $.each(data.result, function (key, value) {
                    total += value.precio;
                });
                currentDeviceAmount = total;
                var inputValue = "$ " + formatNumber(total);
                $("#subsidioTxt").val(inputValue);
                calculatePayBack();
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
                    var precio = "$ " + formatNumber(data.result.precio);
                    $("#landedValue").html(precio);
                    $("#stockValue").html(data.result.stock);
                } else {
                    $("#landedValue").html("");
                    $("#stockValue").html("");
                }
            }
        }
    });
}

function AddDevice() {

    var val = $("#movilesDdl").val();     

    $.ajax({
        type: "POST",
        url: gbAddMovilDecivesUrl + '?code=' + val,
        success: function (data) {
            if (data.status === "ok") {
                $("#movilTableBody tr:last").remove();
                devicesCount++;
                devicesAmount += data.result.precio;
                var precio = formatNumber(data.result.precio);               
                var trashIcon = "<i class='fa fa-times fa-lg' aria-hidden='true'></i>";
                var tr = "<tr id='row" + data.result.codigo + "' ><td scope='row'>" + data.result.marca + "</td><td>$" + precio + "</td><td id='deleteTd" + data.result.codigo + "' onclick='deleteRow(" + data.result.codigo + ")'>" + trashIcon + "</td></tr>";
                $("#movilTableBody").append(tr);

                

                var totalRow = "<tr id='totalRow'><td><b>" + devicesCount + "</b><td><b>$ " + formatNumber(devicesAmount) +"</b><td></td> </tr>"
                $("#movilTableBody").append(totalRow);
             
            }
        }
    });
}



function deleteRow(val) {    

    $.ajax({
        url: gbDeleteMovilUrl + '?code=' + val,
        success: function (data) {
            if (data.status === "ok") {
                $("#movilTableBody tr:last").remove();
                if (data.result) {                 
                    var rowId = "row" + val;
                    $("#" + rowId).remove();
                    devicesCount--;
                    devicesAmount -= data.result.precio;
                    var totalRow = "<tr id='totalRow'><td>" + devicesCount + "</td><td>$ " + formatNumber(devicesAmount) + "</td><td></td> </tr>"
                    $("#movilTableBody").append(totalRow);

                    if (devicesCount === 0) {
                        $("#movilTableBody tr").remove();
                    }
                }
                
                
            }
        }
    });

}

//logica payback

function calculatePayBack() {

    $.ajax({
        url: gbCalculatePayBack,
        success: function (data) {
            if (data.status === "ok") {
                $("#paybackTxt").val(data.result);
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

                $("#gapValue").html(data.result.fixedGap);                
                $("#gapBilingValue").html(data.result.billingGap);
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

    $("#calculateSubsidyTxt").val(formatNumber($subsidy));
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

    $.ajax({
        url: gbExportPdf + '?companyName=' + "asd",
        success: function (data) {
            if (data.status === "ok") {
                var byteArray = getByteArray(data.result);
                console.log(data.result);
                var array = [].slice.call(byteArray);

                
                var file = new Blob(array, { type: 'application/pdf' });
                var fileURL = URL.createObjectURL(file);
                window.open(fileURL);
            }            
        }
    });
}


function getByteArray(str) {
    var decoded = atob(str);
    var i, il = decoded.length;
    var array = new Uint8Array(il);

    for (i = 0; i < il; ++i) {
        array[i] = decoded.charCodeAt(i);
    }

    return array;
}


