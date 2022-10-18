

const registerForm = document.getElementById('register');
const errorElement = document.getElementById('error');

registerForm.addEventListener('submit', onFormSubmit);


function onFormSubmit(e) {

    e.preventDefault();

    let userName = registerForm.elements['userName'].value;
    let password = registerForm.elements['password'].value;
    let confirmPassword = registerForm.elements['confirmPassword'].value;
    
    if (userName.length < 5) {
        showErrorMessage('Username should be at least 5 characters long.');
        return;
    }
    if (password.length < 6) {
        showErrorMessage('Password should be at least 6 characters long.');
        return;
    }
    if (password !== confirmPassword) {
        showErrorMessage('Password and Confirm Password don\'t match.');
        return;
    }

    registerForm.submit();
}


function showErrorMessage(errorMsg) {
    errorElement.textContent = errorMsg;
}