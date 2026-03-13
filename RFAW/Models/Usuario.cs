namespace RFAW.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public float Moedas { get; set; } = 0; 
        public bool IsAdmin { get; set; } = false; 
    }
}