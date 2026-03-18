using Microsoft.Extensions.Primitives;
using System.ComponentModel.DataAnnotations.Schema;

namespace RFAW.Models
{
    public class Rifa
    {
        public string CriadorEmail { get; set; } = "";
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string? Descricao { get; set; } 
        public string? Premio { get; set; } 
        public string? Imagem { get; set; } 
        public float Preço { get; set; }
        public int QuantidadeCotas { get; set; }
        public DateTime? DataSorteio { get; set; }
        public List<Cota> Cotas { get; set; } = new();
    }
    public class Cota
    {
        public string CompradorEmail { get; set; } = "";
        public int Id { get; set; }
        public int Numero { get; set; }
        public string Status { get; set; } = "Disponivel";
        public int RifaId { get; set; }
        public string? Nome { get; set; }
        public string? Tel { get; set; }
        public DateTime? DataReserva { get; set; }

    }
}
