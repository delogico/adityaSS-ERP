$(document).ready(function () {
    if ($('#CRI_Id').val() != null) {

    }
    $('#accountForm')
        .bootstrapValidator({
          
            // Only disabled elements are excluded
            // The invisible elements belonging to inactive tabs must be validated
            excluded: [':disabled'],
            feedbackIcons: {
                valid: 'glyphicon glyphicon-ok',
                invalid: 'glyphicon glyphicon-remove',
                validating: 'glyphicon glyphicon-refresh'
            },
            fields: {             
                    
                CRI_Basic: {
                    feedbackIcons: false,
                    validators: {
                        notEmpty: {
                            message: 'Basic is required'
                        }
                    }
                },
                CRI_DA: {
                    feedbackIcons: false,
                    validators: {
                        notEmpty: {
                            message: 'DA is required'
                        }
                    }
                },
                CRI_Total: {
                    feedbackIcons: false,
                    validators: {
                        greaterThan: {
                            message: 'Number of post must be greater than or equal to 1',
                            min: 1,                            
                        }
                    }
                }
            }
        })
        // Called when a field is invalid
        .on('error.field.bv', function(e, data) {
            // data.element --> The field element
            debugger;
            var $tabPane = data.element.parents('.tab-pane'),
                tabId    = $tabPane.attr('id');

            $('a[href="#' + tabId + '"][data-toggle="tab"]')
                .parent()
                .find('i')
                .removeClass('fa-check')
                .addClass('fa-times');
        })
        // Called when a field is valid
        .on('success.field.bv', function (e, data) {
            debugger;
            // data.bv      --> The BootstrapValidator instance
            // data.element --> The field element

            var $tabPane = data.element.parents('.tab-pane'),
                tabId    = $tabPane.attr('id'),
                $icon    = $('a[href="#' + tabId + '"][data-toggle="tab"]')
                            .parent()
                            .find('i')
                            .removeClass('fa-check fa-times');

            // Check if the submit button is clicked
            if (data.bv.getSubmitButton()) {
                debugger;
                // Check if all fields in tab are valid
                var isValidTab = data.bv.isValidContainer($tabPane);
                $icon.addClass(isValidTab ? 'fa-check' : 'fa-times');
            }
        });
   
});