import { AbstractControl, ValidationErrors, ValidatorFn, FormGroup } from "@angular/forms";


export class CustomValidators{
    static bannedUsernames (bannedWords: string[]): ValidatorFn{
        return (control: AbstractControl): ValidationErrors | null => {
            const value  = control.value?.toLowerCase();
            return bannedWords.some(word => value.includes(word))? {bannedWords: true}: null;
        }
    }

    static passwordStrenght(): ValidatorFn{
        return (control: AbstractControl): ValidationErrors | null => {
            const value = control.value ;
            const hasNumber = /\d/.test(value);
            const hasSymbol = /[!@#$%^&*(),.?":{}|<>]/.test(value);
            const isValid = value?.length >= 8 && hasNumber &&hasSymbol;
            return !isValid ? {weakPassword : true}: null;
        }
    }

    static matchPassword (passwordField :string, confirmField:string){
        return (group : FormGroup): ValidationErrors | null => {
            const password = group.get(passwordField)?.value;
            const confirm = group.get(confirmField)?.value;
            return password !== confirm? {passwordMismatch: true}: null;
        }
    }
}