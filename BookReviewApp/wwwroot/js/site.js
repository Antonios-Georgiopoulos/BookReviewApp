// Book Review App - Site JavaScript

$(document).ready(function () {
    // Initialize tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Auto-hide alerts after 5 seconds
    setTimeout(function () {
        $('.alert-dismissible').fadeOut(500);
    }, 5000);

    // Form validation enhancements
    $('form').on('submit', function (e) {
        var form = $(this);
        var submitBtn = form.find('button[type="submit"]');

        if (form[0].checkValidity() === false) {
            e.preventDefault();
            e.stopPropagation();
            form.addClass('was-validated');
            return false;
        }

        // Add loading state to submit button
        submitBtn.addClass('loading').prop('disabled', true);

        // Re-enable button after 10 seconds (fallback)
        setTimeout(function () {
            submitBtn.removeClass('loading').prop('disabled', false);
        }, 10000);
    });

    // Character counter for textareas
    $('textarea[maxlength]').each(function () {
        var textarea = $(this);
        var maxLength = textarea.attr('maxlength');
        var currentLength = textarea.val().length;

        var counter = $('<div class="char-counter mt-1">' +
            '<span class="current">' + currentLength + '</span> / ' +
            '<span class="max">' + maxLength + '</span> characters' +
            '</div>');

        textarea.after(counter);

        textarea.on('input', function () {
            var length = $(this).val().length;
            counter.find('.current').text(length);

            if (length > maxLength * 0.9) {
                counter.addClass('text-warning');
            } else {
                counter.removeClass('text-warning');
            }

            if (length > maxLength) {
                counter.addClass('text-danger').removeClass('text-warning');
            } else {
                counter.removeClass('text-danger');
            }
        });
    });

    // Smooth scrolling for anchor links
    $('a[href^="#"]').on('click', function (e) {
        e.preventDefault();
        var target = $($(this).attr('href'));
        if (target.length) {
            $('html, body').animate({
                scrollTop: target.offset().top - 70
            }, 500);
        }
    });

    // Rating stars interaction
    $('.rating-display').each(function () {
        var rating = $(this).data('rating');
        var stars = $(this).find('.star');

        stars.each(function (index) {
            if (index < rating) {
                $(this).addClass('filled');
            }
        });
    });

    // Search form auto-submit delay
    var searchTimeout;
    $('#searchTerm').on('input', function () {
        clearTimeout(searchTimeout);
        var form = $(this).closest('form');

        searchTimeout = setTimeout(function () {
            if ($('#searchTerm').val().length >= 3 || $('#searchTerm').val().length === 0) {
                form.submit();
            }
        }, 1000);
    });

    // Confirm dialogs for dangerous actions
    $('.btn-danger[type="submit"]').on('click', function (e) {
        e.preventDefault();
        var form = $(this).closest('form');
        var action = $(this).text().trim();

        if (confirm('Are you sure you want to perform this action: ' + action + '?')) {
            form.submit();
        }
    });

    // Image lazy loading fallback
    $('img[data-src]').each(function () {
        var img = $(this);
        img.attr('src', img.data('src')).removeAttr('data-src');
    });

    // Mobile menu improvements
    $('.navbar-toggler').on('click', function () {
        $(this).toggleClass('active');
    });

    // Dynamic form field validation
    $('.form-control, .form-select').on('blur', function () {
        validateField($(this));
    });

    function validateField(field) {
        var isValid = field[0].checkValidity();
        var feedbackElement = field.siblings('.invalid-feedback');

        if (!isValid) {
            field.addClass('is-invalid').removeClass('is-valid');
            if (feedbackElement.length === 0) {
                field.after('<div class="invalid-feedback">' + field[0].validationMessage + '</div>');
            }
        } else {
            field.addClass('is-valid').removeClass('is-invalid');
            feedbackElement.remove();
        }
    }

    // AJAX form submissions
    $('.ajax-form').on('submit', function (e) {
        e.preventDefault();
        var form = $(this);
        var url = form.attr('action');
        var method = form.attr('method') || 'POST';
        var data = form.serialize();

        $.ajax({
            url: url,
            type: method,
            data: data,
            success: function (response) {
                if (response.success) {
                    showNotification('success', response.message || 'Operation completed successfully!');
                    if (response.redirect) {
                        window.location.href = response.redirect;
                    }
                } else {
                    showNotification('error', response.message || 'An error occurred.');
                }
            },
            error: function () {
                showNotification('error', 'An error occurred while communicating with the server.');
            }
        });
    });

    // Notification system
    function showNotification(type, message) {
        var alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
        var icon = type === 'success' ? 'fa-check-circle' : 'fa-exclamation-circle';

        var notification = $('<div class="alert ' + alertClass + ' alert-dismissible fade show notification" role="alert">' +
            '<i class="fas ' + icon + '"></i> ' + message +
            '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
            '</div>');

        $('.container').first().prepend(notification);

        setTimeout(function () {
            notification.fadeOut(500, function () {
                $(this).remove();
            });
        }, 5000);
    }

    // Book list view enhancements
    if ($('.book-list-view').length > 0) {
        // View mode toggle (grid/list)
        $('.view-toggle').on('click', function (e) {
            e.preventDefault();
            var mode = $(this).data('mode');
            $('.book-list-view').removeClass('grid-view list-view').addClass(mode + '-view');
            $('.view-toggle').removeClass('active');
            $(this).addClass('active');

            localStorage.setItem('bookListViewMode', mode);
        });

        // Restore view mode
        var savedMode = localStorage.getItem('bookListViewMode');
        if (savedMode) {
            $('.book-list-view').removeClass('grid-view list-view').addClass(savedMode + '-view');
            $('.view-toggle').removeClass('active');
            $('.view-toggle[data-mode="' + savedMode + '"]').addClass('active');
        }
    }

    // Keyboard shortcuts
    $(document).on('keydown', function (e) {
        // Ctrl/Cmd + K for search
        if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
            e.preventDefault();
            $('#searchTerm').focus();
        }

        // Escape key to close modals/dropdowns
        if (e.key === 'Escape') {
            $('.dropdown-menu.show').removeClass('show');
            $('.modal.show').modal('hide');
        }
    });

    // Progressive enhancement for voting
    $('.vote-btn').on('click', function (e) {
        if (!$(this).hasClass('ajax-enabled')) {
            // Fallback to form submission for non-AJAX voting
            return true;
        }
    });

    // Copy to clipboard functionality
    $('.copy-btn').on('click', function (e) {
        e.preventDefault();
        var text = $(this).data('text') || $(this).text();

        if (navigator.clipboard) {
            navigator.clipboard.writeText(text).then(function () {
                showNotification('success', 'Copied to clipboard!');
            });
        } else {
            // Fallback for older browsers
            var textArea = document.createElement('textarea');
            textArea.value = text;
            document.body.appendChild(textArea);
            textArea.select();
            document.execCommand('copy');
            document.body.removeChild(textArea);
            showNotification('success', 'Copied to clipboard!');
        }
    });

    // Initialize any additional components
    initializeComponents();
});

function initializeComponents() {
    // Initialize any third-party components here
    console.log('Book Review App initialized successfully!');
}

// Utility functions
window.BookReviewApp = {
    showNotification: function (type, message) {
        // Expose notification function globally
        var alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
        var icon = type === 'success' ? 'fa-check-circle' : 'fa-exclamation-circle';

        var notification = $('<div class="alert ' + alertClass + ' alert-dismissible fade show notification" role="alert">' +
            '<i class="fas ' + icon + '"></i> ' + message +
            '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
            '</div>');

        $('.container').first().prepend(notification);

        setTimeout(function () {
            notification.fadeOut(500, function () {
                $(this).remove();
            });
        }, 5000);
    }
};