using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agreement.Models
{
    public class AgreementRecord
    {
        [Key]
        public int Id { get; set; }

        public string active { get; set; } = string.Empty;
        public string datecaputred { get; set; } = string.Empty;
        public string timecaptured { get; set; } = string.Empty;
        public string formname { get; set; } = string.Empty;
        public string hosigned { get; set; } = string.Empty;
        public string formid { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        public string drname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Surname is required")]
        [StringLength(100, ErrorMessage = "Surname cannot exceed 100 characters")]
        public string drsurname { get; set; } = string.Empty;

        [Required(ErrorMessage = "ID number is required")]
        [StringLength(50, ErrorMessage = "ID number cannot exceed 50 characters")]
        public string dridnr { get; set; } = string.Empty;

        [Required(ErrorMessage = "BHFF number is required")]
        [StringLength(50, ErrorMessage = "BHFF number cannot exceed 50 characters")]
        public string bhff { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(500, ErrorMessage = "Address cannot exceed {1} characters. Please include street, city, and postal code.")]
        [Display(Name = "Full Address")]
        public string draddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Expiry Date is required")]
        [Display(Name = "Expiry Date")]
        [DataType(DataType.Date)]
        public DateTime hpcsaexpire { get; set; } = DateTime.UtcNow; // Default to UTC

        [Required(ErrorMessage = "Expiry Date is required")]
        [Display(Name = "Expiry Date")]
        [DataType(DataType.Date)]
        public DateTime bohfexpire { get; set; } = DateTime.UtcNow; // Default to UTC

        [Required(ErrorMessage = "Expiry Date is required")]
        [Display(Name = "Expiry Date")]
        [DataType(DataType.Date)]
        public DateTime ppiiexpire { get; set; } = DateTime.UtcNow; // Default to UTC

        [Required(ErrorMessage = "Cellphone number is required")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Please enter a valid 10-digit cellphone number")]
        [Display(Name = "Cellphone Number")]
        public string drcell { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Email Address")]
        public string dremail { get; set; } = string.Empty;


        [Required(ErrorMessage = "Physical Address is required")]
        [StringLength(500, ErrorMessage = "Physical Address cannot exceed {1} characters. Please include street, city, and postal code.")]
        [Display(Name = "Full Address")]
        public string drphysicaddrs { get; set; } = string.Empty;


        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Email Address")]
        public string physemail { get; set; } = string.Empty;

        public string? emerfile { get; set; }
        public string? emerfileStoredName { get; set; }
        public string? qsfile { get; set; }
        public string? qsfileStoredName { get; set; }
        public string? idfile { get; set; }
        public string? idfileStoredName { get; set; }
        public string? ppiifile { get; set; }
        public string? ppiifileStoredName { get; set; }
        public string? bohffile { get; set; }
        public string? bohffileStoredname { get; set; }
        public string? hpcsafile { get; set; }
        public string? hpcsafileStoredName { get; set; }
       

        [NotMapped]
        public IFormFile? boh { get; set; }

        [NotMapped]
        public IFormFile? ppi { get; set; }

        [NotMapped]
        public IFormFile? hpcsa { get; set; }

        [NotMapped]
        public IFormFile? idf { get; set; }

        [NotMapped]
        public IFormFile? qsf { get; set; }

        [NotMapped]
        public IFormFile? emer { get; set; }

        //index2
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        public string drfullname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Signature is required")]
        public string SignatureData { get; set; } = string.Empty;
        public DateTime? SignedDate { get; set; } // Nullable until signed


    }


}

