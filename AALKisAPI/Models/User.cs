using System.ComponentModel.DataAnnotations.Schema;

namespace AALKisAPI.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set;}
        public string Password { get; set; }
        public string? PasswordCheck { get; set; }
        public string? Email { get; set; }
    }
}
