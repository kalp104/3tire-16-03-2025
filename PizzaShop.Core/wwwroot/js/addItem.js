// addItem.js
$(document).ready(function () {
    let selectedGroups = {};

    $('.modifier-group-item').click(function (e) {
        e.preventDefault();
        var modifierGroupId = $(this).data('modifiergroup-id');
        var groupName = $(this).text().trim();
        var $checkbox = $(this).find('.modifier-checkbox');

        $checkbox.prop('checked', !$checkbox.prop('checked'));

        if (!selectedGroups[modifierGroupId]) {
            $.ajax({
                url: '/Menu/GetModifiersByGroup',
                type: 'GET',
                data: { modifierGroupId: modifierGroupId },
                success: function (response) {
                    if (response && response.length > 0) {
                        selectedGroups[modifierGroupId] = {
                            name: groupName,
                            minValue: 0,
                            maxValue: 0,
                            modifiers: response.map(modifier => ({
                                modifierId: modifier.modifierid,
                                modifiername: modifier.modifiername || 'Unnamed Modifier',
                                modifierrate: modifier.modifierrate
                            }))
                        };
                    }
                    renderModifiers();
                },
                error: function (xhr, status, error) {
                    $('#modifiers-container').html(`
                        <div class="p-2 m-2alert alert-danger">
                            Error loading modifiers for ${groupName}: ${error}
                        </div>
                    `);
                }
            });
        } else {
            delete selectedGroups[modifierGroupId];
            renderModifiers();
        }
    });

    function renderModifiers() {
        $('#modifiers-container').empty();
        if (Object.keys(selectedGroups).length > 0) {
            let html = '';
            for (let groupId in selectedGroups) {
                let group = selectedGroups[groupId];
                html += `
                <div class="mb-3">
                    <div class="px-3 d-flex justify-content-between">
                        <div style="font-size:20px">${group.name}</div>
                        <div class="trash-icon" style="font-size:20px; cursor:pointer" data-group-id="${groupId}">
                            <i class="bi bi-trash-fill"></i>
                        </div>
                    </div>
                    <div class="px-3 pb-1 d-flex justify-content-between mt-1">
                        <select class="form-select min-value" data-group-id="${groupId}" style="width: 80px;">
                            <option value="0" ${group.minValue === 0 ? 'selected' : ''}>0</option>
                            <option value="1" ${group.minValue === 1 ? 'selected' : ''}>1</option>
                            <option value="2" ${group.minValue === 2 ? 'selected' : ''}>2</option>
                            <option value="3" ${group.minValue === 3 ? 'selected' : ''}>3</option>
                        </select>
                        <select class="form-select max-value" data-group-id="${groupId}" style="width: 80px;">
                            <option value="0" ${group.maxValue === 0 ? 'selected' : ''}>0</option>
                            <option value="1" ${group.maxValue === 1 ? 'selected' : ''}>1</option>
                            <option value="2" ${group.maxValue === 2 ? 'selected' : ''}>2</option>
                            <option value="3" ${group.maxValue === 3 ? 'selected' : ''}>3</option>
                        </select>
                    </div>
                    <ul>`;
                group.modifiers.forEach(function (modifier) {
                    html += `
                    <li class="px-3 d-flex justify-content-between" style="font-size:14px" data-modifier-id="${modifier.modifierId}">
                        <span>${modifier.modifiername}</span>
                        <span>${modifier.modifierrate}</span>
                    </li>`;
                });
                html += `</ul></div>`;
            }
            $('#modifiers-container').html(html);

            $('.trash-icon').click(function () {
                const groupId = $(this).data('group-id');
                delete selectedGroups[groupId];
                renderModifiers();
            });

            $('.min-value').change(function () {
                const groupId = $(this).data('group-id');
                selectedGroups[groupId].minValue = parseInt($(this).val());
            });

            $('.max-value').change(function () {
                const groupId = $(this).data('group-id');
                selectedGroups[groupId].maxValue = parseInt($(this).val());
            });
        } else {
            $('#modifiers-container').html(`
                <div class="p-2 m-2 alert alert-info">No modifier groups selected</div>
            `);
        }

        $('.modifier-checkbox').each(function () {
            var groupId = $(this).data('modifiergroup-id');
            $(this).prop('checked', !!selectedGroups[groupId]);
        });
    }

    $('#addItemForm').submit(function (e) {
        console.log('Submitting form, selectedGroups:', selectedGroups);
        $('#selectedModifierGroups').val(JSON.stringify(selectedGroups));
    });
});