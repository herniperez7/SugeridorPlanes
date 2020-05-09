var gbPlanToEdit;
var gbPlanToEditRut = "";
var defPlansList;
//var totalDefPlansTmm;
$(document).ready(function () {

    $("#clientRutBtn").prop('disabled', true);
    $('#clientRutTxt1').keyup(function () {
        if ($(this).val().length != 0)
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
    if (gbPlanToEdit != undefined) {
        var rows = $('#tablaPlanes tbody tr');
        var planSelected;
        var i = 0
        while (i < rows.length && planSelected == undefined) {
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
            //  bono = plan.bono / 1024;

            if (plan.roaming.toString().toLowerCase() !== "no") {
                roamingCount++;
            }


            var element = "";
            element += "<tr>";
            element += "<td>" + plan.recomendadorId + "</td>";
            element += "<td>" + plan.plan + "</td>";
            element += "<td>" + "$" + plan.tmM_s_iva + "</td>";
            element += "<td>" + plan.bono + " Gb</td>";
            element += "<td>" + plan.roaming + "</td>";

            element += '<td class="editRow"><a data-toggle="modal" onclick="establisPlanToEdit(' + plan.recomendadorId + ', ' + gbPlanToEditRut + ')" href="#plansModal" class="btn btn-outline-success my-2 my-sm-0">Editar</a></td>';
            element += "</tr>";
            $('#tablaPlanesDefi tbody').append(element);
        }


        var totalRow = "<tr><td class='total-column' colspan='3'>" + "$ " + totalTmm + "</td> <td class='total-column'>" + totalBono + " Gb</td> <td class='total-column'>" + roamingCount + "</td> </tr>";

        $('#tablaPlanesDefi tbody').append(totalRow);
        calculatePayBack();
    }
}


//Logica moviles

//sin uso actualmente
function getMovileList() {

    $.ajax({
        url: gbGetMovilListUrl,
        success: function (data) {
            if (data.status == "ok") {
                //console.log(data.result);
                return data.result;
            }
        }
    });
}


function populateBenefitInput() {
    var total = 0;
    $.ajax({
        url: gbGetMovilListUrl,
        success: function (data) {
            if (data.status == "ok") {
                $.each(data.result, function (key, value) {
                    total += value.precio;
                });
                $("#subsidioTxt").val(formatNumber(total));
            }
        }
    });

}


function movileChange() {
    var val = $("#movilesDdl").val();
    if (val == "0") {
        $("#addDeviceBtn").prop('disabled', true);
    } else {
        $("#addDeviceBtn").prop('disabled', false);
    }

    $.ajax({

        url: gbGetMovilInfoUrl + '?code=' + val,
        success: function (data) {
            if (data.status == "ok") {

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
    var subsidio = parseInt($("#subsidioTxt").val());

    $.ajax({
        type: "POST",
        url: gbAddMovilDecivesUrl + '?code=' + val,
        success: function (data) {
            if (data.status == "ok") {
                var precio = formatNumber(data.result.precio);
                var trashIcon = '<svg  class="bi bi-trash-fill" width="1em" height="1em" viewBox="0 0 16 16" fill="currentColor" xmlns="http://www.w3.org/2000/svg">' +
                    ' <path fill-rule="evenodd" d="M2.5 1a1 1 0 00-1 1v1a1 1 0 001 1H3v9a2 2 0 002 2h6a2 2 0 002-2V4h.5a1 1 0 001-1V2a1 1 0 00-1-1H10a1 1 0 00-1-1H7a1 1 0 00-1 1H2.5zm3 4a.5.5 0 01.5.5v7a.5.5 0 01-1 0v-7a.5.5 0 01.5-.5zM8 5a.5.5 0 01.5.5v7a.5.5 0 01-1 0v-7A.5.5 0 018 5zm3 .5a.5.5 0 00-1 0v7a.5.5 0 001 0v-7z" clip-rule="evenodd" />' +
                    ' </svg>';

                var tr = "<tr id='row" + data.result.codigo + "' ><th scope='row'>" + data.result.marca + "</th><td>$" + precio + "</td><td id='deleteTd" + data.result.codigo + "' onclick='deleteRow(" + data.result.codigo + ")'>" + trashIcon + "</td></tr>";
                $("#movilTableBody").append(tr);

                //se agrega el calculo del payback
                calculatePayBack();
                populateBenefitInput();
            }
        }
    });
}

function deleteRow(val) {

    var subsidio = parseInt($("#subsidioTxt").val());

    $.ajax({
        url: gbDeleteMovilUrl + '?code=' + val,
        success: function (data) {
            if (data.status == "ok") {
                if (data.result) {
                    var total = subsidio - parseInt(data.result.precio);
                    $("#subsidioTxt").val(total);
                    var rowId = "row" + val;
                    $("#" + rowId).remove();
                }

                //se agrega el calculo del payback
                calculatePayBack();
                populateBenefitInput();
            }
        }
    });

}

//logica payback

function calculatePayBack() {

    $.ajax({
        url: gbCalculatePayBack,
        success: function (data) {
            if (data.status == "ok") {
                $("#paybackTxt").val(data.result);
            }
        }
    });
}

function formatNumber(n) {
    n = String(n).replace(/\D/g, "");
    return n === '' ? n : Number(n).toLocaleString();
}

//logica gaps

function calculateGaps(val) {

    $.ajax({
        url: gbCalculateGap + '?rut=' + val,
        success: function (data) {
            if (data.status == "ok") {
               
                $("#gapValue").html(data.result.fixedGap);
                $("#gapBilingValue").html(data.result.billingGap);

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
              //  <span class='spanSatus'> Facturación superior </span>
            }
        }
    });
}

