// check allocation table if customerID matches to only show their fridge faults
// also don't show fault status' that have already been delivered

const mainContainer = document.querySelector('.main-container');

faultStatusData.forEach(fault => {
    // Create the container for each fault status
    const statusContainer = document.createElement('div');
    statusContainer.classList.add('status-container');

    // Create the fridge header
    const fridgeHeader = document.createElement('h2');
    const fridgeLabel = document.createElement('label');
    fridgeLabel.id = 'header';
    fridgeLabel.textContent = fault.fridgeNumber;
    fridgeHeader.appendChild(fridgeLabel);

    // Create the description row
    const descriptionRow = createRow('Description:', fault.description, 'lblDescriptionTitle', 'lblDescription');

    // Create the technician row
    const technicianRow = createRow('Technician:', fault.technician, 'lblTechnicianTitle', 'lblTechnicianName');

    // Create status row
    const statusRow = document.createElement('div');
    statusRow.classList.add('status-row');

    const statusLabel = document.createElement('div');
    statusLabel.classList.add('status-label');
    const statusTitle = document.createElement('label');
    statusTitle.id = 'lblStatusTitle';
    statusTitle.textContent = 'Status';
    statusLabel.appendChild(statusTitle);

    const statusValue = document.createElement('div');
    statusValue.classList.add('status-value');
    statusValue.style.color = fault.statusColor;
    statusValue.style.fontWeight = 'bold';
    const statusText = document.createElement('label');
    statusText.id = 'lblStatus';
    statusText.textContent = fault.status;
    statusValue.appendChild(statusText);

    statusRow.appendChild(statusLabel);
    statusRow.appendChild(statusValue);

    // Append all elements to the status container
    statusContainer.appendChild(fridgeHeader);
    statusContainer.appendChild(descriptionRow);
    statusContainer.appendChild(technicianRow);
    statusContainer.appendChild(statusRow);

    // Append the status container to the main container
    mainContainer.appendChild(statusContainer);
})