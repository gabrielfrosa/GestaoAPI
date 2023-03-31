using GestaoAPI.Models;
using GestaoAPI.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAPI.DataAcess.Repository.IRepository
{
    public interface IUsuarioRepository
	{
		Task<bool> Add(Usuario Usuario);

		Task<Usuario?> GetByEmail(string Email);
	}
}
