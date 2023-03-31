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
    public class MovimentacaoRepository : IMovimentacaoRepository
	{
		private readonly ApplicationDbContext _db;
		public MovimentacaoRepository(ApplicationDbContext db)
		{
			_db = db;
		}

		public async Task<bool> Add(Movimentacao Movimentacao)
		{
			try
			{
				await _db.tbMovimentacao.AddAsync(Movimentacao);
				await _db.SaveChangesAsync();
				return true;
			}
			catch
			{
				return false;
			}

		}

		public async Task<Movimentacao?> Get(int MovimentacaoId)
		{
			return await _db.tbMovimentacao.FirstOrDefaultAsync(x => x.Id == MovimentacaoId);
		}

		public async Task<List<Movimentacao>> GetAll(int UsuarioId)
		{
			return await _db.tbMovimentacao.Where(x => x.UsuarioId == UsuarioId).ToListAsync();
		}		

		public async Task<bool> Update(Movimentacao Movimentacao)
		{
			try
			{
				_db.tbMovimentacao.Update(Movimentacao);
				await _db.SaveChangesAsync();
				return true;
			}
			catch
			{
				return false;
			}
		}

		public async Task<bool> Delete(Movimentacao Movimentacao)
		{
			try
			{
				_db.tbMovimentacao.Remove(Movimentacao);
				await _db.SaveChangesAsync();
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}