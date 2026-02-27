using Microsoft.Extensions.Primitives;
using System.ComponentModel.DataAnnotations.Schema;

namespace RFAW.Models.NovaPasta
{
    public class Rifa
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public float Preço { get; set; }
        public int QuantidadeCotas { get; set; }
        public List<Cota> Cotas { get; set; } = new();
    }
    public class Cota
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public string Status { get; set; } = "Disponivel";
        public int RifaId { get; set; }
        public string? Nome { get; set; }
        public string? Tel { get; set; }
           
    }
}
