using GestaoAPI.DataAcess.Data;
using GestaoAPI.DataAcess.Repository.IRepository;
using GestaoAPI.Models;
using GestaoAPI.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAPI.DataAcess.Repository
{
	public class UsuarioRepository : IUsuarioRepository
	{
		private readonly ApplicationDbContext _db;
		public UsuarioRepository(ApplicationDbContext db)
		{
			_db = db;
		}

		public async Task<bool> Add(Usuario Usuario)
		{
			try
			{
				await _db.tbUsuario.AddAsync(Usuario);
				await _db.SaveChangesAsync();
				return true;
			}
			catch
			{
				return false;
			}

		}

		public async Task<Usuario?> GetByEmail(string Email)
		{
			return await _db.tbUsuario.FirstOrDefaultAsync(x => x.Email == Email);
		}
	}
}
