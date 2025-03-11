using System.ComponentModel.DataAnnotations;

namespace Project.DTO
{
    public class CustomCartDTO
    { 
        public int Id { get; set; }

        public int PId { get; set; }

        public string PName { get; set; }

        public string PType { get; set; }

        public string PCategory { get; set; }

        public int PCount { get; set; }

        public string PSize { get; set; }

        public string PColor { get; set; }

        public string? CustomText0 { get; set; }

        public string? CustomText1 { get; set; }

        public string? CustomPhoto0 { get; set; }

        public string? CustomPhoto1 { get; set; }

        public string? Photo0 { get; set; }

        public string? Photo1 { get; set; }

        public int PPrice { get; set; }
    }
}
