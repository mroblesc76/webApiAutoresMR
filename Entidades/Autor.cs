using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades
{
    public class Autor
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        //public List<Libro> Libros { get; set; }
        public List<AutorLibro> AutorLibro { get; set; }
    }
}
