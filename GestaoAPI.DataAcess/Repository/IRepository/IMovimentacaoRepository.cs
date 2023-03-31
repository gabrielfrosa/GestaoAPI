using GestaoAPI.Models;

namespace GestaoAPI.DataAcess.Repository.IRepository
{
    public interface IMovimentacaoRepository
	{
		Task<bool> Add(Movimentacao Movimentacao);
		Task<Movimentacao?> Get(int MovimentacaoId);
		Task<List<Movimentacao>> GetAll(int UsuarioId);
		Task<bool> Update(Movimentacao Movimentacao);
		Task<bool> Delete(Movimentacao Movimentacao);
	}
}