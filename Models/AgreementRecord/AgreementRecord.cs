using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agreement.Models
{
    public class AgreementRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Surname { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? MouFileName { get; set; }
        public string? MouStoredName { get; set; }

        public string? NdaFileName { get; set; }
        public string? NdaStoredName { get; set; }

        [NotMapped]
        public IFormFile? Mou { get; set; }

        [NotMapped]
        public IFormFile? Nda { get; set; }
    }


}

