using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api_asp_net_4.Data;
using System.ComponentModel.DataAnnotations;

namespace api_asp_net_4.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProdutosController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retorna uma lista com detalhes de todos os produtos
        /// </summary>
        /// <returns>Lista com detalhes de todos os produtos</returns>
        /// <response code="200">Retorna a lista requisitada</response>
        /// <response code="401">Se o autor da requisicao nao possui autorizacao</response>
        /// <response code="500">Se houve falha na conexao com o banco de dados</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {
            return await _context.Produtos.ToListAsync();
        }

        /// <summary>
        /// Retorna os detalhes do produto requisitado
        /// </summary>
        /// <remarks>
        ///     Exemplo de requisicao:
        ///         GET api/Produtos/abcd1234-abcd1234-abcd-1234-abcd1234
        /// </remarks>
        /// <param name="id"></param>
        /// <returns>Os detalhes do produto requisitado</returns>
        /// <response code="200">Retorna o produto requisitado</response>
        /// <response code="404">Se não houver produto com esse id</response>
        /// <response code="401">Se o autor da requisicao nao possui autorizacao</response>
        /// <response code="500">Se houve falha de conexao com o banco de dados</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Produto>> GetProduto(string id)
        {
            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)
            {
                return NotFound();
            }

            return produto;
        }

        /// <summary>
        /// Altera informacoes de um produto existente
        /// </summary>
        /// <param name="id"></param>
        /// <param name="produto"></param>
        /// <remarks>
        ///     Exemplo de requisicao:
        ///         Put api/Produtos/abcd1234-abcd1234-abcd-1234-abcd1234
        /// </remarks>
        /// <returns>Os detalhes atualizados do produto requisitado</returns>
        /// <response code="204">Atualiza o produto requisitado</response>
        /// <response code="400">Se nao existe produto com Id informado ou "body" possui valor invalido ou o formato e ilegivel</response>
        /// <response code="401">Se o autor da requisicao nao possui autorizacao</response>
        /// <response code="500">Se houve falha de conexao com o banco de dados</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduto(string id, Produto produto)
        {
            if (id != produto.Id)
            {
                return BadRequest();
            }

            _context.Entry(produto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProdutoExists(id))
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
        /// Cria um novo produto
        /// </summary>
        /// <param name="produto"></param>
        /// <remarks>
        ///     Requisicao padrao:
        ///         Post api/Produtos/
        ///         
        ///     No Body, insira os campos Id, Nome, Valor e Ativo atualizados do produto
        ///     
        ///     Campo "Id" precisa ter 36 caracteres
        /// </remarks>
        /// <returns>Os detalhes do produto criado</returns>
        /// <response code="200">Retorna os detalhes do produto criado</response>
        /// <response code="401">Se o autor da requisicao nao possui autorizacao</response>
        /// <response code="409">Se ja houver um produto com id fornecido</response>
        /// <response code="500">Se houve falha de conexao com o banco de dados</response>
        [ProducesResponseType(typeof(Produto), 201)]
        [ProducesResponseType(401)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        [HttpPost]
        public async Task<ActionResult<Produto>> PostProduto(Produto produto)
        {
            _context.Produtos.Add(produto);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProdutoExists(produto.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetProduto", new { id = produto.Id }, produto);
        }

        /// <summary>
        /// Exclui um produto existente
        /// </summary>
        /// <param name="id"></param>
        /// <remarks>
        ///     Exemplo de requisicao:
        ///         Delete api/Produtos/abcd1234-abcd1234-abcd-1234-abcd1234
        /// </remarks>
        /// <response code="204">Exclui um produto existente</response>
        /// <response code="401">Se o autor da requisicao nao possui autorizacao</response>
        /// <response code="404">Se nao existe produto com Id informado</response>
        /// <response code="500">Se houve falha de conexao com o banco de dados</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduto(string id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProdutoExists(string id)
        {
            return _context.Produtos.Any(e => e.Id == id);
        }
    }
}
