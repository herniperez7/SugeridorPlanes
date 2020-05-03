var gbPlanToEdit;
var gbPlanToEditRut="";
        $(document).ready(function () {
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
                    success: function(recData) { loadDefinitivePlans(recData) },
                    error: function() { alert('A error'); }
                });
            } 
        }

        function establisPlanToEdit(planToEdit,rut) {
            gbPlanToEdit = planToEdit;
            gbPlanToEditRut = rut.toString();
        }

function loadDefinitivePlans(planList) {
    if (planList.length > 0) {
        $('#tablaPlanesDefi tbody').html("");
        for (var i = 0; i < planList.length; i++)
        {
            var plan = planList[i];
            var element = "";
            element += "<tr>";
            element += "<td>" + plan.recomendadorId + "</td>";
            element += "<td>" + plan.plan + "</td>";
            element += "<td>" + plan.tmM_s_iva + "</td>";
            element += "<td>" + plan.bono + "</td>";
            element += "<td>" + plan.roaming + "</td>";
            element += '<td class="editRow"><a data-toggle="modal" onclick="establisPlanToEdit(' + gbPlanToEdit + ', '+ gbPlanToEditRut+ ')" href="#plansModal" class="btn btn-outline-success my-2 my-sm-0">Editar</a></td>';
            element += "</tr>";
            $('#tablaPlanesDefi tbody').append(element);
        }
    }
}
    