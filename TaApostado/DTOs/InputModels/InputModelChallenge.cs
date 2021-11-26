using System.ComponentModel.DataAnnotations;

namespace TaApostado.DTOs.InputModels
{
    public class InputModelChallenge
    {
        [Required]
        [StringLengthAttribute(30, MinimumLength = 10, ErrorMessage = "The field name is invalid.")]
        public string name { get; set; }

        [Required]
        [StringLengthAttribute(12, MinimumLength = 3, ErrorMessage = "The field theme is invalid.")]
        public string theme { get; set; }

        [Required(ErrorMessage = "The field value is invalid.")]
        public int bid { get; set; }

        [Required(ErrorMessage = "The field bid is invalid.")]
        public int quota { get; set; }

        [Required]
        [Range(2,1440,ErrorMessage = "The field time open is invalid.")]
        public int timeOpen { get; set; }

        [Required]
        [Range(2,1440,ErrorMessage = "The field time execution is invalid.")]
        public int timeExecution { get; set; }

        [Required]
        [StringLengthAttribute(80, MinimumLength = 30, ErrorMessage = "The field description is invalid.")]
        public string description { get; set; }
    }
}
