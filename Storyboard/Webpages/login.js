document.addEventListener("DOMContentLoaded", function() {
    const loginForm = document.getElementById('loginForm');

    loginForm.addEventListener('submit', function(event){
        event.preventDefault(); // prevent the default form submission

        // Get the input values
        const email = document.getElementById('email').value;
        const password = document.getElementById('password').value;

        // Validate the login using the function from data.js
        if (validateLogin(email, password)){

            // Check user role and load the proper dashboard

            window.location.href = "faultDashboard.html";
        }
        else {
            alert('Invalid email or password');
        }
    });
});