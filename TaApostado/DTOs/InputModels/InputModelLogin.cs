using System.ComponentModel.DataAnnotations;

namespace TaApostado.DTOs.InputModels
{
    public class InputModelLogin
    {
        [Required(ErrorMessage = "The field email is invalid.")]
        public string email { get; set; }

        [Required(ErrorMessage = "The field password is invalid.")]
        public string password { get; set; }
    }
}
