namespace RFAW.Models
{
    public class MensagemSuporte
    {
        public int Id { get; set; }
        public string Nome { get; set; } = "";
        public string Email { get; set; } = "";
        public string Mensagem { get; set; } = "";
        public DateTime DataEnvio { get; set; } = DateTime.Now;
        public bool Lida { get; set; } = false;
    }
}