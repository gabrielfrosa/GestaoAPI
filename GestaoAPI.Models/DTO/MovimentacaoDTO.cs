using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAPI.Models.DTO
{
    public class MovimentacaoDTO
    {
        [Required]
		[Range(1, 2, ErrorMessage = "Tipo deve ser 1 para Gasto e 2 para Receita.")]
		public int Tipo { get; set; }
        [Required]
		[Range(0.0001, double.MaxValue, ErrorMessage = "O valor deve ser maior que 0.")]
		public double Valor { get; set; }        
    }
}
