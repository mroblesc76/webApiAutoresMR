using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    [Route("api/libros/{libroId:int}/comentarios")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ComentariosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public ComentariosController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet(Name = "obtenerComentario")]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId) 
        {
            var existe = await context.Libros.AnyAsync(x => x.Id == libroId);
            if (!existe)
            {
                return NotFound();
            }

            var comentarios = await context.Comentarios.Where(x=>x.LibroId==libroId).ToListAsync();

            return mapper.Map<List<ComentarioDTO>>(comentarios);

        }

        [HttpGet("{id:int}", Name = "obtenerComentarioPorId")]
        public async Task<ActionResult<ComentarioDTO>> GetPorId(int id) 
        {
            var comentario = await context.Comentarios.FirstOrDefaultAsync(comentarioDB=>comentarioDB.Id == id);
            if (comentario == null)
            {
                return NotFound();
            }

            return mapper.Map<ComentarioDTO>(comentario);
            
        }

        [HttpPost(Name = "crearComentario")]
        [AllowAnonymous]
        public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO comentarioCreacionDTO) 
        {
            var emailClaim = HttpContext.User.Claims.Where(x=>x.Type=="email").FirstOrDefault();
            var email = emailClaim.Value;
            var usuario = await userManager.FindByEmailAsync(email);
            var usuarioId = usuario.Id;

            var existe = await context.Libros.AnyAsync(x=>x.Id==libroId);
            if (!existe) 
            {
                return NotFound();
            }

            var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.LibroId=libroId;
            comentario.UsuarioId=usuarioId;
            context.Add(comentario);
            await context.SaveChangesAsync();

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);

            return CreatedAtRoute("obtenerComentarioPorId", new { id = comentario.Id, libroId }, comentarioDTO);

            //return Ok();

        }

        [HttpPut("{id:int}", Name = "actualizarComentario")]
        public async Task<ActionResult> Put(int libroId, int id, ComentarioCreacionDTO comentarioCreacionDTO) 
        {
            var existe = await context.Libros.AnyAsync(x => x.Id == libroId);
            if (!existe)
            {
                return NotFound();
            }
            var existeComentario = await context.Comentarios.AnyAsync(comentarioDB => comentarioDB.Id == id);

            if (!existeComentario)
            {
                return NotFound();
            }

            var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.Id = id;
            comentario.LibroId = libroId;

            context.Update(comentario);
            await context.SaveChangesAsync();
            return NoContent();


        }
    }
}
