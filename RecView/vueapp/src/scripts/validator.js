export function passwordComplexity(value) {
    if (!value) return false;
    const hasUpperCase = /[A-Z]/.test(value);
    const hasDigit = /\d/.test(value);
    const hasSpecialChar = /[!@#$%^&*(),.?":{}|<>]/.test(value);
    return hasUpperCase && hasDigit && hasSpecialChar;
}

export const passwordComplexityValidator = {
    $validator: passwordComplexity,
    $message: 'Password must contain at least 1 special symbol, 1 digit and 1 upperase letter!'
};
