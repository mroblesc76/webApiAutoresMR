using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiAutores.DTO;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/autores")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public AutoresController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        //estos son 3 rutas para el mismo endpoint
        [HttpGet(Name = "obtenerTodosLosAutores")] // api/autores
        //[HttpGet("listado")] // api/autores/listado
        //[HttpGet("/listado")] // listado
        [AllowAnonymous]
        public async Task<List<AutorDTO>> Get()
        {
            var autores = await context.Autores.ToListAsync();
            // implementando automapper
            return mapper.Map<List<AutorDTO>>(autores);

        }

        [HttpGet("{id:int}", Name = "obtenerAutoresPorId")]
        public async Task<ActionResult<AutorDTOConLibros>> Get(int id)
        {
            var autor = await context.Autores
                .Include(autorDB=>autorDB.AutorLibro)
                .ThenInclude(autorLibroDB=>autorLibroDB.Libro)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (autor==null)
            {
                return NotFound();
            }

            // implementa mapper
            return mapper.Map<AutorDTOConLibros>(autor);

            //return Ok(autor);
        }

        [HttpGet("{nombre}", Name = "obtenerAutorPorNombre")]
        public async Task<ActionResult<List<AutorDTO>>> Get(string nombre)
        {
            var autores = await context.Autores.Where(context => context.Nombre.Contains(nombre)).ToListAsync();

            return mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpPost(Name = "crearAutor")]
        public async Task<ActionResult> Post(AutorCreacionDTO autorCreacionDTO) 
        {
            var _existe = await context.Autores.AnyAsync(x=>x.Nombre==autorCreacionDTO.Nombre);
            if (_existe) 
            {
                return BadRequest($"Ya existe un autor con el nombre {autorCreacionDTO.Nombre}");
            }

            // implementando automapper
            var autor = mapper.Map<Autor>(autorCreacionDTO);

            context.Add(autor);
            await context.SaveChangesAsync();
            //return Ok();

            var autorDTO = mapper.Map<AutorDTO>(autor);

            return CreatedAtRoute("obtenerAutoresPorId", new { id = autor.Id }, autorDTO );
        }

        [HttpPut("{id:int}", Name = "actualizarAutor")] // api/autores/1
        public async Task<ActionResult> Put(AutorCreacionDTO autorCreacionDto, int id) 
        {
            //if (autor.Id != id) {
            //    return BadRequest("El id del autor no coincide con el id de la URL.");
            //}

            var existe = await context.Autores.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            var autor = mapper.Map<Autor>(autorCreacionDto);
            autor.Id = id;

            context.Update(autor);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "eliminarAutor")] // // api/autores/1
        public async Task<ActionResult> Delete(int id) 
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == id);
            if (!existe) {
                return NotFound();
            }

            context.Remove(new Autor() {Id=id });
            await context.SaveChangesAsync();
            return NoContent();

        }
    }
}
