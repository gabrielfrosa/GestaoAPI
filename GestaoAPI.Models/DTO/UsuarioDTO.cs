using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAPI.Models.DTO
{
    public class UsuarioDTO
    {
		[Required(ErrorMessage = "Nome é um campo obrigatório.")]
		public string Nome { get; set; }
		[Required(ErrorMessage = "Email é um campo obrigatório.")]
		[EmailAddress(ErrorMessage = "O Email informado não é válido.")]
		public string Email { get; set; }
		[Required(ErrorMessage = "Password é um campo obrigatório.")]
		public string Password { get; set; }
    }
}