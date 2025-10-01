// site.js - Funcionalidades mejoradas para Spotify MVC

// Inicialización cuando el documento está listo
$(document).ready(function () {
    initSpotifyApp();
});

function initSpotifyApp() {
    // Mejoras en formularios
    enhanceForms();
    
    // Mejoras en tablas
    enhanceTables();
    
    // Efectos de carga
    handleLoadingStates();
    
    // Confirmaciones de eliminación
    handleDeleteConfirmations();
}

function enhanceForms() {
    // Auto-focus en el primer campo de los formularios
    $('form').each(function () {
        const firstInput = $(this).find('input[type="text"], input[type="email"], input[type="password"]').first();
        if (firstInput.length) {
            firstInput.focus();
        }
    });
    
    // Validación en tiempo real
    $('.form-control').on('input', function () {
        const $this = $(this);
        if ($this.val().trim() !== '') {
            $this.removeClass('is-invalid').addClass('is-valid');
        } else {
            $this.removeClass('is-valid');
        }
    });
}

function enhanceTables() {
    // Efectos hover en filas de tabla
    $('.table-hover tbody tr').hover(
        function () {
            $(this).css('transform', 'scale(1.01)');
        },
        function () {
            $(this).css('transform', 'scale(1)');
        }
    );
    
    // Ordenamiento básico de tablas
    $('th[data-sort]').click(function () {
        const table = $(this).closest('table');
        const rows = table.find('tbody tr').get();
        const column = $(this).index();
        const direction = $(this).hasClass('sorted-asc') ? -1 : 1;
        
        // Remover clases de ordenamiento existentes
        table.find('th').removeClass('sorted-asc sorted-desc');
        
        // Ordenar filas
        rows.sort(function (a, b) {
            const aVal = $(a).find('td').eq(column).text().toLowerCase();
            const bVal = $(b).find('td').eq(column).text().toLowerCase();
            return aVal.localeCompare(bVal) * direction;
        });
        
        // Reordenar tabla
        $.each(rows, function (index, row) {
            table.find('tbody').append(row);
        });
        
        // Marcar columna ordenada
        $(this).addClass(direction === 1 ? 'sorted-asc' : 'sorted-desc');
    });
}

function handleLoadingStates() {
    // Mostrar loading en botones de envío
    $('form').on('submit', function () {
        const submitBtn = $(this).find('button[type="submit"]');
        const originalText = submitBtn.html();
        
        submitBtn.prop('disabled', true)
                .html('<div class="loading"></div> Procesando...');
        
        // Restaurar después de 5 segundos (fallback)
        setTimeout(function () {
            submitBtn.prop('disabled', false).html(originalText);
        }, 5000);
    });
}

function handleDeleteConfirmations() {
    // Confirmación mejorada para eliminaciones
    $('form[asp-action="Delete"]').on('submit', function (e) {
        e.preventDefault();
        
        const form = this;
        const entityName = $(this).closest('tr').find('td:first').next().text() || 'este elemento';
        
        Swal.fire({
            title: '¿Estás seguro?',
            text: `Vas a eliminar ${entityName}. Esta acción no se puede deshacer.`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#1DB954',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar',
            background: '#282828',
            color: '#FFFFFF'
        }).then((result) => {
            if (result.isConfirmed) {
                form.submit();
            }
        });
    });
}

// Funciones utilitarias
function showToast(message, type = 'success') {
    const toast = $(`
        <div class="toast align-items-center text-bg-${type} border-0" role="alert">
            <div class="d-flex">
                <div class="toast-body">
                    <i class="fas fa-${type === 'success' ? 'check' : 'exclamation'}-circle me-2"></i>
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        </div>
    `);
    
    $('#toastContainer').append(toast);
    new bootstrap.Toast(toast[0]).show();
    
    // Remover después de animación
    setTimeout(() => toast.remove(), 5000);
}

// Búsqueda en tiempo real
function setupSearch() {
    $('.search-input').on('input', function () {
        const searchTerm = $(this).val().toLowerCase();
        const $rows = $('.searchable-table tbody tr');
        
        $rows.each(function () {
            const rowText = $(this).text().toLowerCase();
            $(this).toggle(rowText.includes(searchTerm));
        });
    });
}