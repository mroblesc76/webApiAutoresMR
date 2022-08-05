using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.DTO
{
    public class AutorCreacionDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [StringLength(maximumLength: 50, ErrorMessage = "el campo {0} no debe tener mas de {1} carácteres.")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }
    }
}
