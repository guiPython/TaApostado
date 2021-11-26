using System.ComponentModel.DataAnnotations;

namespace TaApostado.DTOs.InputModels
{
    public class InputModelVote
    {
        [Required]
        public string user_password { get; set; }

        [Required]
        public string vote { get; set; }
    }
}
