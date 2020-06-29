function openProposal(proposal) {
    if (proposal != undefined) {
        $.ajax({
            type: "POST",
            url: gbOpenProposal + '?proposalId=' + proposal,
            }
        });
    }
    
}