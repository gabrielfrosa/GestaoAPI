using GestaoAPI.DataAcess.Repository.IRepository;
using GestaoAPI.Models.DTO;
using GestaoAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace GestaoAPI.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsuarioController : ControllerBase
	{
		private readonly IUsuarioRepository _usuarioRepository;
		private readonly IConfiguration _configuration;

		public UsuarioController(IUsuarioRepository usuarioRepository, IConfiguration configuration)
		{
			_usuarioRepository = usuarioRepository;
			_configuration = configuration;
		}

		[HttpPost("create")]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		public async Task<ActionResult> Create(UsuarioDTO UsuarioDTO)
		{
			Usuario? usuarioCadastrado = await _usuarioRepository.GetByEmail(UsuarioDTO.Email);
			if (usuarioCadastrado != null)
			{
				return Conflict("Usuário com este Email já cadastrado.");
			}

			CreatePasswordHash(UsuarioDTO.Password, out byte[] passwordHash, out byte[] passwordSalt);
			Usuario usuario = new Usuario()
			{
				Nome = UsuarioDTO.Nome,
				Email = UsuarioDTO.Email,
				PasswordHash = passwordHash,
				PasswordSalt = passwordSalt
			};
			bool result = await _usuarioRepository.Add(usuario);
			if (result)
			{
				return StatusCode(201);
			}
			return BadRequest();
		}

		[HttpPost("login")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<string>> Login(LoginDTO LoginDTO)
		{
			Usuario? usuario = await _usuarioRepository.GetByEmail(LoginDTO.Email);
			if(usuario == null)
			{
				return BadRequest("Usuário ou senha incorretos.");
			}

			if(!VerifyPasswordHash(LoginDTO.Password,usuario.PasswordHash,usuario.PasswordSalt))
			{
				return BadRequest("Usuário ou senha incorretos.");
			}

			string token = CreateToken(usuario);
			
			return Ok(token);
		}

		private string CreateToken(Usuario usuario)
		{
			List<Claim> claims = new List<Claim>()
			{
				new Claim(ClaimTypes.Name, usuario.Nome),
				new Claim(ClaimTypes.Email, usuario.Email)
			};

			var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Key").Value));

			var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

			double lifetime = 30;
			double.TryParse(_configuration.GetSection("JwtConfig:Key").Value, out lifetime);

			string audience = _configuration.GetSection("JwtConfig:Audience").Value;
			
			string issuer = _configuration.GetSection("JwtConfig:Issuer").Value;

			var token = new JwtSecurityToken(
				claims: claims,
				expires: DateTime.Now.AddMinutes(lifetime),
				signingCredentials: cred,
				audience: audience,
				issuer: issuer);

			var jwt = new JwtSecurityTokenHandler().WriteToken(token);

			return jwt;
		}

		#region Password
		private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
		{
			using (var hmac = new HMACSHA512())
			{
				passwordSalt = hmac.Key;
				passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
			}
		}

		private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
		{
			using (var hmac = new HMACSHA512(passwordSalt))
			{
				var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
				return computedHash.SequenceEqual(passwordHash);
			}
		}
		#endregion
	}
}
