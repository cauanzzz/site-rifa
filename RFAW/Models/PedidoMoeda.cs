using System;

namespace RFAW.Models
{
    public class PedidoMoeda
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Pacote { get; set; }
        public int QuantidadeMoedas { get; set; }
        public decimal ValorPago { get; set; }
        public string Status { get; set; } = "Pendente";
        public DateTime DataSolicitacao { get; set; } = DateTime.Now;
        public string NomeTitularPix { get; set; } = "";
        

    }
}