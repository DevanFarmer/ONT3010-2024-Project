const appData = {
    userID: null
}

const Roles = {
    User: 'Admin',
    Technician: 'Technician'
};

const faultStatus = {
    InReview: 'In-Review',
    ReadyToDeliver: 'Read To Deliver'
}

const user = [{
    userID: 1,
    firstname: "John",
    lastname: "Doe",
    email: "JD@email.com",
    password: "12345",
},
{
    userID: 2,
    firstname: "John",
    lastname: "Doe",
    email: "JDUser@email.com",
    password: "12345",
}]

const employee = {
    employeeID: 1,
    userID: 1,
    roleID: Roles.Technician
}

const faultReport = {
    faultID: 1,
    allocationID: 1,
    employeeID: 1,
    faultStatus: faultStatus.InReview,
    faultDescription: 'No work no more.',
    reportDate: '01/01/2024',
    resolutionDate: null
}

const fridgeAllocation = {
    allocationID: 1,
    fridgeID: 1,
    customerID: 1,
    allocationDate: '01/01/2024',
    returnDate: null
}

const fridge = [
    {
        fridgeID: 1,
        supplierID: 1,
        statusID: 1,
        locationID: 1,
        fridgeModel: "Model1",
        serialNumber: "0123456789",
        dateAcquired: "01/01/2024"
    }
];

const status = [
    {
        statusID: 1,
        statusName: "Available"
    },
    {
        statusID: 2,
        statusName: "Allocated"
    },
    {
        statusID: 3,
        statusName: "Faulty"
    },
    {
        statusID: 4,
        statusName: "Scrapped"
    }
    
];

const customer = {
    customerID: 1,
    userID: 2
}

const faultStatusData = [
    {
        fridgeNumber: "Fridge No.1",
        description: "Title Description given by user.",
        technician: "L. Thomas",
        status: "In-review",
        statusColor: "red"
    },
    {
        fridgeNumber: "Fridge No.2",
        description: "Another description.",
        technician: "J. Doe",
        status: "Completed",
        statusColor: "green"
    }
];

function validateLogin(inputEmail, inputPassword){
    for (let element of user){
        if (element.email == inputEmail && element.password == inputPassword) {
            appData.userID = element.userID
            return true;
        }
    }
    return false;
}

function createRow(title, value, titleID, valueID){
    const row = document.createElement('div');
    row.classList.add('status-row');

    const labelContainer = document.createElement('div');
    labelContainer.classList.add('status-label');
    const label = document.createElement('label');
    label.id = titleID;
    label.textContent = title;
    labelContainer.appendChild(label);

    const valueContainer = document.createElement('div');
    valueContainer.classList.add('status-value');
    const valueLabel = document.createElement('label');
    valueLabel.id = valueID;
    valueLabel.textContent = value;
    valueContainer.appendChild(valueLabel);

    row.appendChild(labelContainer);
    row.appendChild(valueContainer);

    return row;
}