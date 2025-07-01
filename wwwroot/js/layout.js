function showToast(type, message) {
    const toast = type === 'success' ?
        new bootstrap.Toast(document.getElementById('successToast')) :
        new bootstrap.Toast(document.getElementById('errorToast'));

    document.getElementById(type + 'Message').textContent = message;
    toast.show();
}

function showLoading(show = true) {
    if (show) {
        $('.loading').show();
        $('body').addClass('loading-cursor');
    } else {
        $('.loading').hide();
        $('body').removeClass('loading-cursor');
    }
}

$.ajaxSetup({
    beforeSend: function () { showLoading(true); },
    complete: function () { showLoading(false); },
    error: function (xhr) {
        const response = xhr.responseJSON;
        showToast('error', response?.message || 'Une erreur est survenue');
    }
});
