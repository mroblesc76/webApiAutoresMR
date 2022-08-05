using System.Collections.Generic;

namespace WebApiAutores.DTO
{
    public class AutorDTOConLibros : AutorDTO
    {
        public List<LibroDTO> Libros { get; set; }
    }
}
