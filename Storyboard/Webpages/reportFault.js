// how is the technician for each fault set?
// do they choose, get assigned or is it dependant on a customer/technician relation

const titleMinLength = 5;
const descriptionMinLength = 5;

document.addEventListener("DOMContentLoaded", function(){
    const reportForm = document.getElementById('reportForm');

    reportForm.addEventListener('submit', function(event){
        event.preventDefault(); // prevent the default form submission

        // get values
        const title = document.getElementById('title').value;
        const description = document.getElementById('description').value;

        // check values: character count, not only numerical characters
        if (isUnderCharacterCount(title, titleMinLength)){
            alert(`Title cannot be less than ${titleMinLength} characters. Try again.`); // use variables for these error messages
            return;
        }
        if (isOnlyNumeric(title)) {
            alert('Title cannot be only numerical. Try again.');
            return;
        }

        if (isUnderCharacterCount(description, descriptionMinLength)){
            alert(`Description cannot be less than ${descriptionMinLength} characters. Try again.`);
            return;
        }
        if (isOnlyNumeric(description)) {
            alert('Description cannot be only numerical. Try again.');
            return;
        }
    
        // if all checks are good, create object to send to database
    
        // after creating report, send success message to user
    });
});

function isUnderCharacterCount(text, minCount){
    if (text.length < minCount) return true;
    else return false;
}

function isOnlyNumeric(text){
    if (/^\d+$/.test(text)) return true;
    else return false;
}