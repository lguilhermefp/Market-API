using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api_asp_net_4.Data;

namespace api_asp_net_4.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly IJwtAuthenticationManager jwtAuthenticationManager;
        public UsuariosController (AppDbContext context, IJwtAuthenticationManager jwtAuthenticationManager)
        {
            this.jwtAuthenticationManager = jwtAuthenticationManager;
            this._context = context;
        }

        /// <summary>
        /// Retorna um token de acesso a api para usuarios autorizados
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns>Um token de acesso a api para usuarios autorizados</returns>
        /// <remarks>
        ///     Requisicao padrao:
        ///         Post api/Usuarios/authenticate
        ///         
        ///     No "body", insira sua Identificacao, Nome, Email e Senha
        /// 
        ///     Identificacao padrao:
        ///         { "Identificacao": "admin-123", "Nome": "admin", "Email": "admin@example.com", "Senha": "admin123" }
        ///     
        /// </remarks>
        /// <response code="200">Retorna um token de acesso a api</response>
        /// <response code="400">Se algum campo inserido possui valor invalido</response>
        /// <response code="401">Se o autor da requisicao nao possui autorizacao</response>
        /// <response code="500">Se houve falha de conexao com o banco de dados</response>
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] Usuario usuario)
        {
            try {
                var resultToken = await jwtAuthenticationManager.AuthenticateAsync(_context, usuario.Identificacao, usuario.Senha);
                if (resultToken == null)
                    return Unauthorized();

                return Ok(resultToken);
            }
            catch
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Retorna uma lista com detalhes de todos os usuarios
        /// </summary>
        /// <remarks>
        ///     Requisicao padrao:
        ///         Get api/Usuarios/
        /// </remarks>
        /// <returns>Lista com detalhes de todos os usuarios</returns>
        /// <response code="200">Retorna a lista requisitada</response>
        /// <response code="401">Se o autor da requisicao nao possui autorizacao</response>
        /// <response code="500">Se houve falha na conexao com o banco de dados</response>
        [ProducesResponseType(typeof(IEnumerable<Usuario>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuario()
        {
            return await _context.Usuarios.ToListAsync();
        }

        /// <summary>
        /// Retorna os detalhes do usuario requisitado
        /// </summary>
        /// <remarks>
        ///     Exemplo de requisicao:
        ///         GET api/Usuarios/abcd-1234
        /// </remarks>
        /// <param name="identificacao"></param>
        /// <returns>Os detalhes do usuario requisitado</returns>
        /// <response code="200">Retorna o usuario requisitado</response>
        /// <response code="400">Se não houver usuario com essa identificacao</response>
        /// <response code="401">Se o autor da requisicao nao possui autorizacao</response>
        /// <response code="500">Se houve falha de conexao com o banco de dados</response>
        [ProducesResponseType(typeof(Usuario), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [HttpGet("{identificacao}")]
        public async Task<ActionResult<Usuario>> GetUsuario(string identificacao)
        {
            var usuario = await _context.Usuarios.FindAsync(identificacao);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        /// <summary>
        /// Altera informacoes de um usuario existente
        /// </summary>
        /// <param name="identificacao"></param>
        /// <param name="usuario"></param>
        /// <remarks>
        ///     Exemplo de requisicao:
        ///         Put api/Usuarios/abcd-1234
        ///         
        ///         No "body", insira sua Identificacao e a atualizacao dos campos Nome, Email e Senha
        /// </remarks>
        /// <response code="204">Atualiza o usuario requisitado</response>
        /// <response code="400">Se nao existe usuario com Identificacao informada ou "body" possui valor invalido ou o formato e ilegivel</response>
        /// <response code="401">Se o autor da requisicao nao possui autorizacao</response>
        /// <response code="500">Se houve falha de conexao com o banco de dados</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [HttpPut("{identificacao}")]
        public async Task<IActionResult> PutUsuario(string identificacao, Usuario usuario)
        {
            usuario.Senha = EncodePasswordToBase64(usuario.Senha);
            if (identificacao != usuario.Identificacao)
            {
                return BadRequest();
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IdentificacaoNotAvailable(identificacao))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Cria um novo usuario
        /// </summary>
        /// <param name="usuario"></param>
        /// <remarks>
        ///     Requisicao padrao:
        ///         Post api/Usuarios/
        ///         
        ///         No "body", insira sua Identificacao, Nome, Email e Senha
        ///         
        ///         Campo "Identificacao" precisa ter 9 caracteres
        /// </remarks>
        /// <returns>Os detalhes do usuario criado</returns>
        /// <response code="201">Retorna os detalhes do usuario criado</response>
        /// <response code="400">Se algum dos campos do "body" da requisicao possui valor invalido ou o formato e ilegivel</response>
        /// <response code="401">Se o autor da requisicao nao possui autorizacao</response>
        /// <response code="409">Se ja houver um usuario com Identificacao ou Email fornecidos</response>
        /// <response code="500">Se houve falha de conexao com o banco de dados</response>
        [ProducesResponseType(typeof(Usuario), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            usuario.Senha = EncodePasswordToBase64(usuario.Senha);
            _context.Usuarios.Add(usuario);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (IdentificacaoOrEmailNotAvailable(usuario.Identificacao, usuario.Email))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUsuario", new { id = usuario.Identificacao }, usuario);
        }

        /// <summary>
        /// Exclui um usuario existente
        /// </summary>
        /// <param name="identificacao"></param>
        /// <remarks>
        ///     Exemplo de requisicao:
        ///         Delete api/Usuarios/abcd-1234
        /// </remarks>
        /// <returns>Retorna os detalhes do usuario excluido</returns>
        /// <response code="204">Exclui um produto existente</response>
        /// <response code="401">Se o autor da requisicao nao possui autorizacao</response>
        /// <response code="404">Se nao existe usuario com Identificacao informada</response>
        /// <response code="500">Se houve falha de conexao com o banco de dados</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [HttpDelete("{identificacao}")]
        public async Task<IActionResult> DeleteUsuario(string identificacao)
        {
            var usuario = await _context.Usuarios.FindAsync(identificacao);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [NonAction]
        public bool IdentificacaoOrEmailNotAvailable(string identificacao, string email)
        {
            return _context.Usuarios.Any(e => e.Identificacao == identificacao || e.Email == email);
        }

        [NonAction]
        public bool IdentificacaoNotAvailable(string identificacao)
        {
            return _context.Usuarios.Any(e => e.Identificacao == identificacao);
        }

        public static string EncodePasswordToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                if (encodedData.Length > 20)
                    encodedData = encodedData.Substring(0, 20);
                else
                    encodedData = EncodePasswordToBase64(encodedData);

                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in base64Encode ${ex.Message}");
            }
        }
    }
}
