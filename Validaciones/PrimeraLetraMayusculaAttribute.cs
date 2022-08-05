﻿using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Validaciones
{
    public class PrimeraLetraMayusculaAttribute:ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString())) 
            {
                return ValidationResult.Success;
                
            }

            var primeraLetra = value.ToString()[0].ToString();
            if (primeraLetra != primeraLetra.ToUpper()) 
            {
                return new ValidationResult("La Primera Letra Debe Ser Mayúscula.");
            }

            return ValidationResult.Success;

            //return base.IsValid(value, validationContext);
        }
    }
}
