using GestaoAPI.DataAcess.Repository.IRepository;
using GestaoAPI.Models;
using GestaoAPI.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestaoAPI.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class MovimentacaoController : ControllerBase
	{
		private readonly IMovimentacaoRepository _movimentacaoRepository;
		private readonly IUsuarioRepository _usuarioRepository;
		public MovimentacaoController(IMovimentacaoRepository movimentacaoRepository, IUsuarioRepository usuarioRepository)
		{
			_movimentacaoRepository = movimentacaoRepository;
			_usuarioRepository = usuarioRepository;	
		}

		[HttpPost("create")]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult> Create(MovimentacaoDTO MovimentacaoDTO)
		{
			Usuario? usuario = await GetUsuario();
			if (usuario == null)
			{
				return NotFound("Usuário não encontrado.");
			}

			Movimentacao Movimentacao = new Movimentacao()
			{
				Tipo = MovimentacaoDTO.Tipo,
				Valor = MovimentacaoDTO.Valor,				
				DataCriacao = DateTime.Now,
				UsuarioId = usuario.Id
			};

			bool result = await _movimentacaoRepository.Add(Movimentacao);

			if (result)
			{
				return StatusCode(201);
			}
			return BadRequest();
		}

		[HttpGet("get")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<List<Movimentacao>>> Get(int Tipo)
		{
			if (Tipo != 0 && Tipo != 1 && Tipo != 2)
			{
				return BadRequest("Tipo deve ser 0 para Todos, 1 para Gasto e 2 para Receita.");
			}

			Usuario? usuario = await GetUsuario();

			if (usuario == null)
			{
				return NotFound("Usuário não encontrado.");
			}

			List<Movimentacao> listaMovimentacao = await _movimentacaoRepository.GetAll(usuario.Id);
			if (Tipo == 0)
			{
				return Ok(listaMovimentacao);
			}

			return Ok(listaMovimentacao.Where(x => x.Tipo == Tipo));
		}

		[HttpPost("update")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult> Update(int MovimentacaoId, MovimentacaoDTO MovimentacaoDTO)
		{
			Usuario? usuario = await GetUsuario();
			if (usuario == null)
			{
				return NotFound("Usuário não encontrado.");
			}

			Movimentacao? Movimentacao = await _movimentacaoRepository.Get(MovimentacaoId);

			if(Movimentacao == null)
			{
				return NotFound("Nenhuma Movimentação com este Id.");
			}

			Movimentacao.Tipo = MovimentacaoDTO.Tipo;
			Movimentacao.Valor = MovimentacaoDTO.Valor;

			bool result = await _movimentacaoRepository.Update(Movimentacao);

			if (result)
			{
				return StatusCode(200);
			}
			return BadRequest();
		}

		[HttpPost("delete")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult> Delete(int MovimentacaoId)
		{			
			Movimentacao? Movimentacao = await _movimentacaoRepository.Get(MovimentacaoId);

			if (Movimentacao == null)
			{
				return NotFound("Nenhuma Movimentação com este Id.");
			}

			bool result = await _movimentacaoRepository.Delete(Movimentacao);

			if (result)
			{
				return StatusCode(204);
			}
			return BadRequest();
		}

		[HttpGet("saldo")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<double>> Saldo(int Tipo)
		{
			if (Tipo != 1 && Tipo != 2)
			{
				return BadRequest("Tipo deve ser 1 para Gasto e 2 para Receita.");
			}

			Usuario? usuario = await GetUsuario();

			if (usuario == null)
			{
				return NotFound("Usuário não encontrado.");
			}

			List<Movimentacao> listaMovimentacao = await _movimentacaoRepository.GetAll(usuario.Id);

			return listaMovimentacao.Where(x => x.Tipo == Tipo).Sum(x => x.Valor);
		}

		private async Task<Usuario?> GetUsuario()
		{
			var usuarioEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

			return await _usuarioRepository.GetByEmail(usuarioEmail);
		}
	}
}
