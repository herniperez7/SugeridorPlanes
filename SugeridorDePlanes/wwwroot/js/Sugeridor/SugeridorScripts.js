var gbPlanToEdit;
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
        var planUpdate = { PlanToEdit: gbPlanToEdit, Plan: planSelected.cells[0].innerText, TMM: planSelected.cells[1].innerText, Bono: planSelected.cells[2].innerText, Roaming: planSelected.cells[3].innerText };

        $.ajax({
            type: "POST",
            url: '@Url.Action("UpdateSuggestedPlan")',
            contentType: "application/json",
            data: JSON.stringify(planUpdate),
            processData: true,
            success: function (recData) { alert('Success'); },
            error: function () { alert('A error'); }
        });
    }
}

function establisPlanToEdit(planToEdit) {
    gbPlanToEdit = planToEdit;
}