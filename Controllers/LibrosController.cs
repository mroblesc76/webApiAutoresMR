using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApiAutores.DTO;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/libros")]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public LibrosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("{id:int}", Name = "obtenerLibrosPorId")]
        public async Task<ActionResult<LibroDTOConAutores>> Get(int id)
        {
            //return await context.Libros.Include(x=>x.Autor).FirstOrDefaultAsync(x => x.Id == id);

            //var libro = await context.Libros.Include(comentarioDB=>comentarioDB.Comentarios).FirstOrDefaultAsync(libroDB => libroDB.Id == id);

            var libro = await context.Libros
                .Include(libroDB => libroDB.AutorLibro)
                .ThenInclude(autorLibroDB => autorLibroDB.Autor)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (libro == null)
            {
                return NotFound();
            }

            libro.AutorLibro = libro.AutorLibro.OrderBy(x => x.Orden).ToList();

            return mapper.Map<LibroDTOConAutores>(libro);

        }

        [HttpPost(Name ="crearLibro")]
        public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO)
        {
            if (libroCreacionDTO.AutoresIds == null)
            {
                return BadRequest("No se puede crear un libro sin autores.");
            }

            var autoresIds = await context.Autores.
                Where(autorBD => libroCreacionDTO.AutoresIds.Contains(autorBD.Id)).
                Select(x => x.Id).ToListAsync();

            if (libroCreacionDTO.AutoresIds.Count() != autoresIds.Count())
            {
                return BadRequest("No existe uno de los autores enviados.");
            }

            var libro = mapper.Map<Libro>(libroCreacionDTO);

            AsignarOrdenAutores(libro);

            context.Add(libro);
            await context.SaveChangesAsync();

            var libroDTO = mapper.Map<LibroDTO>(libro);

            return CreatedAtRoute("obtenerLibrosPorId", new { id = libro.Id }, libroDTO);

            //return Ok();
        }

        [HttpPut("{id:int}", Name ="actualizarLibro")]
        public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
        {
            var libroDB = await context.Libros
                .Include(x => x.AutorLibro)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (libroDB == null)
            {
                return NotFound();
            }

            libroDB = mapper.Map(libroCreacionDTO, libroDB);

            AsignarOrdenAutores(libroDB);

            await context.SaveChangesAsync();
            return NoContent();

        }

        private void AsignarOrdenAutores(Libro libro)
        {

            if (libro.AutorLibro != null)
            {
                for (int i = 0; i < libro.AutorLibro.Count; i++)
                {
                    libro.AutorLibro[i].Orden = i;
                }
            }

        }

        [HttpPatch("{id:int}", Name ="patchLibro")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<LibroPathDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var libroDB = await context.Libros.FirstOrDefaultAsync(x => x.Id == id);
            if (libroDB == null) 
            {
                return NotFound();
            }

            var libroDTO = mapper.Map<LibroPathDTO>(libroDB);
            patchDocument.ApplyTo(libroDTO, ModelState);

            var esValido = TryValidateModel(libroDTO);
            if (!esValido) 
            {
                return BadRequest(ModelState);
            }

            mapper.Map(libroDTO, libroDB);
            await context.SaveChangesAsync();
            return NoContent();

        }

        [HttpDelete("{id:int}", Name ="borrarLibro")]
        public async Task<ActionResult> Delete(int id)
        {
            // este método borra en cascada --> libro --> comentarios --> libroAutores
            var existe = await context.Libros.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Libro() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();

        }



    }
}
