namespace Agreement.Models
{
    // Models/AgreementViewModel.cs
    public class AgreementViewModel
    {
        public int Id { get; set; }
        public string DrName { get; set; } = string.Empty;
        public string DrSurname { get; set; } = string.Empty;
        public string DrCell { get; set; } = string.Empty;
        public string PhysEmail { get; set; } = string.Empty;
        public string DateCaptured { get; set; } = string.Empty;
        // Add any other relevant properties
    }

}
