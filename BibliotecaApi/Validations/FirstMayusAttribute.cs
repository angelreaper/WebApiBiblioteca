using System.ComponentModel.DataAnnotations;

namespace BibliotecaApi.Validations
{
    public class FirstMayusAttribute:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString())){ 
                return ValidationResult.Success;// esto es para que no intervenga con un atributo [Required] que valida si esta en blanco el campo
            }

            var valueString = value.ToString()!;// el ! es para validar que no venga vacio
            var firstLetter = valueString[0].ToString();// sacamos el primer valor del string

            if (firstLetter != firstLetter.ToUpper())//va lido si la letra optenida no esta em mayuscula
            {
                return new ValidationResult("La primera letra debe ser mayúscula");
            }
            return ValidationResult.Success;

        }
    }
}
