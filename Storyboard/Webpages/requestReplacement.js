// count query for how many fridges the logged in customer has in maintenance
let fridgesInMaintenance = 2;

window.onload = function() {
    // Get the elements
    const fridgeCountElement = document.getElementById('fridgeCount');
    const requestButton = document.getElementById('requestButton');

    // Update the bold text with the number of fridges in maintenance
    fridgeCountElement.textContent = fridgesInMaintenance;

    // Check if number of fridges and update the button color
    if (fridgesInMaintenance > 0) {
        requestButton.style.backgroundColor = 'green';
        requestButton.disabled = false;
    } else {
        requestButton.style.backgroundColor = 'grey';
        requestButton.disabled = true;
    }

};