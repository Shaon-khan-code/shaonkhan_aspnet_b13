// Reusable reCAPTCHA v3 helper.
// Usage: attach to a form's submit handler:
//   MaleFashionRecaptcha.protect('#registerForm', 'siteKeyValue', 'register', '#RecaptchaToken');
window.MaleFashionRecaptcha = {
    protect: function (formSelector, siteKey, action, tokenFieldSelector) {
        var form = document.querySelector(formSelector);
        if (!form) return;

        form.addEventListener('submit', function (e) {
            var tokenField = document.querySelector(tokenFieldSelector);
            if (tokenField && tokenField.value) {
                return; // token already fetched, allow normal submit
            }

            e.preventDefault();

            grecaptcha.ready(function () {
                grecaptcha.execute(siteKey, { action: action }).then(function (token) {
                    if (tokenField) {
                        tokenField.value = token;
                    }
                    form.submit();
                });
            });
        });
    }
};