function switchView(viewType) {
    const cardView = document.getElementById('cardView');
    const tableView = document.getElementById('tableView');
    const cardBtn = document.getElementById('cardViewBtn');
    const tableBtn = document.getElementById('tableViewBtn');

    if (viewType === 'card') {
        cardView.classList.remove('d-none');
        tableView.classList.add('d-none');
        cardBtn.classList.add('active');
        tableBtn.classList.remove('active');
    } else {
        cardView.classList.add('d-none');
        tableView.classList.remove('d-none');
        cardBtn.classList.remove('active');
        tableBtn.classList.add('active');
    }

    // Sauvegarder la préférence
    localStorage.setItem('reglementViewType', viewType);
}

function sortReglements(sortType) {
    const container = document.getElementById('reglementsContainer');
    const cards = Array.from(container.children);

    cards.sort((a, b) => {
        switch (sortType) {
            case 'date-desc':
                return new Date(b.dataset.date) - new Date(a.dataset.date);
            case 'date-asc':
                return new Date(a.dataset.date) - new Date(b.dataset.date);
            case 'montant-desc':
                return parseFloat(b.dataset.montant) - parseFloat(a.dataset.montant);
            case 'montant-asc':
                return parseFloat(a.dataset.montant) - parseFloat(b.dataset.montant);
            case 'partenaire':
                return a.dataset.partenaire.localeCompare(b.dataset.partenaire);
            default:
                return 0;
        }
    });

    // Réorganiser les cartes
    cards.forEach(card => container.appendChild(card));
}

function confirmDeleteCard(reglementId, partenaireNom, datePaiement) {
    if (confirm(`Êtes-vous sûr de vouloir supprimer le règlement de ${partenaireNom} du ${datePaiement} ?`)) {
        // Ici vous pouvez utiliser AJAX ou rediriger
        window.location.href = `/Reglement/Delete/${reglementId}`;
    }
}

function clearFilters() {
    // Rediriger vers la page sans filtres
    window.location.href = '/Reglement';
}

// Restaurer la vue préférée au chargement
document.addEventListener('DOMContentLoaded', function () {
    const savedView = localStorage.getItem('reglementViewType');
    if (savedView) {
        switchView(savedView);
    }
});
