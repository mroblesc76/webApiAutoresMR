using AutoMapper;
using System.Collections.Generic;
using WebApiAutores.DTO;
using WebApiAutores.Entidades;

namespace WebApiAutores.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AutorCreacionDTO, Autor>();
            
            CreateMap<Autor, AutorDTO>();

            CreateMap<Autor, AutorDTOConLibros>()
                .ForMember(autorDTO => autorDTO.Libros, opciones => opciones
                .MapFrom(MapAutorDTOLibros));
            
            CreateMap<LibroCreacionDTO, Libro>()
                .ForMember(libro=>libro.AutorLibro, opciones=>opciones
                .MapFrom(MapAutoresLibros));

            CreateMap<Libro, LibroDTO>();

            CreateMap<Libro, LibroDTOConAutores>()
                .ForMember(libroDTO => libroDTO.Autores, opciones => opciones
                .MapFrom(MapLibroDTOAutores));

            CreateMap<ComentarioCreacionDTO, Comentario>();
            CreateMap<Comentario, ComentarioDTO>();

            CreateMap<LibroPathDTO, Libro>().ReverseMap();
        }

        private List<AutorLibro> MapAutoresLibros(LibroCreacionDTO libroCreacionDTO, Libro libro) 
        {
            var resultado = new List<AutorLibro>();

            if (libroCreacionDTO.AutoresIds == null) { return resultado; }

            foreach (var autorId in libroCreacionDTO.AutoresIds)
            {
                resultado.Add(new AutorLibro() { AutorId = autorId });
            }

            return resultado;
            
        }

        private List<AutorDTO> MapLibroDTOAutores(Libro libro, LibroDTO libroDTO)
        {
            var resultado = new List<AutorDTO>();

            if (libro.AutorLibro == null) { return resultado; }

            foreach (var autorlibro in libro.AutorLibro)
            {
                resultado.Add(new AutorDTO()
                {
                    Id=autorlibro.AutorId,
                    Nombre=autorlibro.Autor.Nombre
                });
            }

            return resultado;
        }

        private List<LibroDTO> MapAutorDTOLibros(Autor autor, AutorDTO autorDTO) 
        {
            var resultado = new List<LibroDTO>();

            if (autor.AutorLibro == null) { return resultado; }

            foreach (var autorlibro in autor.AutorLibro)
            {
                resultado.Add(new LibroDTO()
                {
                    Id =autorlibro.LibroId,
                    Titulo=autorlibro.Libro.Titulo
                });
            }

            return resultado;
        }
    }
}

