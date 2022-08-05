using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiAutores.DTO;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "ObtenerRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DatoHATEOASDTO>>> Get()
        {
            var datosHateOAS = new List<DatoHATEOASDTO>();

            // con esta linea podemos controlar el acceso de los metodos de la api
            var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");

            datosHateOAS.Add(new DatoHATEOASDTO(enlace: Url.Link("ObtenerRoot", new { }), descripcion: "self", metodo: "GET"));
            datosHateOAS.Add(new DatoHATEOASDTO(enlace: Url.Link("obtenerTodosLosAutores", new { }), descripcion: "autores", metodo: "GET"));

            if (esAdmin.Succeeded)
            {
                datosHateOAS.Add(new DatoHATEOASDTO(enlace: Url.Link("crearAutor", new { }), descripcion: "autor-crear", metodo: "POST"));
                datosHateOAS.Add(new DatoHATEOASDTO(enlace: Url.Link("crearLibro", new { }), descripcion: "libro-crear", metodo: "POST"));
            }

            return datosHateOAS;
        }
    }
}