
var proposalId = "";

$(document).ready(function () {
    $(function () {
        $("#tableProposalList").tablesorter();
    });
});

function openProposal(val) {
    window.location.href = openProposalUrl + '?proposalId=' + val;
}

function setProposalId(val) {
    proposalId = val;
}

function deleteProposal() {

    window.location.href = deleteProposalUrl + '?proposalId=' + proposalId;

}





//function openProposal(proposal) {
//    if (proposal !== undefined) {
//        $.ajax({
//            type: "POST",
//            url: gbOpenProposal + '?proposalId=' + proposal,
//        });
//    }
//}

